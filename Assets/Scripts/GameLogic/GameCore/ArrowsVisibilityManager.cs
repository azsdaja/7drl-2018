using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class ArrowsVisibilityManager : IArrowsVisibilityManager
	{
		private readonly IUiConfig _uiConfig;
		private readonly IGameContext _gameContext;

		public ArrowsVisibilityManager(IUiConfig uiConfig, IGameContext gameContext)
		{
			_uiConfig = uiConfig;
			_gameContext = gameContext;
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