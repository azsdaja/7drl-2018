using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	public class Navigator : INavigator
	{
		private readonly IPathfinder _pathfinder;
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly INaturalLineCalculator _naturalLineCreator;

		public Navigator(IPathfinder pathfinder, IGridInfoProvider gridInfoProvider, INaturalLineCalculator naturalLineCreator)
		{
			_pathfinder = pathfinder;
			_gridInfoProvider = gridInfoProvider;
			_naturalLineCreator = naturalLineCreator;
		}

		public Vector2Int? ResolveNextStep(ActorData actorData)
		{
			if (actorData.NavigationData.Destination.Value == actorData.LogicalPosition)
			{
				throw new InvalidOperationException("Actor destination is same as his position: " + actorData.LogicalPosition);
			}
			NavigationData navigationData = actorData.NavigationData;
			var currentPosition = actorData.LogicalPosition;

			bool hasPathToTarget = navigationData.PathToFollow != null;
			if (!hasPathToTarget)
			{
				List<Vector2Int> jumpPointsToTarget = GetPath(currentPosition, navigationData.Destination.Value);
				if (jumpPointsToTarget == null)
				{
					return null;
				}
				navigationData.PathToFollow = jumpPointsToTarget;
			}

			bool isAtNextNode = currentPosition == navigationData.NextNode;
			if (isAtNextNode)
			{
				navigationData.PathToFollow.RemoveAt(0);
			}
			bool isOutOfPath = navigationData.StepsToNextNode == null || !navigationData.StepsToNextNode.Any()
			                   || (navigationData.StepsToNextNode.Count > 0 && currentPosition != navigationData.StepsToNextNode.Peek());
			if (isAtNextNode || isOutOfPath)
			{
				// walking from jump point to jump point generates unnatural movement — thus we generate a natural straight line for the 
				// player to follow. It's symmetrical (in terms of walking cost) to the line connecting the jump points and it usually omits
				// the next jump point, so we have to reassign NextNode.
				IList<Vector2Int> naturalLineToWalk = _naturalLineCreator.GetFirstLongestNaturalLine(
					currentPosition, navigationData.PathToFollow, _gridInfoProvider.IsWalkable);

				if (naturalLineToWalk == null) // there may be no natural line when actor has been moved (pushed, displaced etc.) 
				{
					return RecalculateNextStep(actorData, navigationData);
				}
				navigationData.NextNode = naturalLineToWalk.Last();
				if (navigationData.PathToFollow.Count > 1 && navigationData.NextNode == navigationData.PathToFollow[1])
				{
					// if we can reach the second jump point in the list by walking the natural line, 
					// we should remove the first node because it's redundant
					navigationData.PathToFollow.RemoveAt(0);
				}
				navigationData.StepsToNextNode = new Stack<Vector2Int>(naturalLineToWalk.Reverse());
			}

			navigationData.StepsToNextNode.Pop();
			Vector2Int nextStep = navigationData.StepsToNextNode.Peek();
			return nextStep;
		}

		public List<Vector2Int> GetPath(Vector2Int startPosition, Vector2Int targetPosition)
		{
			List<Vector2Int> allJumpPoints = _pathfinder.GetJumpPoints(startPosition, targetPosition);
			if (allJumpPoints == null)
			{
				return null;
			}
			List<Vector2Int> jumpPointsToTarget = allJumpPoints.Skip(1).ToList();
			return jumpPointsToTarget;
		}

		private Vector2Int? RecalculateNextStep(ActorData actorData, NavigationData navigationData)
		{
			actorData.NavigationData = new NavigationData {Destination = navigationData.Destination};
			return ResolveNextStep(actorData);
		}
	}
}