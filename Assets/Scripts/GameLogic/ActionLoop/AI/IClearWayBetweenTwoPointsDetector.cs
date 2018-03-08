using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	public interface IClearWayBetweenTwoPointsDetector
	{
		/// <summary>
		/// It's assuming that from @from to to there are exactly two steps!
		/// </summary>
		bool ClearWayExists(Vector2Int @from, Vector2Int to);
	}
}