using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.UnityUtilities;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class TextEffectPresenter : ITextEffectPresenter
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IGameContext _gameContext;

		public TextEffectPresenter(IGridInfoProvider gridInfoProvider, IGameContext gameContext)
		{
			_gridInfoProvider = gridInfoProvider;
			_gameContext = gameContext;
		}

		public void ShowTextEffect(Vector2Int position, string text, Color? color = null, bool forceShowing = false)
		{
			if (!color.HasValue) color = Color.white;
			if (!forceShowing && !_gameContext.VisiblePositions.Contains(position))
			{
				return;
			}
			var textEffectObject = new GameObject("TextEffect");
			textEffectObject.transform.position = _gridInfoProvider.GetCellCenterWorld(position);
			var textEffect = textEffectObject.AddComponent<TextEffect>();
			float duration = Mathf.Max(2.5f, 1f + (float)text.Length / 5);

			textEffect.Initialize(text, color.Value, duration);

			Object.Destroy(textEffectObject, duration);
		}
	}
}
