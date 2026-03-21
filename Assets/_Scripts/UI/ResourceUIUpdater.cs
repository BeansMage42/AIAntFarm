using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ResourceUIUpdater : MonoBehaviour
{

    [SerializeField] private ResourceType resourceToMonitor;
    [SerializeField] private bool unlocked;
    [SerializeField] private TextMeshProUGUI textToEdit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TheVault.Instance.resourceValueChanged[resourceToMonitor] += UpdateText;
        UpdateText(TheVault.Instance.GetResourceAmountOfType(resourceToMonitor));
    }

    public void UpdateText(float amount)
    {
        if (amount > 0 && !unlocked) 
        {
            unlocked = true;
        }
        textToEdit.text = amount.ToString();
    }
}
