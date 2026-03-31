using System;
using UnityEngine;
using UnityEngine.AI;

public class AntSpawner : MonoBehaviour
{

    [SerializeField] private GameObject AntToSpawn;
    [SerializeField] private GameObject seekerAnt;
    [SerializeField] private GameObject GroupPrefab;
    ObjectPool groupPool;
    ObjectPool collectorPool;
    ObjectPool seekerPool;

    [SerializeField] private int AntsToSpawnInGroup;
    [SerializeField] private float spawnRadius;
    public Action AllAntsASpawned;


    private void Start()
    {
        groupPool = new ObjectPool(GroupPrefab,5);
        collectorPool = new ObjectPool(AntToSpawn,AntsToSpawnInGroup);
        seekerPool = new ObjectPool(seekerAnt, 5);
    }
    public void SpawnAntsInNewGroupWithLeader(Transform leader)
    {
        GameObject newGroup = groupPool.Get();
        
        for (int i = 0; i < AntsToSpawnInGroup; i++)
        {
            GameObject ant = collectorPool.Get();//.transform.parent = newGroup.transform;
            Vector3 random =  (UnityEngine.Random.insideUnitCircle * spawnRadius);
            random += GameManager.instance.Home.transform.position;
            random.y = 8f;
           // ant.transform.position = GameManager.instance.Home.transform.position + random;
            if (NavMesh.SamplePosition(random, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
            {
                random = hit.position;
            }
            ant.transform.position = random;
            ant.transform.parent = newGroup.transform;
            ant.GetComponent<CollectorAnt>().currentGroup = newGroup.GetComponent<Group>();
        }
        newGroup.GetComponent<Group>().InitializeGroup(leader);
        AllAntsASpawned?.Invoke();
    }

    public void SpawnSeeker(ResourceType type)
    {
        GameObject newSeeker = seekerPool.Get();
        newSeeker.transform.position = GameManager.instance.Home.transform.position;
        newSeeker.GetComponent<SeekerAnt>().InitializeAnt(type,this);
    }
}
