using System.Collections.Generic;
using Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration;
using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	public interface IGameContext
	{
		Grid GameGrid { get; }
		UnityEngine.Tilemaps.Tilemap TilemapDefiningOuterBounds { get; }
		UnityEngine.Tilemaps.Tilemap WallsTilemap { get; }
		UnityEngine.Tilemaps.Tilemap ObjectsTilemap { get; }
		UnityEngine.Tilemaps.Tilemap FloorsTilemap { get; }
		UnityEngine.Tilemaps.Tilemap DirtTilemap { get; }
		UnityEngine.Tilemaps.Tilemap EnvironmentTilemap { get; }
		ActorBehaviour PlayerActor { get; set; }
		bool ControlBlocked { get; set; }

		HashSet<ActorBehaviour> Actors { get; set; }
		HashSet<Vector2Int> VisiblePositions { get; set; }
		List<Dungeon> Dungeons { get; set; }
		int CurrentDungeonIndex { get; set; }
		
		IList<Vector2Int> HousePositions { get; }
		IList<Vector2Int> LeavesPositions { get; }
		int BasherSteps { get; set; }
		void AddHousePosition(int x, int y);
	}
}