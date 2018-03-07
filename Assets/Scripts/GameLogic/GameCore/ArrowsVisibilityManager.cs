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

		public void Show(Color color)
		{
			_uiConfig.Arrows.gameObject.SetActive(true);
			var arrowRenderers = _uiConfig.Arrows.transform.GetComponentsInChildren<SpriteRenderer>();
			foreach (var spriteRenderer in arrowRenderers)
			{
				spriteRenderer.color = color;
			}
		}

		public void Hide()
		{
			_uiConfig.Arrows.gameObject.SetActive(false);
		}
	}
}