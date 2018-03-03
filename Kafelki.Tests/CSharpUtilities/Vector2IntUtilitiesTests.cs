using Assets.Scripts.CSharpUtilities;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.CSharpUtilities
{
	[TestFixture]
	public class Vector2IntUtilitiesTests
	{
		[TestCase(0,0, 0,0)]
		[TestCase(0,1, 0,0)]
		[TestCase(1,0, 1,0)]
		[TestCase(1,1, 1,0)]
		[TestCase(5,1, 1,0)]
		[TestCase(-1,0, -1,0)]
		[TestCase(-3,-5, -1,0)]
		public void SnapToXAxisNormalized_ReturnsCorrectResult(int x, int y, int expectedX, int expectedY)
		{
			Vector2Int result = Vector2IntUtilities.SnapToXAxisNormalized(new Vector2Int(x, y));

			result.x.Should().Be(expectedX);
			result.y.Should().Be(expectedY);
		}	

		[TestCase(0,0, 0,0)]
		[TestCase(0,1, 0,1)]
		[TestCase(1,0, 0,0)]
		[TestCase(1,1, 0,1)]
		[TestCase(5,1, 0,1)]
		[TestCase(-1,-1, 0,-1)]
		[TestCase(-3,-5, 0,-1)]
		public void SnapToYAxisNormalized_ReturnsCorrectResult(int x, int y, int expectedX, int expectedY)
		{
			Vector2Int result = Vector2IntUtilities.SnapToYAxisNormalized(new Vector2Int(x, y));

			result.x.Should().Be(expectedX);
			result.y.Should().Be(expectedY);
		}

		[Test]
		public void Average_CalculatesCorrectAverage()
		{
			Vector2Int result = Vector2IntUtilities.Average(new[] {new Vector2Int(-5, 0), new Vector2Int(0, 3), new Vector2Int(20, 0)});

			result.Should().Be(new Vector2Int(5, 1));
		}

		[TestCase(0,0, 0,0, 0)]
		[TestCase(0,0, 1,0, 1)]
		[TestCase(0,0, 1,0, 1)]
		[TestCase(0,0, 2,0, 2)]
		[TestCase(0,0, 1,1, 1)]
		[TestCase(0,0, 3,3, 3)]
		[TestCase(0,0, 2,1, 2)]
		[TestCase(0,0, 5,3, 5)]
		[TestCase(-5,-5, -3,-3, 2)]
		public void WalkDistance_ReturnsCorrectWalkDistance(int x1, int y1, int x2, int y2, int expectedDistance)
		{
			var start = new Vector2Int(x1, y1);
			var target = new Vector2Int(x2, y2);

			int result = Vector2IntUtilities.WalkDistance(start, target);

			result.Should().Be(expectedDistance);
		}
	}
}