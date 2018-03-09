using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration;
using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class GameContext : MonoBehaviour, IGameContext
	{
		private IList<Vector2Int> _housePositions;
		private IList<Vector2Int> _leavesPositions;

		public Grid GameGrid;
		public UnityEngine.Tilemaps.Tilemap TilemapDefiningOuterBounds;
		public UnityEngine.Tilemaps.Tilemap WallsTilemap;
		public UnityEngine.Tilemaps.Tilemap DirtTilemap;
		public UnityEngine.Tilemaps.Tilemap FloorsTilemap;
		public UnityEngine.Tilemaps.Tilemap ObjectsTilemap;
		public UnityEngine.Tilemaps.Tilemap EnvironmentTilemap;
		public ActorBehaviour PlayerActor;
		private HashSet<ActorBehaviour> _actors;
		private HashSet<Vector2Int> _visiblePositions = new HashSet<Vector2Int>();
		private List<Dungeon> _dungeons = new List<Dungeon>();

		Grid IGameContext.GameGrid
		{
			get { return GameGrid; }
		}

		UnityEngine.Tilemaps.Tilemap IGameContext.TilemapDefiningOuterBounds
		{
			get { return TilemapDefiningOuterBounds; }
		}

		UnityEngine.Tilemaps.Tilemap IGameContext.WallsTilemap
		{
			get { return WallsTilemap; }
		}

		UnityEngine.Tilemaps.Tilemap IGameContext.DirtTilemap
		{
			get { return DirtTilemap; }
		}

		UnityEngine.Tilemaps.Tilemap IGameContext.FloorsTilemap
		{
			get { return FloorsTilemap; }
		}

		UnityEngine.Tilemaps.Tilemap IGameContext.EnvironmentTilemap
		{
			get { return EnvironmentTilemap; }
		}


		public bool ControlBlocked { get; set; }

		public HashSet<ActorBehaviour> Actors
		{
			get { return _actors; }
			set { _actors = value; }
		}

		public HashSet<Vector2Int> VisiblePositions
		{
			get { return _visiblePositions; }
			set { _visiblePositions = value; }
		}

		public List<Dungeon> Dungeons
		{
			get { return _dungeons; }
			set { _dungeons = value; }
		}

		public int CurrentDungeonIndex { get; set; }

		ActorBehaviour IGameContext.PlayerActor
		{
			get { return PlayerActor; }
			set { PlayerActor = value; }
		}

		public IList<Vector2Int> HousePositions
		{
			get { return _housePositions ?? (_housePositions = new List<Vector2Int>()); }
		}

		public IList<Vector2Int> LeavesPositions
		{
			get { return _leavesPositions ?? (_leavesPositions = new List<Vector2Int>()); }
		}


		public void AddHousePosition(int x, int y)
		{
			HousePositions.Add(new Vector2Int(x, y));
		}
	}
}
