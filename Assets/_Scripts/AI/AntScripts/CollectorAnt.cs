using UnityEngine;
using UnityEngine.AI;
public class CollectorAnt:AntBase
{


    public NavMeshPath toResourceFromHome;
    public NavMeshPath ToHomefromResource;
    public Agent boidControl;


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
}
