using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPurchaseSO", menuName = "Scriptable Objects/Items/PurchaseItem")]
[Serializable]
public class PurchaseItem : Upgrade
{
    public GameObject _objectToPurchase;
    public override void PurchaseUpgrade()
    {
        base.PurchaseUpgrade();
        shop.SpawnObjectToPlace(_objectToPurchase);
    }
}
