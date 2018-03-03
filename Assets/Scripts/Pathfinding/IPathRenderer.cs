using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	public interface IPathRenderer
	{
		void ShowPath(IList<Vector2Int> pathPoints, float score);
		void ShowNaturalWay(List<Vector2Int> jumpPoints, float score);
	}
}