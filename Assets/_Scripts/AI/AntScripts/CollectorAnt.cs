using System;
using UnityEngine;
using UnityEngine.AI;
public class CollectorAnt:AntBase
{


    public NavMeshPath toResourceFromHome;
    public NavMeshPath ToHomefromResource;
    public Agent boidControl;
    public Group currentGroup;
    public float carryingCapacity;
    public (ResourceType, float) carried;
    public Action extractedResource;
    public Action<CollectorAnt> returnedHome;
    //public Action foundResource;
    public float extractSpeed;
    public bool IsCollecting;
    public bool extractionComplete;

    private FSM_State WaitState;
    private FSM_State ReturnHomeState;
    private FSM_State BoidState;
   // private FSM_State ReturnHomeWithResourceState;
    private FSM_State FollowPathState;
    private FSM_State DepositResourceState;
    private FSM_State ExtractResourceState;

    Vector3 targetPos;
    bool FirstInLine;

    private bool queueMode=true;
    protected override void Start()
    {
        base.Start();
        AntBehaviour = new FSM_StateMachine();
        boidControl = GetComponent<Agent>();
        BoidState = new FSM_State(null, null, null);
        WaitState = new FSM_State(() => { queueMode = true; }, MoveTo, () => { queueMode = false; });
        ExtractResourceState = new FSM_State(ExtractResource, null, SendExtractResourceEvent);
        FollowPathState = new FSM_State(FollowPathHome, FollowPathHome, null);
        // ReturnHomeWithResourceState = new FSM_State(FollowPathHome, null, SetFrontOfLine);
        DepositResourceState = new FSM_State(ReturnedHome, null, null);
        ReturnHomeState = new FSM_State(ReturnHome, MoveTo, null);

        BoidState.AddTransition(new FSM_Transition(WaitState, null, NavMeshState));

        WaitState.AddTransition(new FSM_Transition(ExtractResourceState, null, () => { return IsFirstInLine() && ReachedTarget(); }));
        WaitState.AddTransition(new FSM_Transition(ReturnHomeState, null, () => { return !HasFoundResource(); }));

        //ExtractResourceState.AddTransition(new FSM_Transition(ReturnHomeWithResourceState, null, IsCarrying));
        ExtractResourceState.AddTransition(new FSM_Transition(FollowPathState, null, FinishedCollecting));
        //FollowPathState.AddTransition(new FSM_Transition(FollowPathState,null ,() => { return !CompletedPath(); }));
        FollowPathState.AddTransition(new FSM_Transition(DepositResourceState, null, () => {return ReachedTarget() && CompletedPath(); }));
     //   ReturnHomeState.AddTransition(new FSM_Transition(DepositResourceState, () => { Debug.Log("enter deposit state"); pathIndex = 0; }, ReachedTarget));
       // ReturnHomeWithResourceState.AddTransition(new FSM_Transition(DepositResourceState,null, ReachedTarget));
        DepositResourceState.AddTransition(new FSM_Transition(WaitState, null, () => { return HasFoundResource(); }));
        ReturnHomeState.AddTransition(new FSM_Transition(DepositResourceState, null, () => { return ReachedTarget() && FinishedCollecting(); }));

      //  ExtractResourceState.AddTransition(new FSM_Transition(ReturnHomeState,null, () => { return !HasFoundResource(); }));
       // ReturnHomeState.AddTransition(new FSM_Transition(DepositResourceState, null, IsCarrying));

        //ExtractResourceState.AddTransition()

        AntBehaviour.JumpToState(BoidState);
    }
    public override void RecallAnt()
    {
        base.RecallAnt();
        AntBehaviour.JumpToState(ReturnHomeState);
    }
    public void ResourceFound(Resource source,  NavMeshPath toResource, NavMeshPath fromResource)
    {
       // toResourceFromHome = toResource;
        ToHomefromResource = fromResource;
        BeginTrackingResource(source);

    }
    public void SwitchToNavmeshControl()
    {
        boidControl.Enabled = false;
        Agent.enabled = true;

    }
    public bool NavMeshState()
    {
        return Agent.enabled;
    }

    public void MoveTo(Vector3 pos)
    {
        Agent.SetDestination(pos);
    }
    public async void ExtractResource()
    {
        IsCollecting = true;
        if(trackResource != null)
        {
            await Awaitable.WaitForSecondsAsync(extractSpeed);
            if(trackResource == null)
            {
                IsCollecting = false;
                return;
            }
            float amount =0;
            ResourceType type = trackResource.resourceType;
            trackResource.ExtractResource(carryingCapacity,out amount);
            carried = (type, amount);
            //extractedResource?.Invoke();
            Debug.Log(name + " collected " + carried.Item2 + " " + carried.Item1);
            IsCollecting = false;
            extractionComplete = true;
            // Agent.Warp(ToHomefromResource.corners[0]);
            //Agent.SetPath(ToHomefromResource);
        }
    }
    private void SendExtractResourceEvent()
    {
        extractionComplete = false;
        extractedResource?.Invoke();
    }
    public bool FinishedCollecting()
    {
        return !IsCollecting && extractionComplete;
    }
    private void Update()
    {
        AntBehaviour.Update();
    }
    public void MoveTo()
    {
        Agent.SetDestination(targetPos);
    }
    public bool ReachedTarget()
    {
        
      return Agent.remainingDistance <= Agent.stoppingDistance && !Agent.pathPending && Agent.hasPath;
    }
    public void ReturnedHome()
    {
        Debug.Log($"Ant {name} returned home and deposited {carried.Item2} stuff and thyings");
        if (carried.Item2 > 0)
        {
            TheVault.Instance.ChangeResourceAmountOfType(carried.Item1, carried.Item2);
            carried.Item2 = 0;
          //  ReturnAntToPool();
        }
        extractionComplete = false;
        // returnedHome?.Invoke(this);
    }
    protected override void ResourceDepleted(Resource source)
    {
        base.ResourceDepleted(source);
        IsCollecting = false;
        
       // ReturnHome();
       AntBehaviour.JumpToState(ReturnHomeState);
        //Agent.SetDestination(home.transform.position);
    }


    public void SetTargetPosition(Vector3 position)
    {
        if (!queueMode) return;
        targetPos = position;
    }

    public void SetFrontOfLine()
    {
        FirstInLine = !FirstInLine;
    }
    public bool IsFirstInLine()
    {
        return FirstInLine;
    }
    public bool IsCarrying()
    {
        return carried.Item2 != 0;
    }
    public bool HasFoundResource()
    {
        return (trackResource != null);
    }
    public void ReturnHome()
    {
        targetPos = home.transform.position;
    }
    int pathIndex = 0;
    public void FollowPathHome()
    {
       // ReturnHome();

        targetPos = ToHomefromResource.corners[pathIndex];
       if(pathIndex < ToHomefromResource.corners.Length-1 && ReachedTarget()) pathIndex++;
        MoveTo();
    }
    public bool CompletedPath()
    {
        return pathIndex >= ToHomefromResource.corners.Length -1;
    }
    
}
