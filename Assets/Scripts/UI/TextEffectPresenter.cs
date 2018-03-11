using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.UnityUtilities;
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

		public void ShowTextEffect(Vector2Int position, string text)
		{
			if (!_gameContext.VisiblePositions.Contains(position))
			{
				return;
			}
			var textEffectObject = new GameObject("TextEffect");
			textEffectObject.transform.position = _gridInfoProvider.GetCellCenterWorld(position);
			var textEffect = textEffectObject.AddComponent<TextEffect>();
			textEffect.Initialize(text);

			float time = Mathf.Min(2f, (float)text.Length / 15);
			Object.Destroy(textEffectObject, 2.0f);
		}
	}
}
