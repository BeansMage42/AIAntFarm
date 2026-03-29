using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wander", story: "[Agent] Wanders until [Resource] is found", category: "Action", id: "79489d3f26015061f542986a36308a54")]
public partial class WanderAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<Resource> Resource;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<float> RotSpeed;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> resourceStrength;


    Vector3 wanderDir;

    protected override Status OnStart()
    {
        if (wanderDir == Vector3.zero)
            wanderDir = Self.Value.transform.forward;

        wanderDir += new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            0,
            UnityEngine.Random.Range(-1f, 1f)
        ) * 0.5f;

        //wanderDir = wanderDir.normalized;

        
        Vector3 toCenter = Vector3.zero - Self.Value.transform.position;
        toCenter.y = 0f;
        float centerWeight = Mathf.Clamp01(toCenter.magnitude/50f );
        //Debug.Log(centerWeight);
        wanderDir = Vector3.Lerp(wanderDir.normalized, toCenter.normalized, centerWeight);
        if(Resource.Value != null) wanderDir = Vector3.Lerp(wanderDir, (Resource.Value.transform.position - Self.Value.transform.position).normalized, resourceStrength.Value);

        Vector3 target = Self.Value.transform.position
                       + (wanderDir * Distance.Value); 
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
        {
            target = hit.position;
        }

        if (Vector3.Distance(Agent.Value.destination, target) > 1f)
        {
            Agent.Value.SetDestination(target);
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        
        if (Agent.Value.remainingDistance > Agent.Value.stoppingDistance) 
        {
        return Status.Running;
        }
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

