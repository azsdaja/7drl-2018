using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.ActionLoop.Actions;
using Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.RNG;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.GridRelated.TilemapAffecting
{
	public class TilemapInitializer : MonoBehaviour
	{
		public bool InitializeAsUndiscovered = true;

		public TileBase Dirt;
		public TileBase Grass;
		public TileBase Wall;
		public TileBase StairsUp;
		public TileBase StairsDown;
		public TileBase DoorsHorizontalClosed;
		public TileBase DoorsVerticalClosed;
		public TileBase HeavyDoorsHorizontalClosed;
		public TileBase HeavyDoorsVerticalClosed;
		public TileBase[] FloorEnvironmetals;
		public TileBase[] WallEnvironmetals;
		public TileBase[] WallAttachmentEnvironmetals;

		public ItemDefinition BreadItem;
		public ItemDefinition KeyItem;

		private HashSet<Vector2Int> _caveTiles;
		private HashSet<Vector2Int> _processedCaveTiles;
		private IDictionary<Vector2Int, HashSet<Vector2Int>> _caveRootsToCaves;

		private IGameContext _gameContext;
		private IGameConfig _gameConfig;
		private IUiConfig _uiConfig;
		private IPathfinder _pathfinder;
		private IRandomNumberGenerator _rng;
		private IGridInfoProvider _gridInfoProvider;
		private IEntitySpawner _entitySpawner;

		[Inject]
		public void Init(IGameContext gameContext, IGameConfig gameConfig, IUiConfig uiConfig, IPathfinder pathfinder, 
			IRandomNumberGenerator randomNumberGenerator, IGridInfoProvider gridInfoProvider, IEntitySpawner entitySpawner)
		{
			_gameContext = gameContext;
			_gameConfig = gameConfig;
			_uiConfig = uiConfig;
			_pathfinder = pathfinder;
			_rng = randomNumberGenerator;
			_gridInfoProvider = gridInfoProvider;
			_entitySpawner = entitySpawner;

			_caveTiles = new HashSet<Vector2Int>();
			_processedCaveTiles = new HashSet<Vector2Int>();
			_caveRootsToCaves = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
		}

		// Use this for initialization
		void Start()
		{
			DungeonConfig[] dungeonConfigs = _gameConfig.DungeonConfigs;
			for (var dungeonIndex = 0; dungeonIndex < dungeonConfigs.Length; dungeonIndex++)
			{
				DungeonConfig dungeonConfig = dungeonConfigs[dungeonIndex];
				Dungeon dungeon;
				while (true)
				{
					dungeon = GenerateDungeon(dungeonIndex * 60, 0, dungeonConfig.Size.x, dungeonConfig.Size.y);
					if (dungeon.StairsLocation != Vector2Int.zero)
						break;
					Debug.LogError("dungeon was created without stairs! trying to recreate");
				}
				_gameContext.Dungeons.Add(dungeon);
				GenerateActorsInDungeon(dungeon, dungeonConfig, isFirstDungeon: dungeonIndex == 0);
			}

			GenerateActorsAndItemsOutside();

			_gameContext.CurrentDungeonIndex = 0;

			//GenerateWilderness();
			InitializeVisibilityOfTiles();
			_pathfinder.InitializeNavigationGrid();

			//GenerateAnimals(6);
		}

		private void GenerateActorsAndItemsOutside()
		{
			var bound = new BoundsInt(new Vector3Int(-2, -53, 0), new Vector3Int(19,18,1));
			for (int i = 0; i < 10; i++)
			{
				Vector2Int randomPosition = _rng.NextPosition(bound);
				if (_gridInfoProvider.IsWalkable(randomPosition))
				{
					var actor = _rng.Choice(new[]
					{
						ActorType.Rogue,
						ActorType.BruisedRat,
						ActorType.Rat,
						ActorType.RatChief,
						ActorType.Dog,
						ActorType.RatVeteran,
					});
					_entitySpawner.SpawnActor(actor, randomPosition);
				}
			}
			ItemDefinition recoverTailDefinition = Resources.Load<ItemDefinition>("PotionOfRecoverTail");
			_entitySpawner.SpawnItem(recoverTailDefinition, new Vector2Int(-3,-83));
		}

		private Dungeon GenerateDungeon(int positionX, int positionY, int sizeX, int sizeY)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			
			//_gameContext.TilemapDefiningOuterBounds.CompressBounds();
			//BoundsInt gridBounds = _gameContext.TilemapDefiningOuterBounds.cellBounds;
			BoundsInt gridBounds = new BoundsInt(positionX, positionY, 0, sizeX, sizeY, 1);

			Dungeon generator = new Dungeon(_rng, message => { });

			int tilesCount = gridBounds.size.x * gridBounds.size.y;
			int requestedFeatures = tilesCount / 100;

			generator.CreateDungeon(gridBounds.min, gridBounds.size.x, gridBounds.size.y, requestedFeatures);

			//GenerateDirt(gridBounds);

			PlaceTilesBasingOnDungeon(gridBounds, generator);

			return generator;
		}

		private void GenerateActorsInDungeon(Dungeon currentDungeon, DungeonConfig dungeonConfig, bool isFirstDungeon)
		{
			BoundsInt playerRoom = new BoundsInt();
			if (isFirstDungeon)
			{
				BoundsInt furthestRoomToStairs = FurthestRoomToStairsResolver.GetFurthestRoomToStairs(currentDungeon);

				playerRoom = furthestRoomToStairs;

				BoundsInt roomToSpawnPlayerIn = playerRoom;
				Vector2Int playerPosition = BoundsIntUtilities.Center(roomToSpawnPlayerIn);
				Vector2Int breadPosition = Vector2Int.zero;
				foreach (var neighbour in Vector2IntUtilities.Neighbours8(playerPosition))
				{
					if (_gridInfoProvider.IsWalkable(neighbour))
					{
						_entitySpawner.SpawnItem(BreadItem, neighbour);
						breadPosition = neighbour;
						break;
					}
				}
				foreach (var neighbour in Vector2IntUtilities.Neighbours8(playerPosition))
				{
					if (neighbour != breadPosition && _gridInfoProvider.IsWalkable(neighbour))
					{
						_entitySpawner.SpawnItem(KeyItem, neighbour);
						break;
					}
				}
				BoundsInt aroundPlayerRoom = new BoundsInt(playerRoom.position - new Vector3Int(1, 1, 0), 
					playerRoom.size + new Vector3Int(2, 2, 0));
				foreach (Vector3Int positionInPlayerRoom in aroundPlayerRoom.allPositionsWithin)
				{
					if (_gameContext.WallsTilemap.GetTile(positionInPlayerRoom) == DoorsVerticalClosed)
					{
						_gameContext.WallsTilemap.SetTile(positionInPlayerRoom, HeavyDoorsVerticalClosed);
					}
					if (_gameContext.WallsTilemap.GetTile(positionInPlayerRoom) == DoorsHorizontalClosed)
					{
						_gameContext.WallsTilemap.SetTile(positionInPlayerRoom, HeavyDoorsHorizontalClosed);
					}
				}
				var playerActorBehaviour = _entitySpawner.SpawnActor(ActorType.Player, playerPosition);

				playerActorBehaviour.ActorData.ControlledByPlayer = true;
				_gameContext.PlayerActor = playerActorBehaviour;
				_gameConfig.FollowPlayerCamera.Follow = playerActorBehaviour.transform;
				_uiConfig.Arrows.transform.parent = playerActorBehaviour.transform;
				_uiConfig.Arrows.transform.localPosition = new Vector3(0, -0.1f, 0);
			}

			foreach (BoundsInt room in currentDungeon.Rooms.Skip(isFirstDungeon ? 1 :0))
			{
				if (room == playerRoom) continue;
				float populationValue = _rng.NextFloat();
				int populationInRoom = Mathf.RoundToInt(dungeonConfig.ChanceToRoomPopulation.Evaluate(populationValue));
				for (int i = 0; i < populationInRoom; i++)
				{
					ActorDefinition[] actorTypesAvailable = dungeonConfig.EnemiesToSpawn;
					ActorType actorTypeChosen = _rng.Choice(actorTypesAvailable).ActorType;
					Vector2Int centralPosition = BoundsIntUtilities.Center(room);
					_entitySpawner.SpawnActor(actorTypeChosen, centralPosition);
				}
			}

			_entitySpawner.SpawnActor(dungeonConfig.BossToSpawn.ActorType, currentDungeon.StairsLocation);
		}

		private void PlaceTilesBasingOnDungeon(BoundsInt gridBounds, Dungeon generator)
		{
			foreach (Vector3Int position in gridBounds.allPositionsWithin)
			{
				Vector2Int position2D = position.ToVector2Int();
				GenTile genTile = generator.GetCellType(position2D.x, position2D.y);
				switch (genTile)
				{
					case GenTile.DirtFloor:
					{
						_gameContext.DirtTilemap.SetTile(position, Dirt);
						if (_rng.Check(0.03f))
						{
							_gameContext.EnvironmentTilemap.SetTile(position, _rng.Choice(FloorEnvironmetals));
						}
						if (_rng.Check(0.06f))
						{
							if(Vector2IntUtilities.Neighbours8(position2D).All(n => generator.GetCellType(n.x, n.y) == GenTile.DirtFloor))
							_gameContext.ObjectsTilemap.SetTile(position, _rng.Choice(WallEnvironmetals));
						}
						break;
					}
					case GenTile.Corridor:
					{
						_gameContext.DirtTilemap.SetTile(position, Dirt);
						break;
					}
					case GenTile.StoneWall:
					case GenTile.DirtWall:
					{
						_gameContext.WallsTilemap.SetTile(position, Wall);
						if (_rng.Check(0.04f))
						{
							_gameContext.EnvironmentTilemap.SetTile(position, _rng.Choice(WallAttachmentEnvironmetals));
						}
							break;
					}
					case GenTile.Upstairs:
					{
						_gameContext.DirtTilemap.SetTile(position, Dirt);
						_gameContext.EnvironmentTilemap.SetTile(position, StairsUp);
						break;
					}
					case GenTile.Downstairs:
					{
						_gameContext.DirtTilemap.SetTile(position, Dirt);
						break;
					}
					case GenTile.Door:
					{
						_gameContext.DirtTilemap.SetTile(position, Dirt);
						GenTile tileToRight = generator.GetCellType(position.x + 1, position.y);
						bool isHorizontalDoor = tileToRight == GenTile.Corridor || tileToRight == GenTile.DirtFloor;
						_gameContext.WallsTilemap.SetTile(position, isHorizontalDoor ? DoorsHorizontalClosed : DoorsVerticalClosed);
						break;
					}
					default:
					{
						break;
					}
				}
			}
		}


		private void InitializeVisibilityOfTiles()
		{
			BoundsInt gridBounds = _gameContext.TilemapDefiningOuterBounds.cellBounds;
			for (int x = gridBounds.xMin; x <= gridBounds.xMax; x++)
			for (int y = gridBounds.yMin; y <= gridBounds.yMax; y++)
			{
				var currentPosition = new Vector3Int(x, y, 0);
				var tilemapsToAffect = new[]
				{
					_gameContext.DirtTilemap, _gameContext.FloorsTilemap, _gameContext.EnvironmentTilemap,
					_gameContext.ObjectsTilemap, _gameContext.WallsTilemap
				};
				foreach (var tilemap in tilemapsToAffect)
				{
					tilemap.SetTileFlags(currentPosition, TileFlags.None);
					if(InitializeAsUndiscovered)
						tilemap.SetColor(currentPosition, TileColors.UndiscoveredColor);
					else
						tilemap.SetColor(currentPosition, TileColors.UnlitColor);
				}
			}
		}
	}
}