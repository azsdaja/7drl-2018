using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FieldOfView
{
	public interface IFovSquareOutlineCreator
	{
		/// <summary>
		/// Creates a list of points outlining a square around Field of View, so that Bresenham lines can be cast to the outline.
		/// </summary>
		HashSet<Vector2Int> CreateSquareOutline(Vector2Int fovCenter, int rayLength);
	}
}