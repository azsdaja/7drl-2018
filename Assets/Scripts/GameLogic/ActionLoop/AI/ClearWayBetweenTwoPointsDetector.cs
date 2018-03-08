using System;
using System.Linq;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	public class ClearWayBetweenTwoPointsDetector : IClearWayBetweenTwoPointsDetector
	{
		private readonly IEntityDetector _entityDetector;
		private readonly IGridInfoProvider _gridInfoProvider;

		public ClearWayBetweenTwoPointsDetector(IEntityDetector entityDetector, IGridInfoProvider gridInfoProvider)
		{
			_entityDetector = entityDetector;
			_gridInfoProvider = gridInfoProvider;
		}

		/// <summary>
		/// It's assuming that from @from to to there are exactly two steps!
		/// </summary>
		public bool ClearWayExists(Vector2Int @from, Vector2Int to)
		{
			Func<float, bool> isIntegral = val => Math.Abs(val % 1) < .001f;

			float middleX = ((float)@from.x + to.x) / 2;
			float middleY = ((float)@from.y + to.y) / 2;

			int[] xsToCheck = isIntegral(middleX) ? new[] { (int)middleX } : new[] { Mathf.CeilToInt(middleX), Mathf.FloorToInt(middleX) };
			int[] ysToCheck = isIntegral(middleY) ? new[] { (int)middleY } : new[] { Mathf.CeilToInt(middleY), Mathf.FloorToInt(middleY) };

			foreach (int x in xsToCheck)
			{
				foreach (int y in ysToCheck)
				{
					bool actorIsBlockingWay = _entityDetector.DetectActors(new Vector2Int(x, y)).Any();
					bool wayIsWalkable = _gridInfoProvider.IsWalkable(new Vector2Int(x, y));
					if (wayIsWalkable && !actorIsBlockingWay)
					{
						return true;
					}
				}
			}

			return false;
		}

	}
}