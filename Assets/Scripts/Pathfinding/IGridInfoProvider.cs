using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	public interface IGridInfoProvider
	{
		int XSize { get; }
		int YSize { get; }
		Vector2Int MinPosition { get; }
		float CellSize { get; set; }
		bool IsWalkable(Vector2Int positionToCheckForWall);
		Vector3Int LocalToCell(Vector3 localPosition);
		Vector3Int WorldToCell(Vector3 worldPosition);
		Vector3 GetCellCenterWorld(Vector2Int cellPosition);
	}
}