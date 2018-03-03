using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.RNG;

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

			int maxDamage = ActorData.MaxDamage;
			int damage = _rng.Next(1, maxDamage + 1);
			bool knockout = damage == maxDamage || damage >= (.9f * maxDamage);


			if (AttackedActor.Health <= 0)
			{
				_deathHandler.HandleDeath(_attackedActor);
			}
			if (knockout)
			{
				AttackedActor.Energy = -5f;
				IActionEffect knockoutEffect = ActionEffectFactory.CreateKnockoutEffect(AttackedActor);
				yield return knockoutEffect;
			}
			AttackedActor.Health -= damage;
			IActionEffect bumpEffect = ActionEffectFactory.CreateBumpEffect(ActorData, AttackedActor.LogicalPosition);
			yield return bumpEffect;
		}
	}
}