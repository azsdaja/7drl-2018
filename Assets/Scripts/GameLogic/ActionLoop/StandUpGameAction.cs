using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class StandUpGameAction : GameAction
	{
		public StandUpGameAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory) 
			: base(actorData, energyCost, actionEffectFactory)
		{
		
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			yield return new StandUpEffect(ActorData);
		}
	}
}