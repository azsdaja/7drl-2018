using System;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class ActionEffectFactory : IActionEffectFactory
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private IEntityDetector _entityDetector;

		public ActionEffectFactory(IGridInfoProvider gridInfoProvider, IEntityDetector entityDetector)
		{
			_gridInfoProvider = gridInfoProvider;
			_entityDetector = entityDetector;
		}

		public IActionEffect CreateMoveEffect(ActorData activeActor, Vector2Int activeActorPositionBefore)
		{
			return new MoveEffect(activeActor, activeActorPositionBefore, _gridInfoProvider, _entityDetector);
		}

		public IActionEffect CreateLambdaEffect(Action action)
		{
			return new LambdaEffect(action);
		}

		public IActionEffect CreateBumpEffect(ActorData actorData, Vector2Int newPosition)
		{
			return new BumpEffect(actorData, newPosition);
		}

		public IActionEffect CreateKnockoutEffect(ActorData actorData)
		{
			return new KnockOutEffect(actorData);
		}

		public IActionEffect CreateStrikeEffect(ActorData actorData, ActorData attackedActor, bool parried)
		{
			return new StrikeEffect(actorData, attackedActor, parried);
		}
	}
}