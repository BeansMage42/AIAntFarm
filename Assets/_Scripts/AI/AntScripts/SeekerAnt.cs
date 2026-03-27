
using UnityEngine;
using UnityEngine.AI;

public class SeekerAnt:AntBase
{
    public ResourceType _seekingResourceType;
    public QuadTreeManager _quadTreeManager;
    public Collider _antBounds;


   

    public FSM_State WanderState;
    private FSM_State SeekResourceState;
    private FSM_State ReturnHomeState;
    private FSM_State WaitForSpawnState;
    private FSM_State GuideState;

    private Vector3 Target;
    private float resourceStrength;
    public float resourceStrengthBeforeFullPivot;

    private Vector3 wanderDir;

    [SerializeField] private float randomPositionSamplingDistance;

    private bool groupSpawned;
    [SerializeField] AntSpawner spawner;
    private void Awake()
    {
        spawner.AllAntsASpawned += () => { groupSpawned = true; };
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("doodleDee");
        AntBehaviour = new FSM_StateMachine();

        WanderState = new FSM_State(WanderRandom, MoveToTarget, () => { Debug.Log("on leave wander action"); });
        WanderState.AddTransition(new FSM_Transition(WanderState, () => { Debug.Log("wander to wander transition"); }, ReachedTarget ));

        SeekResourceState = new FSM_State(QueryTreeForScent,MoveToTrackedResource, () => { Debug.Log("on leave seek action"); });
        WanderState.AddTransition(new FSM_Transition(SeekResourceState, () => { Debug.Log("wander to seekResource state transition"); }, IsWithinProximityOfResource));

        SeekResourceState.AddTransition(new FSM_Transition(WanderState, () => { Debug.Log("seek to wander transition"); }, () => { return !HasFoundResource(); }));
       
        ReturnHomeState = new FSM_State(GoHome,MoveToTarget, () => { Debug.Log("on exit return home state"); });
        WaitForSpawnState = new FSM_State(SpawnAnts, GoHome, () => { Debug.Log("on exit wait state"); });
        SeekResourceState.AddTransition(new FSM_Transition(ReturnHomeState, () => { Debug.Log("seek to return to home transition"); }, ReachedTarget));
        ReturnHomeState.AddTransition(new FSM_Transition(WaitForSpawnState, () => { Debug.Log("home to wait transition"); }, ReachedTarget));


        GuideState = new FSM_State(MoveToTrackedResource, MoveToTrackedResource, null);
        WaitForSpawnState.AddTransition(new FSM_Transition(GuideState, () => { Debug.Log("Wait to guide transition"); }, GroupSpawned));

        //ReturnHomeState.AddTransition


        AntBehaviour.JumpToState(WanderState);

    }

    private void Update()
    {
        AntBehaviour.Update();
    }
    public void SpawnAnts()
    {
        spawner.SpawnAntsInNewGroupWithLeader(gameObject.transform);
    }
    public void QueryTreeForScent()
    {
        if (_quadTreeManager.TreeContainsBounds(_antBounds.bounds, out Quad[] searchForIntersections))
        {
            Resource bestResource = null;
            float strongest = -float.MaxValue;
            foreach (var quad in searchForIntersections)
            {
                foreach (var scent in quad._scents.Keys)
                {
                    if (scent == _seekingResourceType)
                    {
                        if (quad._scents[scent].Item2 > strongest)
                        {
                            bestResource = quad._scents[scent].Item1;
                            strongest = quad._scents[scent].Item2;
                        }
                    }
                }
            }
            if (bestResource != null)
            {
                Debug.Log("track new targfet");
                resourceStrength = strongest;
                BeginTrackingResource(bestResource);
            }
            else
            {
                Debug.Log("lost target");
                resourceStrength = 0.0f;
                OnStopTrackingResource(trackResource);
            }
        }
        else
        {
        Debug.Log("not even on the tree");
        resourceStrength = 0.0f;
        OnStopTrackingResource(trackResource);

        }

    }

    public bool GroupSpawned()
    {
        return groupSpawned;
    }
    public bool HasFoundResource()
    {
        return (trackResource != null);
    }

    public bool IsWithinProximityOfResource()
    {
        return resourceStrength >= resourceStrengthBeforeFullPivot;
    }
    public void WanderRandom()
    {
        QueryTreeForScent();
        if (wanderDir == Vector3.zero)
            wanderDir = transform.forward;

        wanderDir += new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            0,
            UnityEngine.Random.Range(-1f, 1f)
        ) * 0.5f;

        //wanderDir = wanderDir.normalized;


        Vector3 toCenter = Vector3.zero - transform.position;
        toCenter.y = 0f;
        float centerWeight = Mathf.Clamp01(toCenter.magnitude / 50f);
        //Debug.Log(centerWeight);
        wanderDir = Vector3.Lerp(wanderDir.normalized, toCenter.normalized, centerWeight);
        if (trackResource != null) wanderDir = Vector3.Lerp(wanderDir, (trackResource.gameObject.transform.position - transform.position).normalized, resourceStrength);

        Vector3 target = transform.position
                       + (wanderDir * randomPositionSamplingDistance);
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
        {
            target = hit.position;
        }
        Target = target;
    }
    public void MoveToTrackedResource()
    {
        Target = trackResource.bounds.center;
        MoveToTarget();
    }
    public void GoHome()
    {
        Target = home.transform.position;
    }
    public void MoveToTarget()
    {
        Debug.Log("set target");
        Agent.SetDestination(Target);
    }

    public bool ReachedTarget()
    {
        return (Agent.remainingDistance <= Agent.stoppingDistance);
        
    }

    protected override void ResourceDepleted(Resource source)
    {
        OnStopTrackingResource(source);
    }
    protected override void BeginTrackingResource(Resource resource)
    {
        base.BeginTrackingResource(resource);
    }
    protected override void OnStopTrackingResource(Resource resource) 
    {
        base.OnStopTrackingResource(resource);
    }

}
