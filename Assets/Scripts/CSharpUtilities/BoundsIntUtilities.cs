using UnityEngine;

namespace Assets.Scripts.CSharpUtilities
{
	public static class BoundsIntUtilities
	{
		public static Vector2Int Center(BoundsInt bounds)
		{
			return new Vector2Int((bounds.xMin + bounds.xMax) / 2, (bounds.yMin + bounds.yMax) / 2);
		}
	}
}