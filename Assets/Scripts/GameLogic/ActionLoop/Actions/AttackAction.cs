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
		private readonly bool _isDaringBlow;

		public AttackAction(ActorData actorData, ActorData attackedActor, float energyCost, IActionEffectFactory actionEffectFactory, 
			IRandomNumberGenerator rng, IDeathHandler deathHandler, bool isDaringBlow) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_attackedActor = attackedActor;
			_rng = rng;
			_deathHandler = deathHandler;
			_isDaringBlow = isDaringBlow;
		}

		internal ActorData AttackedActor
		{
			get { return _attackedActor; }
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			float chanceToDealAccurateBlow = ActorData.Accuracy;
			bool accurate = _rng.Check(chanceToDealAccurateBlow);

			if (_isDaringBlow)
			{
				ActorData.Swords -= 2;
			}

			bool hit = (_isDaringBlow || _attackedActor.Swords <= 0) && accurate;
			if (hit)
			{
				int damage = Math.Max(_rng.Next(ActorData.WeaponWeld.WeaponDefinition.MaxDamage + 1), _rng.Next(ActorData.WeaponWeld.WeaponDefinition.MaxDamage + 1));
				_attackedActor.Health -= damage;
				if (_attackedActor.Health <= 0)
				{
					_deathHandler.HandleDeath(_attackedActor);
					ActorData.Xp += _attackedActor.XpGiven;
				}

				yield return new LambdaEffect(() =>
				{
					Animator blood = Resources.Load<Animator>("Prefabs/Blood");
					Animator bloodObject = GameObject.Instantiate(blood, AttackedActor.Entity.transform.position, Quaternion.identity);
					bloodObject.Play("Blood");
					GameObject.Destroy(bloodObject.gameObject, .4f);
				});
			}

			if (_attackedActor.Swords > 0 && accurate)
			{
				--_attackedActor.Swords;
			}

			if (ActorData.WeaponWeld.WeaponDefinition.IsBodyPart)
			{
				IActionEffect bumpEffect = ActionEffectFactory.CreateBumpEffect(ActorData, AttackedActor.LogicalPosition);
				yield return bumpEffect;
			}
			else
			{
				IActionEffect strikeEffect = ActionEffectFactory.CreateStrikeEffect(ActorData, AttackedActor, !hit, _isDaringBlow);
				yield return strikeEffect;
			}
			

			AttackedActor.BlockedUntil = DateTime.UtcNow + TimeSpan.FromMilliseconds(300);
			ActorData.BlockedUntil = DateTime.UtcNow + TimeSpan.FromMilliseconds(300);
		}
	}
}