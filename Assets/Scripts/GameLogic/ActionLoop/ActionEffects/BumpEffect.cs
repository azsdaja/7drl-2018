using Assets.Scripts.GameLogic.Animation;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.ActionEffects
{
	public class BumpEffect : IActionEffect
	{
		public BumpEffect(ActorData actorData, Vector2Int bumpedPosition)
		{
			ActorData = actorData;
			BumpedPosition = bumpedPosition;
		}

		public ActorData ActorData { get; private set; }

		public Vector2Int BumpedPosition { get; private set; }

		public virtual void Process()
		{
			IGameEntity entity = ActorData.Entity;
			IEntityAnimator entityAnimator = entity.EntityAnimator;
			bool actorIsVisible = entity.IsVisible;
			if (actorIsVisible)
				entityAnimator.Bump(ActorData.LogicalPosition, BumpedPosition);
		}
	}
}