using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.GridRelated.TilemapAffecting;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class AscendAction : GameAction
	{
		private readonly IGameContext _gameContext;
		private readonly IEntityDetector _entityDetector;
		private readonly IEntityRemover _entityRemover;

		public AscendAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, IGameContext gameContext, 
			IEntityDetector entityDetector, IEntityRemover entityRemover) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_gameContext = gameContext;
			_entityDetector = entityDetector;
			_entityRemover = entityRemover;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			++_gameContext.CurrentDungeonIndex;
			Dungeon nextDungeon = _gameContext.Dungeons[_gameContext.CurrentDungeonIndex];
			BoundsInt furthestRoomToStairs = FurthestRoomToStairsResolver.GetFurthestRoomToStairs(nextDungeon);
			Vector2Int startingPosition = new Vector2Int((int) furthestRoomToStairs.center.x, (int) furthestRoomToStairs.center.y);
			_gameContext.PlayerActor.ActorData.LogicalPosition = startingPosition;
			IEnumerable<ActorData> actorAround = _entityDetector.DetectActors(startingPosition, 3);
			foreach (var actorData in actorAround)
			{
				_entityRemover.CleanSceneAndGameContextAfterDeath(actorData);
				Debug.Log("Removing an actor, because he was too close: " + actorData.ActorType);
			}
			Action action = () =>
			{
				_gameContext.PlayerActor.RefreshWorldPosition();
			};

			yield return new LambdaEffect(action);
		}
	}
}