using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.FieldOfView
{
	public class FovSquareOutlineCreator : IFovSquareOutlineCreator
	{
		public HashSet<Vector2Int> CreateSquareOutline(Vector2Int fovCenter, int rayLength)
		{
			HashSet<Vector2Int> squareOutline = new HashSet<Vector2Int>();

			for (int xOnOutline = fovCenter.x - rayLength; xOnOutline <= fovCenter.x + rayLength; xOnOutline++)
			{
				int yOnNorthernOutline = fovCenter.y + rayLength;
				squareOutline.Add(new Vector2Int(xOnOutline, yOnNorthernOutline));
				int yOnSouthernOutline = fovCenter.y - rayLength;
				squareOutline.Add(new Vector2Int(xOnOutline, yOnSouthernOutline));
			}
			for (int yOnOutline = fovCenter.y - rayLength + 1; yOnOutline <= fovCenter.y + rayLength - 1; yOnOutline++)
			{
				int xOnWesternOutline = fovCenter.x - rayLength;
				squareOutline.Add(new Vector2Int(xOnWesternOutline, yOnOutline));
				int xOnEasternOutline = fovCenter.x + rayLength;
				squareOutline.Add(new Vector2Int(xOnEasternOutline, yOnOutline));
			}

			return squareOutline;
		}
	}
}