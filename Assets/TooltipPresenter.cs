using Assets.Scripts.GameLogic.ActionLoop.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPresenter : MonoBehaviour
{
	public CanvasRenderer Panel;
	public Text DescriptionHeader;
	public TextMeshProUGUI Description;
	public Text Footer;

	public void Present(ItemDefinition itemDefinition, bool isInInventory)
	{
		gameObject.SetActive(true);
		Panel.gameObject.SetActive(true);
		DescriptionHeader.text = isInInventory ? itemDefinition.Name : itemDefinition.Name + " at your feet";
		Description.text = itemDefinition.GetDescription();
		Footer.text = isInInventory ? "Press 'e' to use, \r\n'd' to drop." : "Press 'g' to pick up.";
	}
}
