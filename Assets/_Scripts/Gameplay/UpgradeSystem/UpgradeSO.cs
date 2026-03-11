using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{

    [SerializeField] public string upgradeName;
    [SerializeField] public string description;
    [SerializeField] public float baseCost;
    [SerializeField] public float costIncreaseMod;
    [SerializeField] public float amountIncrease;
    [SerializeField] public ResourceType resourceTypeRequired;
    [SerializeField] public UpgradeSO[] prerequisites;
    [SerializeField] public bool defaultUnlocked = false;

    
}
