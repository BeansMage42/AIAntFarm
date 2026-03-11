using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public class Upgrade
{
    public string upgradeName;
    public string description;
    public float baseCost;
    public float costIncreaseMod;
    public float amountIncrease;
    public float currentCost;
    public bool isUnlocked;
    public bool canAfford;
    public int level = 0;
    public List<string> upgradePrerequisites = new List<string>();
    public ResourceType resourceTypeRequired;
    public TheVault vault;

    public Upgrade(TheVault vault, ResourceType type)
    {
        this.vault = vault;
        resourceTypeRequired = type;
        vault.resourceValueChanged[type] += EvaluateCost; 
    }
    public void PurchaseUpgrade()
    {
    }

    private void EvaluateCost( float amount)
    {

    }

}
