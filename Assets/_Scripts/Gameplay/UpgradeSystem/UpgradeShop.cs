using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class UpgradeShop : MonoBehaviour
{

    public UpgradeSO[] upgradesSO;
    public List<Upgrade> upgrades = new List<Upgrade>();
    public TheVault vault;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (UpgradeSO upgrade in upgradesSO)
        {
            Upgrade newUpgrade = new Upgrade(vault,upgrade.resourceTypeRequired);
            newUpgrade.currentCost = upgrade.baseCost;
            newUpgrade.baseCost = upgrade.baseCost;
            newUpgrade.upgradeName = upgrade.upgradeName;
            newUpgrade.isUnlocked = upgrade.defaultUnlocked;
            newUpgrade.costIncreaseMod = upgrade.costIncreaseMod;
            newUpgrade.amountIncrease = upgrade.amountIncrease;
            newUpgrade.description = upgrade.description;
            foreach (UpgradeSO prereq in upgrade.prerequisites)
            {
                newUpgrade.upgradePrerequisites.Add(prereq.name);
            }
            upgrades.Add(newUpgrade);

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
