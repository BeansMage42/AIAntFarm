using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public Queue<GameObject> InactivePool;
    public List<GameObject> ActivePool;
    public GameObject ObjectToPool;
    public int maxPoolSize;

    public ObjectPool(GameObject objectToPool, int poolSize)
    {
        ObjectToPool = objectToPool;
        maxPoolSize = poolSize;
        InactivePool = new Queue<GameObject>();
        ActivePool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewForPool();
        }

    }

    public GameObject Get()
    {
        if(InactivePool.Count <= 0)
        {
            CreateNewForPool();
        }

        GameObject grabbedObj = InactivePool.Dequeue();
        ActivePool.Add(grabbedObj);
        grabbedObj.SetActive(true);
        return grabbedObj;

    }
    private GameObject CreateNewForPool()
    {
        GameObject newObj = Object.Instantiate(ObjectToPool);
        newObj.GetComponent<Poolable>().Pool = this;
        newObj.SetActive(false);
        InactivePool.Enqueue(newObj);
        return newObj;
    }
    public void ReturnToPool(GameObject obj) 
    {
        if (!ActivePool.Contains(obj)) return;
        if (InactivePool.Contains(obj)) return;

        ActivePool.Remove(obj);
        obj.SetActive(false);
        InactivePool.Enqueue(obj);
        
    }

}
