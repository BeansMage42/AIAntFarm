using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class Group : MonoBehaviour
{
    private Transform leader;
    List<Agent> agents = new();
    [SerializeField] private float radius = 5f;
    [SerializeField, Range(-1f, 1f)] private float fovThreshold = 0f;

    [SerializeField] private float separationWeight = 1f;
    [SerializeField] private float cohesionWeight = 1.0f;
    [SerializeField] private float alignmentWeight = 1.0f;
    [SerializeField] private float lemmingWeight = 1.0f;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 5f;

    bool GroupInitialized;

    NavMeshPath fromHomeToResource;
    NavMeshPath fromResourceToHome;
    public Transform home;
    public Transform targetResource;

    // Start is called before the first frame update
    void Start()
    {
      //  agents = GetComponentsInChildren<Agent>();
        
       
    }

    public void CreatePaths()
    {
        Vector3 fromTO = targetResource.position - home.position;
        Vector2 normal = new Vector2(-fromTO.z, fromTO.x).normalized ;
        Vector3 offset = new Vector3(normal.x, 0, normal.y);
         NavMesh.CalculatePath(targetResource.position + offset, home.position + offset, NavMesh.AllAreas,fromResourceToHome);
        NavMesh.CalculatePath(home.position -   offset, targetResource.position - offset, NavMesh.AllAreas, fromHomeToResource);
    }
    public void InitializeGroup(Transform Leader)
    {
        agents.AddRange(GetComponentsInChildren<Agent>());
        leader = Leader;
        foreach (Agent i in agents)
        {
            i.InitializeBoid(separationWeight,cohesionWeight,alignmentWeight,lemmingWeight,moveSpeed,rotationSpeed);
            i.SetLeader(leader);
        }
        GroupInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GroupInitialized) return;
        if(leader == null)
        {
            leader = agents.First().transform;
        }
        foreach (Agent i in agents)
        {
            if (i.Enabled)
                i.CalculateMovement();
        }
        foreach (Agent i in agents)
        {
            if (i.Enabled)
                i.UpdateMovement();
        }
    }

    public void ChangeGroupLeader(Transform newLeader)
    {
        leader = newLeader;
        foreach(Agent i in agents)
        {
            i.SetLeader(leader);
        }
    }
}
