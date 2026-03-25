using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Query seeker for resource scent", story: "Query [Seeker] For [Resource] Type", category: "Action", id: "7b8d3afd7051f61885934b245d4eb0f3")]
public partial class QuerySeekerForResourceScentAction : Action
{
    [SerializeReference] public BlackboardVariable<SeekerAnt> Seeker;
    [SerializeReference] public BlackboardVariable<int> _Resource;
    [SerializeReference] public BlackboardVariable<float> strength;
    [SerializeReference] public BlackboardVariable<Resource> foundResource;
    protected override Status OnStart()
    {
        (Resource,float) found = Seeker.Value.QueryTreeForScent(_Resource.Value);

        if(foundResource.Value != found.Item1)
        {
            foundResource.Value = found.Item1;
        }
        strength.Value = found.Item2;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

