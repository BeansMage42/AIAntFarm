using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
public class UpgradeShop : MonoBehaviour
{

    //public UpgradeSO[] upgradesSO;
    //public Dictionary<string,Upgrade> upgrades = new Dictionary<string, Upgrade>();
    // public List<Upgrade> Upgrades = new List<Upgrade>();
   // [SerializeReference]
    public List<Upgrade> Upgrades = new List<Upgrade>();

    private List<Upgrade> RuntimeUpgradeList = new();
    private Dictionary<Upgrade,Upgrade> RuntimeLookUp = new();
    private TheVault vault;
    public Action<string> UpgradePurchased;
    [SerializeField] private GameObject upgradeButtonUIObject;
    [SerializeField] private GameObject upgradeMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vault = TheVault.Instance;
        foreach (var upgrade in Upgrades)
        {
            var clone = Instantiate(upgrade);
            RuntimeUpgradeList.Add(clone);
            //upgrade.InitializeUpgrade(vault);
            RuntimeLookUp.Add(upgrade,clone);

        }

        foreach (var upgrade in RuntimeUpgradeList)
        {
            List<Upgrade> remapped = new List<Upgrade>();
            foreach (var prerequisit in upgrade.upgradePrerequisites)
            {
                if (RuntimeLookUp.TryGetValue(prerequisit, out Upgrade runtime))
                {
                    remapped.Add(runtime);
                }
            }
            upgrade.upgradePrerequisites = remapped;
            upgrade.InitializeUpgrade(vault);

            GameObject newUIElement = Instantiate(upgradeButtonUIObject, upgradeMenuUI.transform);
            UpgradeButton newButton = newUIElement.GetComponent<UpgradeButton>();
            newButton.OnInitializeButton(upgrade);
        }
        

    }

    public void PurchaseUpgrade(Upgrade upgrade)
    {

    }
}
