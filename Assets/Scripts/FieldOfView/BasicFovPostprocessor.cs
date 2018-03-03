using System;
using System.Collections.Generic;
using Assets.Scripts.CSharpUtilities;
using UnityEngine;

namespace Assets.Scripts.FieldOfView
{
	public class BasicFovPostprocessor : IBasicFovPostprocessor
	{
		public IEnumerable<Vector2Int> PostprocessBasicFov(
			HashSet<Vector2Int> visibleBeforePostProcessing, Vector2Int centerOfSquareToPostProcess, int rayLength, Func<Vector2Int, bool> isWalkable)
		{
			int rayLengthSquared = rayLength * rayLength;
		
			var boundsMinCorner = new Vector3Int(centerOfSquareToPostProcess.x - rayLength, centerOfSquareToPostProcess.y - rayLength, 0);
			int fovSquareSize = 2 * rayLength + 1; // two rayLengths plus one unit for center
			BoundsInt boundsToPostProcess = new BoundsInt(boundsMinCorner, new Vector3Int(fovSquareSize, fovSquareSize, 1));
			foreach (Vector3Int position3D in boundsToPostProcess.allPositionsWithin)
			{
				var currentPosition = new Vector2Int(position3D.x, position3D.y);

				bool isWithinRay = (currentPosition - centerOfSquareToPostProcess).sqrMagnitude <= rayLengthSquared;
				bool canBeIlluminated = 
					!visibleBeforePostProcessing.Contains(currentPosition) 
					&& isWithinRay 
					&& !isWalkable(currentPosition);
				if (!canBeIlluminated)
					continue;

				IEnumerable<Vector2Int> behindnessVectors = GetBehindnessVectors(currentPosition, centerOfSquareToPostProcess);
				foreach (var behindnessVector in behindnessVectors)
				{
					Vector2Int potentialIlluminatingPosition = currentPosition - behindnessVector;
					bool potentialIlluminatingNeighbourIsLitAndNonBlocking 
						= visibleBeforePostProcessing.Contains(potentialIlluminatingPosition) && isWalkable(potentialIlluminatingPosition);
					if (potentialIlluminatingNeighbourIsLitAndNonBlocking)
					{
						yield return currentPosition;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Returns normalized vectors that point from a tile to the tile that can be considered to be "behind" it.
		/// </summary>
		internal IEnumerable<Vector2Int> GetBehindnessVectors(Vector2Int currentPosition, Vector2Int centerOfSquareToPostProcess)
		{
			Vector2Int direction = currentPosition - centerOfSquareToPostProcess;
			var snappedToX = Vector2IntUtilities.SnapToXAxisNormalized(direction);
			var snappedToY = Vector2IntUtilities.SnapToYAxisNormalized(direction);
			if(snappedToX != Vector2Int.zero) yield return snappedToX;
			if(snappedToY != Vector2Int.zero) yield return snappedToY;
		}
	}
}