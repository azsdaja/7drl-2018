using Assets.Scripts.GameLogic.Animation;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public interface IGameEntity
	{
		IEntityAnimator EntityAnimator { get; }
		EntityData EntityData { get; }
		Vector3 Position { get; set; }
		void Show();
		void Hide();
		bool IsVisible { get; }
	}
}