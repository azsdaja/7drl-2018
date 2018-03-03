using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.FieldOfView;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	public class NaturalLineCalculator : INaturalLineCalculator
	{
		private readonly IBresenhamLineCreator _bresenhamLineCreator;

		public NaturalLineCalculator(IBresenhamLineCreator bresenhamLineCreator)
		{
			_bresenhamLineCreator = bresenhamLineCreator;
		}

		public IList<Vector2Int> GetFirstLongestNaturalLine(IList<Vector2Int> jumpPoints, Func<Vector2Int, bool> isWalkable)
		{
			if (jumpPoints.Count == 1)
			{
				return jumpPoints;
			}
			IList<Vector2Int> naturalJumpPoints = GetNaturalJumpPoints(jumpPoints);
			if (naturalJumpPoints.Count == 2)
			{
				return _bresenhamLineCreator.GetBresenhamLine(jumpPoints[0].x, jumpPoints[0].y, jumpPoints[1].x, jumpPoints[1].y, -1, isWalkable, false);
			}
			IList<Vector2Int> firstThreeNaturalJumpPoints = naturalJumpPoints.Take(3).ToList();
			Vector2Int firstJumpPoint = firstThreeNaturalJumpPoints[0];
			Vector2Int rangeCheckBeginning = firstThreeNaturalJumpPoints[1];
			Vector2Int rangeCheckEnd = firstThreeNaturalJumpPoints[2];
			IList<Vector2Int> rangeToCheck = // note that it's going from range end to range beginning
				_bresenhamLineCreator.GetBresenhamLine(rangeCheckEnd.x, rangeCheckEnd.y, rangeCheckBeginning.x, rangeCheckBeginning.y, -1,
					position => true);
			IList<Vector2Int> naturalWay = null;
			foreach (Vector2Int checkedPosition in rangeToCheck)
			{
				IList<Vector2Int> bresenhamLineToChecked = 
					_bresenhamLineCreator.GetBresenhamLine(firstJumpPoint.x, firstJumpPoint.y, checkedPosition.x, checkedPosition.y, -1, isWalkable, false);
				bool clearWayToThirdExists = bresenhamLineToChecked.Any() && bresenhamLineToChecked.Last() == checkedPosition;
				if (clearWayToThirdExists)
				{
					naturalWay = bresenhamLineToChecked;
					break;
				}
			}
			return naturalWay;
		}

		public IList<Vector2Int> GetFirstLongestNaturalLine(Vector2Int startNode, IList<Vector2Int> followingJumpPoints, Func<Vector2Int, bool> isWalkable)
		{
			return GetFirstLongestNaturalLine(new[] {startNode}.Union(followingJumpPoints).ToList(), isWalkable);
		}

		/// <summary>
		/// Usually the current JPS implementation creates too many jump points (many of them are aligned in one line).
		/// This function gives three first „natural” jump points (two or three), which means they don't form a single line.
		/// </summary>
		public IList<Vector2Int> GetNaturalJumpPoints(IList<Vector2Int> jumpPoints)
		{
			if (jumpPoints.Count <= 2)
			{
				return jumpPoints;
			}

			var result = new List<Vector2Int> {jumpPoints[0]};

			Vector2Int currentDirectionNormalized = Vector2IntUtilities.Normalized(jumpPoints[1] - jumpPoints[0]);
			for (int i = 2; i < jumpPoints.Count; i++) // we should start checking from current == third and previous == second
			{
				Vector2Int previousPointToCheck = jumpPoints[i - 1];
				Vector2Int currentPointToCheck = jumpPoints[i];
				Vector2Int currentDirection = Vector2IntUtilities.Normalized(currentPointToCheck - previousPointToCheck);

				if (currentDirection != currentDirectionNormalized) // change of direction implicates that the last jump point was natural
				{
					result.Add(previousPointToCheck);
					currentDirectionNormalized = currentDirection;
				}
			}

			result.Add(jumpPoints.Last());
			return result;
		}
	}
}