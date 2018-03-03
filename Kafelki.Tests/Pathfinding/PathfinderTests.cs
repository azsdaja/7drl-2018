using System;
using System.Collections.Generic;
using Assets.Scripts.FieldOfView;
using Assets.Scripts.Pathfinding;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.Pathfinding
{
	[TestFixture]
	public class PathfinderTests
	{
		[TestCase(-3,-3)]
		[TestCase(0,0)]
		[TestCase(3,3)]
		public void GetJumpPoints_IntegrationTest_AllGridIsWalkable_ReturnsCorrectPath(int xOffset, int yOffset)
		{
			/* Illustration:
			  ........
			  ...jt...
			  ..s.....
			  ........*/

			Vector2Int minPositionInUnityGrid = new Vector2Int(-10, -10);
			int unityGridXSize = 20;
			int unityGridYSize = 20;
			Vector2Int offset = new Vector2Int(xOffset, yOffset);
			var startPosition = new Vector2Int(0,0) + offset;
			var targetPosition = new Vector2Int(2, 1) + offset;
			var expectedMiddleJumpPoint = new Vector2Int(1, 1) + offset;
			var gridInfoProvider = Mock.Of<IGridInfoProvider>(
				f => f.IsWalkable(It.IsAny<Vector2Int>()) == true
				&& f.MinPosition == minPositionInUnityGrid
				&& f.XSize == unityGridXSize
				&& f.YSize == unityGridYSize);
			var pathfinder = new Pathfinder(gridInfoProvider, new NaturalLineCalculator(new BresenhamLineCreator()));

			IList<Vector2Int> jumpPoints = pathfinder.GetJumpPoints(startPosition, targetPosition);

			jumpPoints.Count.Should().Be(3);
			jumpPoints[0].Should().Be(startPosition);
			jumpPoints[1].Should().Be(expectedMiddleJumpPoint);
			jumpPoints[2].Should().Be(targetPosition);
		}

		[Test]
		public void GetJumpPoints_IntegrationTest_WallIsBlockingWayToTarget_ReturnsCorrectPath()
		{
			/* Illustration:
			  ........
			  ...#....
			  ..s#t...
			  ...j....*/

			Vector2Int minPositionInUnityGrid = new Vector2Int(-10, -10);
			int unityGridXSize = 20;
			int unityGridYSize = 20;
			var startPosition = new Vector2Int(0,0);
			var targetPosition = new Vector2Int(2,0);
			var expectedMiddleJumpPoint = new Vector2Int(1,-1);
			Func<Vector2Int, bool> isWalkable = position =>
			{
				if (position == new Vector2Int(1, 0) || position == new Vector2Int(1, 1))
					return false;
				return true;
			};
			var gridInfoProvider = Mock.Of<IGridInfoProvider>(
				f => f.IsWalkable(It.Is<Vector2Int>(v => isWalkable(v))) == true
				&& f.IsWalkable(It.Is<Vector2Int>(v => !isWalkable(v))) == false
				&& f. MinPosition == minPositionInUnityGrid
				&& f.XSize == unityGridXSize
				&& f.YSize == unityGridYSize);
			var pathfinder = new Pathfinder(gridInfoProvider, new NaturalLineCalculator(new BresenhamLineCreator()));

			IList<Vector2Int> jumpPoints = pathfinder.GetJumpPoints(startPosition, targetPosition);

			jumpPoints.Count.Should().Be(3);
			jumpPoints[0].Should().Be(startPosition);
			jumpPoints[1].Should().Be(expectedMiddleJumpPoint);
			jumpPoints[2].Should().Be(targetPosition);
		}

		[Test]
		public void GetJumpPoints_IntegrationTest_ThereIsNoPathToTarget_ReturnsNull()
		{
			/* Illustration:
			  ........
			  .###....
			  .#s#t...
			  .###....*/

			Vector2Int minPositionInUnityGrid = new Vector2Int(-10, -10);
			int unityGridXSize = 20;
			int unityGridYSize = 20;
			var startPosition = new Vector2Int(0,0);
			var targetPosition = new Vector2Int(2,0);
			Func<Vector2Int, bool> isWalkable = position =>
			{
				if ( // surrounding start point with walls
					position == new Vector2Int(-1, 1) || position == new Vector2Int(0, 1) || position == new Vector2Int(1, 1)
					|| position == new Vector2Int(-1, 0) || position == new Vector2Int(1, 0)
					|| position == new Vector2Int(-1, -1) || position == new Vector2Int(0, -1) || position == new Vector2Int(1, -1))
					return false;
				return true;
			};
			var gridInfoProvider = Mock.Of<IGridInfoProvider>(
				f => f.IsWalkable(It.Is<Vector2Int>(v => isWalkable(v))) == true
				&& f.IsWalkable(It.Is<Vector2Int>(v => !isWalkable(v))) == false
				&& f. MinPosition == minPositionInUnityGrid
				&& f.XSize == unityGridXSize
				&& f.YSize == unityGridYSize);
			var pathfinder = new Pathfinder(gridInfoProvider, new NaturalLineCalculator(new BresenhamLineCreator()));

			IList<Vector2Int> jumpPoints = pathfinder.GetJumpPoints(startPosition, targetPosition);

			jumpPoints.Should().BeNull();
		}
	}
}