using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.Configuration;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.RNG;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class AttackAction : GameAction
	{
		private readonly ActorData _attackedActor;
		private readonly IRandomNumberGenerator _rng;
		private readonly IDeathHandler _deathHandler;
		private readonly IUiConfig _uiConfig;
		private readonly IGameContext _gameContext;
		private readonly bool _isDaringBlow;

		public AttackAction(ActorData actorData, ActorData attackedActor, float energyCost, IActionEffectFactory actionEffectFactory, 
			IRandomNumberGenerator rng, IDeathHandler deathHandler, bool isDaringBlow, IUiConfig uiConfig, IGameContext gameContext) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_attackedActor = attackedActor;
			_rng = rng;
			_deathHandler = deathHandler;
			_isDaringBlow = isDaringBlow;
			_uiConfig = uiConfig;
			_gameContext = gameContext;
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

				if (_attackedActor.ActorType == ActorType.Basher && ActorData.ActorType == ActorType.Player && _attackedActor.HealthProgress < 0.5f)
				{
					_uiConfig.BasherMessage.gameObject.SetActive(true);
					_uiConfig.BasherMessage.GetComponent<RectTransform>().sizeDelta = new Vector3(560, 180);
					_uiConfig.BasherMessage.SetMessage("Ouch! Enough for me! That was a satisfactory duel. My reputation is clean now. Thank you! Now, let's go!");
					_gameContext.BasherSteps = 2;
					_attackedActor.Team = Team.Beasts;
					ActorData.HasFinishedDuel = true;
					ActorData.HasWonDuel = true;
				}
				if (_attackedActor.ActorType == ActorType.Player && ActorData.ActorType == ActorType.Basher && _attackedActor.HealthProgress < 0.5f)
				{
					_uiConfig.BasherMessage.gameObject.SetActive(true);
					_uiConfig.BasherMessage.GetComponent<RectTransform>().sizeDelta = new Vector3(560, 180);
					_uiConfig.BasherMessage.SetMessage("Ha! I think you've had enough. That was a good duel! My reputation is clean now. Thank you! Now, let's go!");
					_gameContext.BasherSteps = 2;
					ActorData.Team = Team.Beasts;
					_attackedActor.HasFinishedDuel = true;
					_attackedActor.HasWonDuel = false;
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