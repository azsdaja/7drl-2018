using System;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class StrikeEffect : IActionEffect
	{
		private readonly Vector2Int _attackedActorLogicalPosition;
		private ActorBehaviour _actorBehaviour;
		private ActorBehaviour _attackedActorBehaviour;
		private ActorAligner _actorAligner = new ActorAligner();

		public StrikeEffect(ActorData actorData, ActorData attackedActor)
		{
			_attackedActorLogicalPosition = attackedActor.LogicalPosition;
			_actorBehaviour = actorData.Entity as ActorBehaviour;
			_attackedActorBehaviour = attackedActor.Entity as ActorBehaviour;
		}

		public void Process()
		{
			_actorAligner.AlignActorToDirection(_actorBehaviour.ActorData.Entity, _attackedActorLogicalPosition.x, 
				_actorBehaviour.ActorData.LogicalPosition.x);
			_actorBehaviour.WeaponAnimator.SwingTo(_attackedActorLogicalPosition);
			_attackedActorBehaviour.WeaponAnimator.DefendSwing(_actorBehaviour.WeaponAnimator.transform, _attackedActorLogicalPosition);
		}
	}
}