using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FieldOfView
{
	public class BresenhamLineCreator : IBresenhamLineCreator
	{
		/// <remarks>Anno Domini 1965, doesn't need testing!</remarks>
		public IList<Vector2Int> GetBresenhamLine(int x1, int y1, int x2, int y2, 
			int rayLength, Func<Vector2Int, bool> isWalkable, bool allowFinishOnUnwalkable = true)
		{
			var result = new List<Vector2Int>();

			bool lengthLimited = rayLength != -1;
			int rayLengthSquared = rayLength * rayLength;
			Vector2Int lineBeginning = new Vector2Int(x1, y1);
			int xSize = x2 - x1;
			int ySize = y2 - y1;
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
			if (xSize < 0) dx1 = -1; else if (xSize > 0) dx1 = 1;
			if (ySize < 0) dy1 = -1; else if (ySize > 0) dy1 = 1;
			if (xSize < 0) dx2 = -1; else if (xSize > 0) dx2 = 1;
			int shorterAxis = Mathf.Abs(ySize);
			int longerAxis = Mathf.Abs(xSize);
			if (!(longerAxis > shorterAxis))
			{
				shorterAxis = Mathf.Abs(xSize);
				longerAxis = Mathf.Abs(ySize);
				if (ySize < 0) dy2 = -1; else if (ySize > 0) dy2 = 1;
				dx2 = 0;
			}
			int index = longerAxis >> 1;
			for (int i = 0; i <= longerAxis; i++)
			{
				var currentPosition = new Vector2Int(x1, y1);
				if (lengthLimited && (currentPosition - lineBeginning).sqrMagnitude > rayLengthSquared) continue;
				result.Add(currentPosition);
				if (!isWalkable(currentPosition))
				{
					if (!allowFinishOnUnwalkable) result.Remove(currentPosition);
					break;
				}
				index += shorterAxis;
				if (index < longerAxis)
				{
					x1 += dx2;
					y1 += dy2;
				}
				else
				{
					index -= longerAxis;
					x1 += dx1;
					y1 += dy1;
				}
			}
			return result;
		}
	}
}