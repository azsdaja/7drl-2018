using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	public interface IPathfinder
	{
		List<Vector2Int> GetJumpPoints(Vector2Int startPoint, Vector2Int targetPoint);
		int CalculateCityLength(IList<Vector2Int> jumpPoints);
		void InitializeNavigationGrid();
	}
}
