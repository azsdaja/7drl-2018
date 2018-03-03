using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FieldOfView
{
	/// <summary>
	/// A calculator of field of view basing on "Basic" Algorithm described in https://sites.google.com/site/jicenospam/visibilitydetermination.
	/// </summary>
	public class BasicFovCalculator : IFovCalculator
	{
		private readonly IFovSquareOutlineCreator _fovSquareOutlineCreator;
		private readonly IBasicFovPostprocessor _basicFovPostprocessor;
		private readonly IBresenhamLineCreator _bresenhamLineCreator;

		public BasicFovCalculator(IFovSquareOutlineCreator fovSquareOutlineCreator, IBasicFovPostprocessor basicFovPostprocessor, 
			IBresenhamLineCreator bresenhamLineCreator)
		{
			_basicFovPostprocessor = basicFovPostprocessor;
			_fovSquareOutlineCreator = fovSquareOutlineCreator;
			_bresenhamLineCreator = bresenhamLineCreator;
		}

		public HashSet<Vector2Int> CalculateFov(Vector2Int fovCenter, int rayLength, Func<Vector2Int, bool> isWalkable)
		{
			var visibleTiles = new HashSet<Vector2Int>();
			HashSet<Vector2Int> outlineToCastRaysOn = _fovSquareOutlineCreator.CreateSquareOutline(fovCenter, rayLength);
			foreach (Vector2Int point in outlineToCastRaysOn)
			{
				IList<Vector2Int> bresenhamLineTiles = _bresenhamLineCreator.GetBresenhamLine(fovCenter.x, fovCenter.y, point.x, point.y, rayLength, isWalkable);
				visibleTiles.UnionWith(bresenhamLineTiles);
			}

			IEnumerable<Vector2Int> visibleTilesFromPostProcessing = 
				_basicFovPostprocessor.PostprocessBasicFov(visibleTiles, fovCenter, rayLength, isWalkable);
			visibleTiles.UnionWith(visibleTilesFromPostProcessing);
			return visibleTiles;
		}
	}
}