using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class CatchAction : GameAction
	{
		private readonly ActorData _caughtActor;

		public CatchAction(ActorData actorData, float energyCost, ActorData caughtActor, IActionEffectFactory actionEffectFactory) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_caughtActor = caughtActor;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			ActorData.CaughtActor = _caughtActor;
			_caughtActor.Energy = -int.MaxValue;
			ActorData.EnergyGain *= 0.5f;

			yield return new CatchEffect(_caughtActor.Entity, ActorData.Entity);
		}
	}
}