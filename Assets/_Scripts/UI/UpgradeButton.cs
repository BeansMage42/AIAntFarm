using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI upgradetext;
    private Upgrade upgrade;

    public void OnInitializeButton(Upgrade newUpgrade)
    {
        upgrade = newUpgrade;
        purchaseButton.onClick.AddListener(upgrade.PurchaseUpgrade);
        upgrade.UpgradeAffordanceStateChange += ChangeButtonAffordState;
        upgrade.UpgradeUnlockStateChange += ChangeButtonUnlockState;
        UpdateButtonText();
        ChangeButtonUnlockState(upgrade.isUnlocked);

        ChangeButtonAffordState(upgrade.canAfford);
    }
    public void UpdateButtonText()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(upgrade.upgradeName);
        sb.AppendLine(upgrade.description);
        sb.AppendLine("Level: ");
        sb.Append(upgrade.level);
        sb.AppendLine("Cost: ");
        sb.Append(upgrade.currentCost);
        sb.Append(" ");
        sb.Append(upgrade.resourceTypeRequired.ToString());

        upgradetext.text = sb.ToString();

    }
    public void ChangeButtonUnlockState(bool state)
    {
        purchaseButton.gameObject.SetActive(state);
    }
    public void ChangeButtonAffordState(bool state)
    {
        purchaseButton.interactable = state;
    }
    

}
