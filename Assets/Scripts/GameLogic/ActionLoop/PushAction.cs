using System;
using System.Collections.Generic;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class PushAction : GameAction
	{
		private readonly ActorData _targetEnemy;
		private readonly IRandomNumberGenerator _rng;
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IEntityDetector _entityDetector;

		public PushAction(ActorData actorData, ActorData targetEnemy, float energyCost, IActionEffectFactory actionEffectFactory, 
			IRandomNumberGenerator rng, IGridInfoProvider gridInfoProvider, IEntityDetector entityDetector) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_targetEnemy = targetEnemy;
			_rng = rng;
			_gridInfoProvider = gridInfoProvider;
			_entityDetector = entityDetector;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			float chanceToSucceed = ActorData.Accuracy;
			bool success = _rng.Check(chanceToSucceed);

			yield return new BumpEffect(ActorData, _targetEnemy.LogicalPosition);

			if (success)
			{
				_targetEnemy.Energy -= 1.0f;
				--ActorData.Swords;

				Vector2Int direction = _targetEnemy.LogicalPosition - ActorData.LogicalPosition;

				Assert.IsTrue(Vector2IntUtilities.IsOneStep(direction));

				Vector2Int previousPosition = _targetEnemy.LogicalPosition;
				_targetEnemy.LogicalPosition += direction;
				yield return new MoveEffect(_targetEnemy, previousPosition, _gridInfoProvider, _entityDetector);
			}
		}
	}
}