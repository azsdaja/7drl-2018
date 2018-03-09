using System;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class StrikeEffect : IActionEffect
	{
		private readonly bool _parried;
		private readonly bool _isDaringBlow;
		private readonly Vector2Int _attackedActorLogicalPosition;
		private readonly ActorBehaviour _actorBehaviour;
		private readonly ActorBehaviour _attackedActorBehaviour;
		private readonly ActorAligner _actorAligner = new ActorAligner();
		private readonly IWeaponColorizer _weaponColorizer;

		public StrikeEffect(ActorData actorData, ActorData attackedActor, bool parried, bool isDaringBlow, IWeaponColorizer weaponColorizer)
		{
			_parried = parried;
			_isDaringBlow = isDaringBlow;
			_weaponColorizer = weaponColorizer;
			_attackedActorLogicalPosition = attackedActor.LogicalPosition;
			_actorBehaviour = actorData.Entity as ActorBehaviour;
			_attackedActorBehaviour = attackedActor.Entity as ActorBehaviour;
		}

		public void Process()
		{
			_actorAligner.AlignActorToDirection(_actorBehaviour.ActorData.Entity, _attackedActorLogicalPosition.x -
				_actorBehaviour.ActorData.LogicalPosition.x);
			if (_isDaringBlow)
			{
				_weaponColorizer.Colorize(_actorBehaviour.WeaponAnimator, Color.red);
			}
			_actorBehaviour.WeaponAnimator.SwingTo(_attackedActorLogicalPosition, _isDaringBlow);
			if (_parried)
			{
				_attackedActorBehaviour.WeaponAnimator.DefendSwing(_actorBehaviour.WeaponAnimator.transform, _attackedActorLogicalPosition);
			}
		}
	}
}