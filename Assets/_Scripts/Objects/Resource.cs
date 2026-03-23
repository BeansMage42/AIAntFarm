using System;
using UnityEngine;
[Serializable]
public class Resource : QuadTreeObject
{

    public ResourceType resourceType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float resourceRadius;
/*
    public override void OnEnable()
    {
        bounds = GetComponent<Collider>().bounds;
        resourceRadius = bounds.
        QuadTreeManager.Instance.AddObjectToTree(this);
        //base.OnEnable();
    }*/

    

}
