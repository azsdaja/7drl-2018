using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated.TilemapAffecting;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class AscendAction : GameAction
	{
		private readonly IGameContext _gameContext;

		public AscendAction(ActorData actorData, float energyCost, IActionEffectFactory actionEffectFactory, IGameContext gameContext) 
			: base(actorData, energyCost, actionEffectFactory)
		{
			_gameContext = gameContext;
		}

		public override IEnumerable<IActionEffect> Execute()
		{
			++_gameContext.CurrentDungeonIndex;
			Dungeon nextDungeon = _gameContext.Dungeons[_gameContext.CurrentDungeonIndex];
			BoundsInt furthestRoomToStairs = FurthestRoomToStairsResolver.GetFurthestRoomToStairs(nextDungeon);
			Vector2Int startingPosition = new Vector2Int((int) furthestRoomToStairs.center.x, (int) furthestRoomToStairs.center.y);
			_gameContext.PlayerActor.ActorData.LogicalPosition = startingPosition;
			Action action = () =>
			{
				_gameContext.PlayerActor.RefreshWorldPosition();
			};

			yield return new LambdaEffect(action);
		}
	}
}