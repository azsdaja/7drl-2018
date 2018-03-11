using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class MoveAction : DirectedAction
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IEntityDetector _entityDetector;

		public MoveAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, 
			Vector2Int direction, IGridInfoProvider gridInfoProvider, IEntityDetector entityDetector) 
			: base(actorData, energyCost, actionEffectFactory, direction)
		{
			GuardDirection(direction);
			_gridInfoProvider = gridInfoProvider;
			_entityDetector = entityDetector;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			Vector2Int previousPosition = ActorData.LogicalPosition;
			Vector2Int newPosition = previousPosition + Direction;
			if (_gridInfoProvider.IsWalkable(newPosition))
			{
				IActionEffect effect = ActionEffectFactory.CreateMoveEffect(ActorData, previousPosition);
				ActorData.LogicalPosition = newPosition;
				if (ActorData.CaughtActor != null)
				{
					ActorData.CaughtActor.LogicalPosition = newPosition;
				}

				yield return effect;
			}
			else
			{
				IActionEffect effect = ActionEffectFactory.CreateBumpEffect(ActorData, newPosition);
				yield return effect;
			}
		}

		private void GuardDirection(Vector2Int direction)
		{
			if(direction.x > 1 || direction.x < -1 || direction.y > 1 || direction.y < -1)
				throw new ArgumentException("Direction to move is exceeding one step: " + direction, "direction");
		}
	}
}
