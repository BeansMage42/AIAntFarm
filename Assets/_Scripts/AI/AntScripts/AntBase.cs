
using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class AntBase : MonoBehaviour
{
    //public BehaviorGraphAgent _antTree;
    public FSM_StateMachine AntBehaviour;
    protected Resource trackResource;
    private Poolable poolable;
    protected NavMeshAgent Agent;
    [SerializeField] protected GameObject home;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        poolable = GetComponent<Poolable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void ResourceDepleted(Resource source)
    {
        OnStopTrackingResource(source);
        Debug.Log("stop tracking");
    }
    protected virtual void BeginTrackingResource(Resource resource)
    {
        if (trackResource == resource) return;
        resource.OnDepleteResource += ResourceDepleted;
        trackResource = resource;
        Debug.Log("track new resource " +  resource.name);
    }
    protected virtual void OnStopTrackingResource(Resource resource)
    {
        if (trackResource == null) return;
        Debug.Log("on stop tracking resource");
        resource.OnDepleteResource -= ResourceDepleted;
        //_antTree.SetVariableValue<Resource>("nearestResourceOfType", null);
        trackResource = null;
    }
    protected virtual void ReturnAntToPool()
    {
        poolable.Pool.ReturnToPool(gameObject);
    }
}
