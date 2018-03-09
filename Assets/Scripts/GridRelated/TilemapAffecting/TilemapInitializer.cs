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
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using Debug = UnityEngine.Debug;
using Tile = UnityEngine.Tilemaps.Tile;

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
				var dungeon = GenerateDungeon(dungeonIndex * 60, 0, dungeonConfig.Size.x, dungeonConfig.Size.y);
				_gameContext.Dungeons.Add(dungeon);
				GenerateActorsInDungeon(dungeon, dungeonConfig, isFirstDungeon: dungeonIndex == 0);
			}

			_gameContext.CurrentDungeonIndex = 0;

			//GenerateWilderness();
			InitializeVisibilityOfTiles();
			_pathfinder.InitializeNavigationGrid();

			//GenerateAnimals(6);
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
				var playerActorBehaviour = _entitySpawner.SpawnActor(ActorType.Player, playerPosition);

				playerActorBehaviour.ActorData.ControlledByPlayer = true;
				_gameContext.PlayerActor = playerActorBehaviour;
				_gameConfig.FollowPlayerCamera.Follow = playerActorBehaviour.transform;
				_uiConfig.Arrows.transform.parent = playerActorBehaviour.transform;
				_uiConfig.Arrows.transform.localPosition = Vector3.zero;
			}

			foreach (BoundsInt room in currentDungeon.Rooms.Skip(isFirstDungeon ? 1 :0))
			{
				if (room == playerRoom) continue;
				float populationValue = _rng.NextFloat();
				int populationInRoom = Mathf.RoundToInt(dungeonConfig.ChanceToRoomPopulation.Evaluate(populationValue));
				Debug.Log("dungeon " + dungeonConfig.Size + ": " + populationInRoom);
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
						_gameContext.FloorsTilemap.SetTile(position, Dirt);
						break;
					}
					case GenTile.Corridor:
					{
						_gameContext.FloorsTilemap.SetTile(position, Dirt);
						break;
					}
					case GenTile.StoneWall:
					case GenTile.DirtWall:
					{
						_gameContext.WallsTilemap.SetTile(position, Wall);
						break;
					}
					case GenTile.Upstairs:
					{
						_gameContext.FloorsTilemap.SetTile(position, Dirt);
						_gameContext.EnvironmentTilemap.SetTile(position, StairsUp);
						break;
					}
					case GenTile.Downstairs:
					{
						_gameContext.FloorsTilemap.SetTile(position, Dirt);
						//_gameContext.EnvironmentTilemap.SetTile(position, StairsDown);
						break;
					}
					case GenTile.Door:
					{
						_gameContext.FloorsTilemap.SetTile(position, Dirt);
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


		// Update is called once per frame

		private void GenerateWilderness()
		{
			_gameContext.TilemapDefiningOuterBounds.CompressBounds();
			BoundsInt gridBounds = _gameContext.TilemapDefiningOuterBounds.cellBounds;

			GenerateDirt(gridBounds);
			GenerateFloorsWithPerlin(gridBounds);
			GenerateCavesWithPerlin(gridBounds);

			PlaceHouses(gridBounds, 3);
		}

		private void GenerateDirt(BoundsInt gridBounds)
		{
			foreach (Vector3Int currentPosition in gridBounds.allPositionsWithin)
			{
				UnityEngine.Tilemaps.Tilemap tilemapToAffect = _gameContext.DirtTilemap;
				tilemapToAffect.SetTileFlags(currentPosition, TileFlags.None);
				tilemapToAffect.SetTile(currentPosition, Dirt);
			}
		}

		private void GenerateFloorsWithPerlin(BoundsInt gridBounds)
		{
			float perlinScale = .08f;
			float perlinOffsetX = _rng.Next(10000);
			float perlinOffsetY = _rng.Next(10000);

			for (int x = gridBounds.xMin; x <= gridBounds.xMax; x++)
			for (int y = gridBounds.yMin; y <= gridBounds.yMax; y++)
			{
				var currentPosition = new Vector3Int(x, y, 0);
				var tilemapsToAffect = new[] {_gameContext.FloorsTilemap};
				foreach (var tilemap in tilemapsToAffect)
				{
					float perlinX = currentPosition.x * perlinScale + perlinOffsetX;
					float perlinY = currentPosition.y * perlinScale + perlinOffsetY;
					float value1 = Mathf.PerlinNoise(perlinX, perlinY);
					float value2 = Mathf.PerlinNoise(perlinX * 2, perlinY * 2);
					float value4 = Mathf.PerlinNoise(perlinX * 4, perlinY * 4);
					float compundValue = (value1 + value2 + value4) * .4f;

					tilemap.SetTileFlags(currentPosition, TileFlags.None);
					InitializeTileWithBiomeFloor(compundValue, tilemap, currentPosition);
					tilemap.SetColor(currentPosition, Color.white);
				}
				PlaceRandomEnvironmentalObjects(currentPosition);
			}
		}

		private void GenerateCavesWithPerlin(BoundsInt gridBounds)
		{
			float perlinScale = .08f;
			float perlinOffsetX = _rng.Next(10000);
			float perlinOffsetY = _rng.Next(10000);

			for (int x = gridBounds.xMin; x <= gridBounds.xMax; x++)
			for (int y = gridBounds.yMin; y <= gridBounds.yMax; y++)
			{
				var currentPosition = new Vector2Int(x, y);
				float perlinX = currentPosition.x * perlinScale + perlinOffsetX;
				float perlinY = currentPosition.y * perlinScale + perlinOffsetY;
				float value = Mathf.PerlinNoise(perlinX, perlinY);

				if (value < 0.29f)
				{
					_caveTiles.Add(currentPosition);
					RemoveEnvironmentalObjects(currentPosition);
					_gameContext.WallsTilemap.SetTile(currentPosition.ToVector3Int(), Wall);
				}
			}

			foreach (var potentialRootTile in _caveTiles)
			{
				if (_processedCaveTiles.Contains(potentialRootTile))
					continue;

				_caveRootsToCaves[potentialRootTile] = new HashSet<Vector2Int>();
				Stack<Vector2Int> tilesToProcess = new Stack<Vector2Int>();
			
				tilesToProcess.Push(potentialRootTile);
				do
				{
					Vector2Int poppedTile = tilesToProcess.Pop();

					if (_gameContext.WallsTilemap.HasTile(poppedTile.ToVector3Int()))
					{
						_processedCaveTiles.Add(poppedTile);
						_caveRootsToCaves[potentialRootTile].Add(poppedTile);
						foreach (var neighbour in Vector2IntUtilities.Neighbours4(poppedTile))
						{
							if (!_processedCaveTiles.Contains(neighbour))
							{
								tilesToProcess.Push(neighbour);
							}
						}
					}
				} while (tilesToProcess.Any());
			}
			foreach (var caveRootsToCave in _caveRootsToCaves)
			{
				HashSet<Vector2Int> positionsInThisCave = caveRootsToCave.Value;
				if (positionsInThisCave.Count < 20) continue;
				int drunkardWalksToPerformInThisCave = positionsInThisCave.Count / 15;
				for (int i = 0; i < drunkardWalksToPerformInThisCave; i++)
				{
					DrunkardWalk(_rng.Choice(positionsInThisCave.ToList()));
				}
			}
		}

		private void DrunkardWalk(Vector2Int startingPosition)
		{
			Vector3Int currentPosition = startingPosition.ToVector3Int();
			while (_gameContext.WallsTilemap.HasTile(currentPosition))
			{
				_gameContext.WallsTilemap.SetTile(currentPosition, null);
				_gameContext.FloorsTilemap.SetTile(currentPosition, null);

				currentPosition = _rng.Choice(Vector2IntUtilities.Neighbours4(currentPosition.ToVector2Int())).ToVector3Int();
			}
		}

		private void InitializeTileWithBiomeFloor(float value, UnityEngine.Tilemaps.Tilemap tilemap, Vector3Int currentPosition)
		{
			if (value < .3)
			{
				// let dirt remain there
			}
			else if (value < 1.0)
			{
				tilemap.SetTile(currentPosition, Grass);
			}
		}

		private void PlaceRandomEnvironmentalObjects(Vector3Int currentPosition)
		{
			var treePotTile = Resources.Load<Tile>(@"Tiles\Environment\TreePot");
			var bigLeavesTile = Resources.Load<Tile>(@"Tiles\Environment\BigLeaves");
			var bushTile = Resources.Load<Tile>(@"Tiles\Environment\Bush");
			var choppedTreeTile = Resources.Load<Tile>(@"Tiles\Environment\ChoppedTree");
			var littleLeavesTile = Resources.Load<Tile>(@"Tiles\Environment\LittleLeaves");
			var mushroomsTile = Resources.Load<Tile>(@"Tiles\Environment\Mushrooms");
			if (_rng.Check(0.01f))
			{
				_gameContext.WallsTilemap.SetTile(currentPosition, treePotTile);
			}
			else if (_rng.Check(0.001f))
			{
				_gameContext.EnvironmentTilemap.SetTile(currentPosition, bigLeavesTile);
				//_gameContext.AddSmellable(new Smellable(bigLeavesTile.sprite, .5f, () => currentPosition.ToVector2Int(), SmellableType.BigLeaves));
				_gameContext.LeavesPositions.Add(currentPosition.ToVector2Int());
			}
			else if (_rng.Check(0.005f))
			{
				_gameContext.EnvironmentTilemap.SetTile(currentPosition, bushTile);
			}
			else if (_rng.Check(0.005f))
			{
				_gameContext.EnvironmentTilemap.SetTile(currentPosition, choppedTreeTile);
			}
			else if (_rng.Check(0.005f))
			{
				_gameContext.EnvironmentTilemap.SetTile(currentPosition, littleLeavesTile);
			}
			else if (_rng.Check(0.002f))
			{
				_gameContext.EnvironmentTilemap.SetTile(currentPosition, mushroomsTile);
			}
		}

		private void RemoveEnvironmentalObjects(Vector2Int currentPosition)
		{
			_gameContext.EnvironmentTilemap.SetTile(currentPosition.ToVector3Int(), null);
			_gameContext.LeavesPositions.Remove(currentPosition);
		}

		private void PlaceHouses(BoundsInt gridBounds, int count)
		{
			var woodenWallTile = Resources.Load<Tile>(@"Tiles\Walls\WoodenWall");
			var woodenFloorTile = Resources.Load<Tile>(@"Tiles\Floors\WoodenFloor");
			var foodSetTile = Resources.Load<Tile>(@"Tiles\Objects\FoodSet");
			for (int i = 0; i < count; i++)
			{
				int houseX = _rng.Next(gridBounds.xMin, gridBounds.xMax);
				int houseY = _rng.Next(gridBounds.yMin, gridBounds.yMax);
				int size = 5;
				_gameContext.AddHousePosition(houseX + size/2, houseY + size/2);
				bool foodHasBeenPlaced = false;

				for (int wallX = houseX; wallX <= houseX + size; wallX++)
				for (int wallY = houseY; wallY <= houseY + size; wallY++)
				{
					_gameContext.FloorsTilemap.SetTile(new Vector3Int(wallX, wallY, 0), woodenFloorTile);
					_gameContext.EnvironmentTilemap.SetTile(new Vector3Int(wallX, wallY, 0), null);

					bool isPerimeter = wallX == houseX || wallX == houseX + size || wallY == houseY || wallY == houseY + size;
					bool isDoor = isPerimeter && wallX == houseX + 3 && wallY == houseY;
					if (isPerimeter && !isDoor)
					{
						_gameContext.WallsTilemap.SetTile(new Vector3Int(wallX, wallY, 0), woodenWallTile);
					}
					else // is inside
					{
						if (!foodHasBeenPlaced && _rng.Check(0.05f))
						{
							//_gameContext.EnvironmentTilemap.SetTile(new Vector3Int(wallX, wallY, 0), foodSetTile);
							int smellableX = wallX;
							int smellableY = wallY;
							//_gameContext.AddSmellable(new Smellable(foodSetTile.sprite, 0.5f, () => new Vector2Int(smellableX, smellableY), SmellableType.Food));
							foodHasBeenPlaced = true;
						}
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
				var tilemapsToAffect = new[] {_gameContext.DirtTilemap, _gameContext.FloorsTilemap, _gameContext.EnvironmentTilemap, _gameContext.WallsTilemap };
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

		private void GenerateAnimals(int familiesCount)
		{
			BoundsInt gridBounds = _gameContext.TilemapDefiningOuterBounds.cellBounds;

			for (int i = 0; i < familiesCount; i++)
			{
				Vector2Int familyPosition = _rng.NextPosition(gridBounds);
				var deerFatherPosition = GetBiasedWalkablePosition(familyPosition);
				var deerMotherPosition = GetBiasedWalkablePosition(familyPosition);
				var deerImmaturePosition = GetBiasedWalkablePosition(familyPosition);
				//ActorBehaviour spawnedFather = _entitySpawner.SpawnActor(ActorType.HerdAnimalFather, deerFatherPosition);
				//ActorBehaviour spawnedMother = _entitySpawner.SpawnActor(ActorType.HerdAnimalMother, deerMotherPosition);
				//ActorBehaviour spawnedImmature = _entitySpawner.SpawnActor(ActorType.HerdAnimalImmature, deerImmaturePosition);
				//spawnedFather.ActorData.AiData.HerdMemberData.Child = spawnedImmature;
				//spawnedMother.ActorData.AiData.HerdMemberData.Child = spawnedImmature;
				//spawnedImmature.ActorData.AiData.HerdMemberData.Protector = spawnedMother;
			}
		}

		private Vector2Int GetBiasedWalkablePosition(Vector2Int centralPosition)
		{
			Vector2Int deerPosition;
			bool positionIsClosed;
			do
			{
				deerPosition = _rng.BiasedPosition(centralPosition, 4);
				Vector2Int positionToValidateWalkability = deerPosition + new Vector2Int(5, 0);
				var jumpPoints = _pathfinder.GetJumpPoints(deerPosition, positionToValidateWalkability);
				positionIsClosed = jumpPoints == null;
			} while (!_gridInfoProvider.IsWalkable(deerPosition) || positionIsClosed);
			return deerPosition;
		}
	}
}