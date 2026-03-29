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
    public float extractSpeed;
    public bool IsCollecting;

    private FSM_State WaitState;
    private FSM_State ReturnHomeState;
    private FSM_State BoidState;
    private FSM_State ReturnHomeWithResource;


    protected override void Start()
    {
        base.Start();
        boidControl = GetComponent<Agent>();
    }

    public void ResourceFound(Resource source,  NavMeshPath toResource, NavMeshPath fromResource)
    {
        toResourceFromHome = toResource;
        ToHomefromResource = fromResource;
        BeginTrackingResource(source);

    }
    public void SwitchToNavmeshControl()
    {
        boidControl.Enabled = false;
        Agent.enabled = true;

    }

    public void MoveTo(Vector3 pos)
    {
        Agent.SetDestination(pos);
    }
    public async void ExtractResource()
    {
        if(trackResource != null && !IsCollecting)
        {
            IsCollecting = true;
            await Awaitable.WaitForSecondsAsync(extractSpeed);
            float amount =0;
            ResourceType type = trackResource.resourceType;
            trackResource.ExtractResource(carryingCapacity,out amount);
            carried = (type, amount);
            extractedResource?.Invoke();
            Debug.Log(name + " collected");
            Agent.SetPath(ToHomefromResource);
        }
    }
    public bool ReachedTarget()
    {
        //  return (Agent.remainingDistance <= Agent.stoppingDistance);
        Debug.Log($"{name} is {Vector3.Distance(transform.position, trackResource.transform.position)} from target resource");
      return Vector3.Distance(transform.position,trackResource.transform.position) <= Agent.stoppingDistance +1;
    }
    public void ReturnedHome()
    {
        IsCollecting = false;
        TheVault.Instance.ChangeResourceAmountOfType(carried.Item1, carried.Item2);
        returnedHome?.Invoke(this);
    }

}
