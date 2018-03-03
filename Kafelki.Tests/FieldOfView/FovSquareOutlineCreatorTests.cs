using System.Collections.Generic;
using Assets.Scripts.FieldOfView;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.FieldOfView
{
	[TestFixture]
	public class FovSquareOutlineCreatorTests
	{
		[Test]
		public void CreateSquareOutline_RayLengthIsZero_ReturnsOnlyCenter()
		{
			var creator = new FovSquareOutlineCreator();
			var expectedOutlinePoints = new List<Vector2Int>{new Vector2Int(0,0)};

			HashSet<Vector2Int> outline = creator.CreateSquareOutline(fovCenter: new Vector2Int(0, 0), rayLength: 0);

			outline.ShouldBeEquivalentTo(expectedOutlinePoints);
		}

		[Test]
		public void CreateSquareOutline_RayLengthIsOne_ReturnsAllNeighboursOfCenter()
		{
			var creator = new FovSquareOutlineCreator();
			var expectedOutlinePoints = new List<Vector2Int>
			{
				new Vector2Int(-1,-1),
				new Vector2Int(0,-1),
				new Vector2Int(1,-1),
				new Vector2Int(-1,0),
				new Vector2Int(1,0),
				new Vector2Int(-1,1),
				new Vector2Int(0,1),
				new Vector2Int(1,1),
			};

			HashSet<Vector2Int> outline = creator.CreateSquareOutline(fovCenter: new Vector2Int(0, 0), rayLength: 1);

			outline.ShouldBeEquivalentTo(expectedOutlinePoints);
		}

		[TestCase(1,0)]
		[TestCase(0,1)]
		[TestCase(-15,24)]
		public void CreateSquareOutline_RayLengthIsOneAndCenterIsNotAtZero_ReturnsAllNeighboursOfCenter(int offsetX, int offsetY)
		{
			var offset = new Vector2Int(offsetX, offsetY);
			var creator = new FovSquareOutlineCreator();
			var expectedOutlinePoints = new List<Vector2Int>
			{
				new Vector2Int(-1,-1) + offset,
				new Vector2Int(0,-1) + offset,
				new Vector2Int(1,-1) + offset,
				new Vector2Int(-1,0) + offset,
				new Vector2Int(1,0) + offset,
				new Vector2Int(-1,1) + offset,
				new Vector2Int(0,1) + offset,
				new Vector2Int(1,1) + offset
			};

			HashSet<Vector2Int> outline = creator.CreateSquareOutline(fovCenter: offset, rayLength: 1);

			outline.ShouldBeEquivalentTo(expectedOutlinePoints);
		}
	}
}