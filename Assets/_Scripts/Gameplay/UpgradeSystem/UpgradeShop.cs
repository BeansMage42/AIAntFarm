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

    private GameObject placingObject;
    private bool isPlacingObject;
    [SerializeField] private GameObject mCam;
    [SerializeField] private LayerMask groundLayer;
    public int testDist;
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
            upgrade.InitializeUpgrade(vault,this);

            GameObject newUIElement = Instantiate(upgradeButtonUIObject, upgradeMenuUI.transform);
            UpgradeButton newButton = newUIElement.GetComponent<UpgradeButton>();
            newButton.OnInitializeButton(upgrade);
        }
        

    }

    public void PurchaseUpgrade(Upgrade upgrade)
    {

    }

    public void SpawnObjectToPlace(GameObject obj)
    {
        if (placingObject != null || isPlacingObject) return;
        placingObject = Instantiate(obj);
        isPlacingObject = true;

    }
    private void Update()
    {
        if (placingObject != null && isPlacingObject)
        {
            /*placingObject.transform.position*/
            /* Vector3 screenPos = Camera.main.ViewportToWorldPoint(Input.mousePosition);
             screenPos = new Vector3(screenPos.x, mCam.transform.position.y, screenPos.y);
             Debug.Log(screenPos);
             RaycastHit test;
             Debug.DrawLine(screenPos, screenPos + mCam.transform.forward * 50,Color.yellow,0.2f);
             if (Physics.Raycast(screenPos, mCam.transform.forward, out test))
             {
                 placingObject.transform.position = test.point;
             }
             else
             {

             }*/
            Vector2 mousePos = Input.mousePosition;
            Vector3 mouseToScreen = new Vector3(mousePos.x, mousePos.y, testDist /*Camera.main.nearClipPlane*/);
            Vector3 screenPos = Camera.main.ScreenToWorldPoint(mouseToScreen);
            Ray newRay = new Ray(screenPos, Vector3.down);
            //placingObject.transform.position = screenPos;
            //screenPos.z = 
            // screenPos = new Vector3(screenPos.x, mCam.transform.position.y, screenPos.y);
            //Debug.Log(screenPos);
            RaycastHit test;
            Debug.DrawRay(newRay.origin, newRay.GetPoint(50), Color.yellow, 0.2f);
            if (Physics.Raycast(newRay, out test, 20, groundLayer))
            {
                placingObject.transform.position = test.point;
            }
            else
            {

            }

        }
    }
}
