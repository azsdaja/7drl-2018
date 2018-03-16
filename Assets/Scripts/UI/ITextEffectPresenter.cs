using UnityEngine;

namespace Assets.Scripts.UI
{
	public interface ITextEffectPresenter
	{
		void ShowTextEffect(Vector2Int position, string text, Color? color = null, bool forceShowing = false);
	}
}