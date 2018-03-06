using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.RNG;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class AttackAction : GameAction
	{
		private readonly ActorData _attackedActor;
		private readonly IRandomNumberGenerator _rng;
		private readonly IDeathHandler _deathHandler;

		public AttackAction(ActorData actorData, ActorData attackedActor, float energyCost, IActionEffectFactory actionEffectFactory, 
			IRandomNumberGenerator rng, IDeathHandler deathHandler) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_attackedActor = attackedActor;
			_rng = rng;
			_deathHandler = deathHandler;
		}

		internal ActorData AttackedActor
		{
			get { return _attackedActor; }
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			bool hit = _rng.Check(0.667f);

			if (_attackedActor.Swords == 0 && hit)
			{
				if (_attackedActor.Swords == 0)
				{
					_attackedActor.Health -= ActorData.Weapon.MaxDamage;
				}
				if (_attackedActor.Health <= 0)
				{
					_deathHandler.HandleDeath(_attackedActor);
				}

				yield return new LambdaEffect(() =>
				{
					Animator blood = Resources.Load<Animator>("Prefabs/Blood");
					Animator bloodObject = GameObject.Instantiate(blood, AttackedActor.Entity.transform.position, Quaternion.identity);
					bloodObject.Play("Blood");
					GameObject.Destroy(bloodObject.gameObject, .4f);
				});
			}

			if (_attackedActor.Swords > 0 && hit)
			{
				--_attackedActor.Swords;
			}
			
			IActionEffect strikeEffect = ActionEffectFactory.CreateStrikeEffect(ActorData, AttackedActor);
			yield return strikeEffect;

			AttackedActor.BlockedUntil = DateTime.UtcNow + TimeSpan.FromMilliseconds(300);
		}
	}
}