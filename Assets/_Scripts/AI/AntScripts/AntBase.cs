
using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class AntBase : MonoBehaviour
{
    //public BehaviorGraphAgent _antTree;
    //[SerializeReference]
    public FSM_StateMachine AntBehaviour;
    public Resource trackResource;
    private Poolable poolable;
    protected NavMeshAgent Agent;
    protected GameObject home;
    protected bool WasRecalled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        poolable = GetComponent<Poolable>();
        home = GameManager.instance.Home;
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void RecallAnt()
    {
        OnStopTrackingResource(trackResource);
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
        Debug.Log("return to pool");
        if(poolable.Pool == null) { poolable.Pool = new ObjectPool(gameObject, 0); poolable.Pool.ActivePool.Add(gameObject); }//temp solution
        poolable.Pool.ReturnToPool(gameObject);
    }
}
