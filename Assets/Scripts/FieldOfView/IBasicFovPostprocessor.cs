using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FieldOfView
{
	public interface IBasicFovPostprocessor
	{
		/// <summary>
		/// In postprocessing, in order to fix some artifacts, we include in the visible set the wall tiles that are behind a visible floor tile.
		/// </summary>
		IEnumerable<Vector2Int> PostprocessBasicFov(
			HashSet<Vector2Int> visibleBeforePostProcessing, Vector2Int centerOfSquareToPostProcess, int rayLength, Func<Vector2Int, bool> isWalkable);
	}
}