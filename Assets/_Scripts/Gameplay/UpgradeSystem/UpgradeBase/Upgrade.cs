using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public abstract class Upgrade : ScriptableObject
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
    public List<Upgrade> upgradePrerequisites = new List<Upgrade>();
    public ResourceType resourceTypeRequired;
    public TheVault vault;

    public Action<bool> UpgradeUnlockStateChange;
    public Action<bool> UpgradeAffordanceStateChange;
    public Action FirstLevelBought;
    public void InitializeUpgrade(TheVault vault)
    {
        this.vault = vault;
        vault.resourceValueChanged[resourceTypeRequired] += EvaluateCost;
        foreach (Upgrade upgrade in upgradePrerequisites)
        {
            upgrade.FirstLevelBought += CheckIfPrerequisitesPurchased;
        }
        EvaluateCost(vault.GetResourceAmountOfType(resourceTypeRequired));

    }
    private void CheckIfPrerequisitesPurchased()
    {
        bool unlock = true;
        foreach (var upgrade in upgradePrerequisites)
        {
            if (upgrade.level < 1)
            {
                unlock = false;
                break;
            }
        }
        isUnlocked = unlock;
        EvaluateCost(vault.GetResourceAmountOfType(resourceTypeRequired));
        UpgradeUnlockStateChange?.Invoke(unlock);
    }
    public virtual void PurchaseUpgrade()
    {
        if (!canAfford || !isUnlocked) return;
        vault.ChangeResourceAmountOfType(resourceTypeRequired, -currentCost);
        level++;
        if(level == 1)
        {
            FirstLevelBought?.Invoke();
        }

        
    }

    private void EvaluateCost( float amount)
    {
        Debug.Log(resourceTypeRequired.ToString() + " was increased to " +  amount);
        if (!isUnlocked) return;
        canAfford = (amount >= currentCost);
        UpgradeAffordanceStateChange?.Invoke(canAfford);
    }

}
