using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public abstract class DirectedAction : GameAction
	{
		public Vector2Int Direction { get; protected set; }

		protected DirectedAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, Vector2Int direction) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			Direction = direction;
		}
	}
}
