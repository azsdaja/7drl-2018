using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Libraries.EpPathFinding;
using Assets.Libraries.EpPathFinding.Grid;
using UnityEngine;

namespace Assets.Scripts.Pathfinding
{
	public class Pathfinder : IPathfinder
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly INaturalLineCalculator _naturalLineCalculator;
		private StaticGrid _navigationGrid;
		private readonly Vector2Int _positionOffset;

		public Pathfinder(IGridInfoProvider gridInfoProvider, INaturalLineCalculator naturalLineCalculator)
		{
			_gridInfoProvider = gridInfoProvider;
			_naturalLineCalculator = naturalLineCalculator;
			_positionOffset = _gridInfoProvider.MinPosition;
			//InitializeNavigationGrid();
		}

		public void InitializeNavigationGrid()
		{
			bool[][] walkabilityMatrix = CreateWalkabilityMatrix();
			_navigationGrid = new StaticGrid(_gridInfoProvider.XSize, _gridInfoProvider.YSize, walkabilityMatrix);
		}

		private bool[][] CreateWalkabilityMatrix()
		{
			var walkabilityMatrix = new bool[_gridInfoProvider.XSize][];
			var nodePool = new NodePool();
			for (int currentX = 0; currentX < _gridInfoProvider.XSize; currentX++)
			{
				walkabilityMatrix[currentX] = new bool[_gridInfoProvider.YSize];
				for (int currentY = 0; currentY < _gridInfoProvider.YSize; currentY++)
				{
					Vector2Int positionToCheckForWall = NonZeroBasedPosition(new Vector2Int(currentX, currentY));
					bool isWalkable = _gridInfoProvider.IsWalkable(positionToCheckForWall);
					walkabilityMatrix[currentX][currentY] = isWalkable;
					nodePool.SetNode(currentX, currentY, isWalkable);
				}
			}
			return walkabilityMatrix;
		}

		public List<Vector2Int> GetJumpPoints(Vector2Int startPoint, Vector2Int targetPoint)
		{
			IList<GridPos> resultPathList = GetJumpPointsFromJumpPointFinder(startPoint, targetPoint);

			if (!resultPathList.Any())
			{
				return null;
			}

			List<Vector2Int> resultJumpPointsInGrid = resultPathList.Select(NonZeroBasedPosition).ToList();

			IList<Vector2Int> naturalJumpPoints = _naturalLineCalculator.GetNaturalJumpPoints(resultJumpPointsInGrid);

			return naturalJumpPoints.ToList();
		}

		public int CalculateCityLength(IList<Vector2Int> jumpPoints)
		{
			int sum = 0;

			for (int jumpPointsIndex = 1; jumpPointsIndex < jumpPoints.Count - 1; jumpPointsIndex++)
			{
				int fragmentLength =
					Math.Abs(jumpPoints[jumpPointsIndex].x - jumpPoints[jumpPointsIndex - 1].x)
					+ Math.Abs(jumpPoints[jumpPointsIndex].y - jumpPoints[jumpPointsIndex - 1].y);
				sum += fragmentLength;
			}
			return sum;
		}

		private IList<GridPos> GetJumpPointsFromJumpPointFinder(Vector2Int startPoint, Vector2Int targetPoint)
		{
			if (!_gridInfoProvider.IsWalkable(targetPoint))
			{
				return new GridPos[0];
			}

			GridPos startPosition = ZeroBasedPosition(startPoint);
			GridPos targetPosition = ZeroBasedPosition(targetPoint);

			// todo: when iAllowEndNodeUnWalkable is set to false, it seems that search is performed anyway — it just finishes with failure. Fix!
			JumpPointParam jumpPointsParams 
				= new JumpPointParam(_navigationGrid, startPosition, targetPosition, iAllowEndNodeUnWalkable: false, iMode: HeuristicMode.MANHATTAN);
			
			// todo: note that resetting the grid causes all the nodes to be created again, 
			// which probably causes bad performance (5-30 ms for 30x50 grid).
			// Possible solutions: use DynamicGridWPool instead of StaticGrid; refactor the library; use other library.
			jumpPointsParams.Reset(startPosition, targetPosition);
			IList<GridPos> resultPathList = JumpPointFinder.FindPath(jumpPointsParams);
			if (!resultPathList.Any())
			{
				return new GridPos[0];
			}
			return resultPathList;
		}

		private GridPos ZeroBasedPosition(Vector2Int nonZeroBasedPosition)
		{
			return new GridPos(nonZeroBasedPosition.x - _positionOffset.x, nonZeroBasedPosition.y - _positionOffset.y);
		}

		private Vector2Int NonZeroBasedPosition(Vector2Int zeroBasedPosition)
		{
			return zeroBasedPosition + _positionOffset;
		}

		private Vector2Int NonZeroBasedPosition(GridPos epGridPosition)
		{
			return new Vector2Int(epGridPosition.x + _positionOffset.x, epGridPosition.y + _positionOffset.y);
		}
	}
}