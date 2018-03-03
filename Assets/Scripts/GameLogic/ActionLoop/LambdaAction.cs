using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class LambdaAction : GameAction
	{
		private readonly Func<ActorData, IEnumerable<IActionEffect>> _inlineAction;

		public LambdaAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, 
			Func<ActorData, IEnumerable<IActionEffect>> inlineAction) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_inlineAction = inlineAction;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			return _inlineAction(ActorData);
		}
	}
}