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
        ReturnHomeState = new FSM_State(ReturnHome, () => { Debug.Log("return home"); if (WasRecalled && ReachedTarget()) { Debug.Log("return to pool check in state");WasRecalled = false; ReturnAntToPool(); } else { Debug.Log($"was recalled:{WasRecalled} Reached Target:{ReachedTarget()} Distance left: {Agent.remainingDistance} has path{Agent.hasPath}"); MoveTo(); }  }, null);

        BoidState.AddTransition(new FSM_Transition(WaitState, null, NavMeshState));

        WaitState.AddTransition(new FSM_Transition(ExtractResourceState, null, () => { return IsFirstInLine() && ReachedTarget(); }));
        WaitState.AddTransition(new FSM_Transition(ReturnHomeState, null, () => { return !HasFoundResource(); }));

        ExtractResourceState.AddTransition(new FSM_Transition(FollowPathState, null, FinishedCollecting));
        FollowPathState.AddTransition(new FSM_Transition(ReturnHomeState, ReturnHome, () => {return ReachedTarget() && CompletedPath(); }));
        DepositResourceState.AddTransition(new FSM_Transition(BoidState,() => { Debug.Log("go to pool"); ReturnAntToPool();  }, () => WasRecalled));
        DepositResourceState.AddTransition(new FSM_Transition(WaitState, null, () => { return HasFoundResource(); }));
        //ReturnHomeState.AddTransition(new FSM_Transition(BoidState, /*() => { Debug.Log("go to pool"); ReturnAntToPool();  }*/null, () => { return ReachedTarget() && WasRecalled; }));
        ReturnHomeState.AddTransition(new FSM_Transition(DepositResourceState, null, () => { return ReachedTarget() && FinishedCollecting(); }));
        ReturnHomeState.AddTransition(new FSM_Transition(BoidState, () => { Debug.Log("return transition to pool"); ReturnAntToPool(); }, ReachedTarget));
       

        AntBehaviour.JumpToState(BoidState);
    }
    public override void RecallAnt()
    {
        base.RecallAnt();
        WasRecalled = true;
        //if (AntBehaviour.CurrentState == FollowPathState) return;
        IsCollecting = false;
        
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
        //extractionComplete = false;
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
            extractionComplete = false;
            ReturnAntToPool();
        //returnedHome?.Invoke(this);
        }
        extractionComplete = false;
    }
    protected override void ResourceDepleted(Resource source)
    {
        base.ResourceDepleted(source);
        IsCollecting = false;

        // ReturnHome();
       // WasRecalled = true;
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
        //WasRecalled = true;
        Agent.stoppingDistance += 0.5f;
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
        return pathIndex >= ToHomefromResource.corners.Length-1;
    }
    
}
