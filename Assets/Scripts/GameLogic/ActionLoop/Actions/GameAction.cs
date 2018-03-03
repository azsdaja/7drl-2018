using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public abstract class GameAction : IGameAction
	{
		protected readonly IActionEffectFactory ActionEffectFactory;

		public ActorData ActorData { get; private set; }
		public float EnergyCost { get; private set; }

		protected GameAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory)
		{
			ActorData = actorData;
			EnergyCost = energyCost;
			ActionEffectFactory = actionEffectFactory;
		}

		public abstract IEnumerable<IActionEffect> Execute();
	}
}