using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	public class GridInfoProvider : IGridInfoProvider
	{
		private readonly IGameContext _gameContext;

		public int XSize { get; private set; }
		public int YSize { get; private set; }
		public Vector2Int MinPosition { get; private set; }
		public float CellSize { get; set; }

		public GridInfoProvider(IGameContext gameContext)
		{
			_gameContext = gameContext;
			BoundsInt cellBounds = _gameContext.TilemapDefiningOuterBounds.cellBounds;
			XSize = cellBounds.xMax - cellBounds.xMin;
			YSize = cellBounds.yMax - cellBounds.yMin;
			MinPosition = cellBounds.min.ToVector2Int();
			CellSize = _gameContext.TilemapDefiningOuterBounds.cellSize.x;
		}

		public bool IsWalkable(Vector2Int positionToCheckForWall)
		{
			return !_gameContext.WallsTilemap.HasTile(positionToCheckForWall.ToVector3Int())
			       && !_gameContext.ObjectsTilemap.HasTile(positionToCheckForWall.ToVector3Int());
		}

		public bool IsPassingLight(Vector2Int positionToCheckForWall)
		{
			return !_gameContext.WallsTilemap.HasTile(positionToCheckForWall.ToVector3Int());
		}

		public Vector3Int LocalToCell(Vector3 localPosition)
		{
			return _gameContext.GameGrid.LocalToCell(localPosition);
		}

		public Vector3Int WorldToCell(Vector3 worldPosition)
		{
			return _gameContext.GameGrid.WorldToCell(worldPosition);
		}

		public Vector3 GetCellCenterWorld(Vector2Int cellPosition)
		{
			return _gameContext.GameGrid.GetCellCenterWorld(cellPosition.ToVector3Int());
		}
	}
}