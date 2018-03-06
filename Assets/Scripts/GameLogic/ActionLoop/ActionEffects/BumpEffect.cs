using Assets.Scripts.GameLogic.Animation;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.ActionEffects
{
	public class BumpEffect : IActionEffect
	{
		private readonly ActorAligner _actorAligner;

		public BumpEffect(ActorData actorData, Vector2Int bumpedPosition)
		{
			ActorData = actorData;
			BumpedPosition = bumpedPosition;
			_actorAligner = new ActorAligner();
		}

		public ActorData ActorData { get; private set; }

		public Vector2Int BumpedPosition { get; private set; }

		public virtual void Process()
		{
			IGameEntity entity = ActorData.Entity;
			IEntityAnimator entityAnimator = entity.EntityAnimator;
			bool actorIsVisible = entity.IsVisible;
			_actorAligner.AlignActorToDirection(ActorData.Entity, BumpedPosition.x - ActorData.LogicalPosition.x);
			if (actorIsVisible)
				entityAnimator.Bump(ActorData.LogicalPosition, BumpedPosition);
		}
	}
}