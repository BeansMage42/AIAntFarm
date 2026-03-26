
using UnityEngine;
using UnityEngine.AI;

public class SeekerAnt:AntBase
{
    public ResourceType _seekingResourceType;
    public QuadTreeManager _quadTreeManager;
    public Collider _antBounds;


    public FSM_StateMachine seekerAntBehaviour;

    public FSM_State WanderState;

    private Vector3 Target;
    private float resourceStrength;
    public float resourceStrengthBeforeFullPivot;

    private Vector3 wanderDir;

    [SerializeField] private float randomPositionSamplingDistance;
    private void Awake()
    {
      
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("doodleDee");
        seekerAntBehaviour = new FSM_StateMachine();

        WanderState = new FSM_State(WanderRandom, MoveToTarget, null);
        WanderState.AddTransition(new FSM_Transition(WanderState, null, ReachedTarget ));

        seekerAntBehaviour.JumpToState(WanderState);

    }

    private void Update()
    {
        seekerAntBehaviour.Update();
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
                resourceStrength = strongest;
                BeginTrackingResource(bestResource);
            }
            else
            {
                resourceStrength = 0.0f;
                OnStopTrackingResource(trackResource);
            }
        }
        resourceStrength = 0.0f;
        OnStopTrackingResource(trackResource);

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
