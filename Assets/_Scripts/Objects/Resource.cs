using System;
using UnityEngine;
[Serializable]
public class Resource : QuadTreeObject
{

    public ResourceType resourceType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float resourceRadius;
    public float _floatAmountAvailable;
    public Action<Resource> OnDepleteResource;
    public AntBase occupied;
    private void Update()
    {
       //. if (Input.GetKeyDown(KeyCode.Space)) ExtractResource(10f, out float temp);
    }
    public void ExtractResource(float amountToExtract, out float amountExtracted)
    {
        if (amountToExtract <= _floatAmountAvailable)
        {
            _floatAmountAvailable -= amountToExtract;
            amountExtracted = amountToExtract;
            if (_floatAmountAvailable == 0)
            {
                OnDepleteResource?.Invoke(this);
                DisableObject();
            }
            //return amountToExtract;
        }
        else
        {
            float temp = _floatAmountAvailable;
            _floatAmountAvailable = 0;
            amountExtracted = temp;
            OnDepleteResource?.Invoke(this);
            DisableObject();
            //return temp;

        }

    }

}
