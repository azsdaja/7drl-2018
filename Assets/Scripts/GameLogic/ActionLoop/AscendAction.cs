using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.ActionEffects;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration;
using Assets.Scripts.GameLogic.Configuration;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.GridRelated.TilemapAffecting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
			if(_gameContext.CurrentDungeonIndex < 6)
				GameObject.Find("PrisonLevelIndicator").GetComponent<Text>().text = "Prison level: " + -(5 - _gameContext.CurrentDungeonIndex);
			if (_gameContext.CurrentDungeonIndex >= _gameContext.Dungeons.Count)
			{
				_gameContext.PlayerActor.ActorData.LogicalPosition =
					//new Vector2Int(6, -41); // dawno tego nie robiłem... niesamowite uczucie
					new Vector2Int(5, -65); // dawno tego nie robiłem... niesamowite uczucie
													
				 _gameContext.PlayerActor.ActorData.VisionRayLength = 8; _gameContext.VisiblePositions = new HashSet<Vector2Int>();
				IEnumerable<ActorData> enemiesAround =
					_entityDetector.DetectActors(_gameContext.PlayerActor.ActorData.LogicalPosition, 20)
					.Where(a => a.Team == Team.Beasts);
				foreach (var actorData in enemiesAround)
				{
					actorData.VisionRayLength = 8;
				}
				//this still breaks the field of view!
			}
			else
			{
				Dungeon nextDungeon = _gameContext.Dungeons[_gameContext.CurrentDungeonIndex];
				BoundsInt furthestRoomToStairs = FurthestRoomToStairsResolver.GetFurthestRoomToStairs(nextDungeon);
				Vector2Int startingPosition = new Vector2Int((int)furthestRoomToStairs.center.x, (int)furthestRoomToStairs.center.y);
				_gameContext.PlayerActor.ActorData.LogicalPosition = startingPosition;
				TileBase stairsDownTile = Resources.Load<TileBase>("Tiles/Environment/Stairs_down");
				_gameContext.EnvironmentTilemap.SetTile(startingPosition.ToVector3Int(), stairsDownTile);
				IEnumerable<ActorData> actorAround = _entityDetector.DetectActors(startingPosition, 3);
				foreach (var actorData in actorAround)
				{
					_entityRemover.CleanSceneAndGameContextAfterDeath(actorData);
					Debug.Log("Removing an actor, because he was too close: " + actorData.ActorType);
				}
			}
			
			Action action = () =>
			{
				_gameContext.PlayerActor.RefreshWorldPosition();
			};

			yield return new LambdaEffect(action);
		}
	}
}