using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FieldOfView
{
	public interface IFovCalculator
	{
		HashSet<Vector2Int> CalculateFov(Vector2Int fovCenter, int rayLength, Func<Vector2Int, bool> isWalkable);
	}
}