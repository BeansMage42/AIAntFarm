using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
public class TheVault: MonoBehaviour 
{
    public InputAction temp;

    private Dictionary<ResourceType,float> resourcesStored = new Dictionary<ResourceType,float>();
    public Dictionary<ResourceType, Action<float>> resourceValueChanged = new Dictionary<ResourceType,Action<float>>();

    private void Start()
    {
        foreach(ResourceStarter kvp in GameManager.instance.resources)
        {
            AddResourceType(kvp.type, kvp.startingAmount);
        }
    }
   
    public void AddResourceType(ResourceType key, float value)
    {
        if (resourcesStored.ContainsKey(key)) return;
        resourcesStored.Add(key, value);
    }
    public Dictionary<ResourceType, float> GetResources()
    {
        return resourcesStored;
    }
    public void ChangeResourceAmountOfType(ResourceType key, float increaseAmount)
    {
        if(!resourcesStored.ContainsKey(key)) return;
        resourcesStored[key] += increaseAmount;
        resourceValueChanged[key]?.Invoke(resourcesStored[key]);
        //resourceValueChanged?.Invoke(key, resourcesStored[key]);
    }
    public void SetResourceOfTypeToAmount(ResourceType key, float setAmount)
    {
        if (!resourcesStored.ContainsKey(key)) return;
        resourcesStored[key] = setAmount;
        resourceValueChanged[key]?.Invoke(resourcesStored[key]);

    }
    public float GetResourceAmountOfType(ResourceType key)
    {
        if(!resourcesStored.ContainsKey(key))return 0f;
        else return resourcesStored[key];
    }

}
