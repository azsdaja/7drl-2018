using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class ReleaseAction : GameAction
	{
		public ReleaseAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory)
			: base(actorData, energyCost, actionEffectFactory)
		{
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			ActorData actorToRelease = ActorData.CaughtActor;
			ActorData.CaughtActor = null;
			actorToRelease.Energy = -0.5f;
			actorToRelease.LogicalPosition = ActorData.LogicalPosition + Vector2Int.right;

			ActorData.EnergyGain *= 2;

			yield return new LambdaEffect(() =>
			{
				actorToRelease.Entity.transform.parent = ActorData.Entity.transform.parent;
				actorToRelease.Entity.EntityAnimator.FallOut(ActorData.LogicalPosition, actorToRelease.LogicalPosition);
			});
		}
	}
}