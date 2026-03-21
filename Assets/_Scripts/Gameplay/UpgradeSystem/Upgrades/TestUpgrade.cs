using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/TestUpgrade")]
[Serializable]
public class TestUpgrade : Upgrade
{

    public override void PurchaseUpgrade()
    {
        base.PurchaseUpgrade();
        Debug.Log("upgrade purchased");
    }



}
