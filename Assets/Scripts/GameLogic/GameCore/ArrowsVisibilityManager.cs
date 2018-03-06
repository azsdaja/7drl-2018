using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class ArrowsVisibilityManager : IArrowsVisibilityManager
	{
		private readonly IUiConfig _uiConfig;

		public ArrowsVisibilityManager(IUiConfig uiConfig, IGameContext gameContext)
		{
			_uiConfig = uiConfig;
		}

		public void Show()
		{
			_uiConfig.Arrows.gameObject.SetActive(true);
		}

		public void Hide()
		{
			_uiConfig.Arrows.gameObject.SetActive(false);
		}
	}
}