
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class AntBase : MonoBehaviour
{
    public BehaviorGraphAgent _antTree;
    protected Resource trackResource;
    private Poolable poolable;
    protected NavMeshAgent Agent;

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
    }
    protected virtual void OnStopTrackingResource(Resource resource)
    {
        if (trackResource == null) return;
        resource.OnDepleteResource -= ResourceDepleted;
        _antTree.SetVariableValue<Resource>("nearestResourceOfType", null);
        trackResource = null;
    }
}
