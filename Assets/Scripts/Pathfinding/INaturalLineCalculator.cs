using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	/// <summary>Calculates the longest natural (Bresenham) line which a character 
	/// can walk to optimally follow the path defined by given jump points</summary>
	/// <example>
	/// For:
	/// ..2******3
	/// .*..######
	/// 1.........
	/// 
	/// Gives:
	/// ....**....
	/// ..**######
	/// **........
	/// </example>
	public interface INaturalLineCalculator
	{
		IList<Vector2Int> GetFirstLongestNaturalLine(IList<Vector2Int> jumpPoints, Func<Vector2Int, bool> isWalkable);
		IList<Vector2Int> GetFirstLongestNaturalLine(Vector2Int startNode, IList<Vector2Int> followingJumpPoints, Func<Vector2Int, bool> isWalkable);

		/// <summary>
		/// Usually the current JPS implementation creates too many jump points (many of them are aligned in one line).
		/// This function gives three first „natural” jump points (two or three), which means they don't form a single line.
		/// </summary>
		IList<Vector2Int> GetNaturalJumpPoints(IList<Vector2Int> jumpPoints);
	}
}