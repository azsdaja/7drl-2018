using Assets.Scripts.GameLogic.ActionLoop.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPresenter : MonoBehaviour
{
	public CanvasRenderer Panel;
	public Text DescriptionHeader;
	public TextMeshProUGUI Description;

	public void Present(ItemDefinition itemDefinition)
	{
		Panel.gameObject.SetActive(true);
		DescriptionHeader.text = itemDefinition.Name;
		Description.text = itemDefinition.Description;
	}
}
