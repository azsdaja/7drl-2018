using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FieldOfView
{
	/// <summary>
	/// Creates a raster line connecting two points on a grid. Stops on blocking cells.
	/// </summary>
	/// <param name="rayLength">Limit for length of line. If set to -1, unlimited.</param>
	/// <example>
	/// .......**2
	/// ...****...
	/// 1**.......
	/// </example>
	public interface IBresenhamLineCreator
	{
		IList<Vector2Int> GetBresenhamLine(int x1, int y1, int x2, int y2);

		/// <remarks>Anno Domini 1965, doesn't need testing!</remarks>
		IList<Vector2Int> GetBresenhamLine(int x1, int y1, int x2, int y2, 
			int rayLength, Func<Vector2Int, bool> isWalkable, bool allowFinishOnUnwalkable = true);
	}
}