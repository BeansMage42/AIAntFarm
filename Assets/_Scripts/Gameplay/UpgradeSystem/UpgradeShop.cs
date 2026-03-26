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

    //SPAWNING

    public Dictionary<GameObject,ObjectPool> placeAbleResourcePool = new();

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
        if (!placeAbleResourcePool.ContainsKey(obj)) 
        {
            placeAbleResourcePool.Add(obj, new ObjectPool(obj, 20));
        }
        placingObject = placeAbleResourcePool[obj].Get();
        isPlacingObject = true;

    }
    private void Update()
    {
        if (placingObject != null && isPlacingObject)
        {
            Vector2 mousePos = Input.mousePosition;
            Ray screenPos = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit test;
            Debug.DrawRay(screenPos.origin, screenPos.origin + screenPos.direction *10, Color.yellow, 0.2f);
            if (Physics.Raycast(screenPos, out test, 20, groundLayer))
            {
                placingObject.transform.position = test.point;
            }
            else
            {
                Plane plane = new Plane(Vector3.up,Vector3.zero + new Vector3(0,8,0));

                float distance;
                if(plane.Raycast(screenPos, out distance)){
                    Vector3 point = screenPos.GetPoint(distance);
                    placingObject.transform.position = point;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                placingObject.GetComponent<QuadTreeObject>().OnPlace();
                placingObject = null;
                isPlacingObject = false;
            }

        }
    }
}
