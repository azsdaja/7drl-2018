using Assets.Scripts.GameLogic.GameCore;
using TMPro;
using UnityEngine;
using Zenject;

public class Tooltippable : MonoBehaviour
{
	private IGameConfig _gameConfig;
	private IUiConfig _uiConfig;

	public string Description;

	[Inject]
	public void Init(IGameConfig gameConfig, IUiConfig uiConfig)
	{
		_gameConfig = gameConfig;
		_uiConfig = uiConfig;
	}

	void OnMouseEnter()
	{
		if (!_gameConfig.ModeConfig.ShowActorTooltip) return;

		Canvas tooltipPooled = _uiConfig.TooltipPooled;
		tooltipPooled.gameObject.SetActive(true);
		tooltipPooled.transform.position = transform.position;
		tooltipPooled.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = Description;
	}

	void OnMouseExit()
	{
		_uiConfig.TooltipPooled.gameObject.SetActive(false);
	}
}
