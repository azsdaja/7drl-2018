using System;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public interface IActionEffectFactory
	{
		IActionEffect CreateMoveEffect(ActorData activeActor, Vector2Int activeActorPositionBefore);
		// todo: accepting ANY lambda looks too much permissive. Consider limiting it, for example to Action<EntityData> without closures.
		IActionEffect CreateLambdaEffect(Action action);
		IActionEffect CreateBumpEffect(ActorData actorData, Vector2Int newPosition);
		IActionEffect CreateKnockoutEffect(ActorData actorData);
		IActionEffect CreateStrikeEffect(ActorData actorData, ActorData attackedActor);
	}
}