using UnityEngine;

namespace Assets.Scripts.CSharpUtilities
{
	public static class Vector2IntExtensions
	{
		public static Vector3Int ToVector3Int(this Vector2Int vector)
		{
			return new Vector3Int(vector.x, vector.y, 0);
		}
	}
}
