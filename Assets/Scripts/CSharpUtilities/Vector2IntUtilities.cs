using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// todo: test
namespace Assets.Scripts.CSharpUtilities
{
	public static class Vector2IntUtilities
	{
		public static Vector2Int Min { get { return new Vector2Int(-int.MaxValue, -int.MaxValue); } }

		public static Vector2Int SnapToXAxisNormalized(Vector2Int vector)
		{
			return new Vector2Int(vector.x.Sign(), 0);
		}

		public static Vector2Int SnapToYAxisNormalized(Vector2Int vector)
		{
			return new Vector2Int(0, vector.y.Sign());
		}

		public static Vector2Int Normalized(Vector2Int vector)
		{
			Vector2 normalizedNonDiscrete = new Vector2(vector.x, vector.y).normalized;
			return new Vector2Int(Mathf.RoundToInt(normalizedNonDiscrete.x), Mathf.RoundToInt(normalizedNonDiscrete.y));
		}

		public static Vector2Int Average(IList<Vector2Int> vectors)
		{
			var sum = new Vector2Int();
			foreach (var vector in vectors)
			{
				sum += vector;
			}
			var average = new Vector2Int(sum.x / vectors.Count, sum.y / vectors.Count);
			return average;
		}

		public static int WalkDistance(Vector2Int vector1, Vector2Int vector2)
		{
			int xDistance = Math.Abs(vector1.x - vector2.x);
			int yDistance = Math.Abs(vector1.y - vector2.y);

			return xDistance > yDistance ? xDistance : yDistance;
		}

		public static bool IsOneStep(Vector2Int vector)
		{
			return Neighbours8(Vector2Int.zero).Contains(vector);
		}

		public static bool IsTwoSteps(Vector2Int vector)
		{
			return vector.x >= -2 && vector.x <= 2 && vector.y >= -2 && vector.y <= 2;
		}
	
		public static List<Vector2Int> Neighbours4(Vector2Int vector)
		{
			return new List<Vector2Int>(4)
			{
				vector + Vector2Int.up,
				vector + Vector2Int.left,
				vector + Vector2Int.down,
				vector + Vector2Int.right,
			};
		}

		public static List<Vector2Int> Neighbours8(Vector2Int vector)
		{
			return new List<Vector2Int>(8)
			{
				vector + Vector2Int.up,
				vector + Vector2Int.left,
				vector + Vector2Int.down,
				vector + Vector2Int.right,
				vector + Vector2Int.up + Vector2Int.left,
				vector + Vector2Int.up + Vector2Int.right,
				vector + Vector2Int.down + Vector2Int.left,
				vector + Vector2Int.down + Vector2Int.right,
			};
		}
	}
}