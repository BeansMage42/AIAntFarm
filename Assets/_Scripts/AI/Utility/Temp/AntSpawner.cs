using System;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{

    [SerializeField] private GameObject AntToSpawn;
    [SerializeField] private GameObject GroupPrefab;
    ObjectPool groupPool;
    ObjectPool AntPool;

    [SerializeField] private int AntsToSpawnInGroup;
    [SerializeField] private float spawnRadius;
    public Action AllAntsASpawned;


    private void Start()
    {
        groupPool = new ObjectPool(GroupPrefab,5);
        AntPool = new ObjectPool(AntToSpawn,AntsToSpawnInGroup);

    }
    public void SpawnAntsInNewGroupWithLeader(Transform leader)
    {
        GameObject newGroup = groupPool.Get();
        
        for (int i = 0; i < AntsToSpawnInGroup; i++)
        {
            GameObject ant = AntPool.Get();//.transform.parent = newGroup.transform;
            Vector3 random = UnityEngine.Random.insideUnitCircle * spawnRadius;
            random.y = 8f;
            ant.transform.position = random;
            ant.transform.parent = newGroup.transform;
        }
        newGroup.GetComponent<Group>().InitializeGroup(leader);
        AllAntsASpawned?.Invoke();
    }
}
