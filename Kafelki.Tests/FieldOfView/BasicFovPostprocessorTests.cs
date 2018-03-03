using System;
using System.Collections.Generic;
using Assets.Scripts.FieldOfView;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.FieldOfView
{
	[TestFixture]
	public class BasicFovPostprocessorTests
	{
		[TestCase(0,0)]
		[TestCase(1,0)]
		[TestCase(0,1)]
		[TestCase(1,1)]
		[TestCase(-15,292)]
		public void GetBehindnessVectors_ReturnsCorrectVectors(int offsetX, int offsetY)
		{
			var postprocessor = new BasicFovPostprocessor();
			var offset = new Vector2Int(offsetX, offsetY);
			var currentPosition = new Vector2Int(1,1) + offset;
			var squareCenter = new Vector2Int(0,0) + offset;

			IEnumerable<Vector2Int> result = postprocessor.GetBehindnessVectors(currentPosition, squareCenter);

			result.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(1,0), new Vector2Int(0,1) });
		}

		[Test]
		public void GetBehindnessVectors_PositionHasNonZeroComponents_ReturnsCorrectVectors()
		{
			var postprocessor = new BasicFovPostprocessor();
			var positionPositivePositive = new Vector2Int(12,30);
			var positionNegativePositive = new Vector2Int(-5,14);
			var positionPositiveNegative = new Vector2Int(11,-1);
			var positionNegativeNegative = new Vector2Int(-5,-5);
			var squareCenter = new Vector2Int(0,0);

			IEnumerable<Vector2Int> resultPosPos = postprocessor.GetBehindnessVectors(positionPositivePositive, squareCenter);
			IEnumerable<Vector2Int> resultNegPos = postprocessor.GetBehindnessVectors(positionNegativePositive, squareCenter);
			IEnumerable<Vector2Int> resultPosNeg = postprocessor.GetBehindnessVectors(positionPositiveNegative, squareCenter);
			IEnumerable<Vector2Int> resultNegNeg = postprocessor.GetBehindnessVectors(positionNegativeNegative, squareCenter);

			resultPosPos.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(1,0), new Vector2Int(0,1) });
			resultNegPos.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(-1,0), new Vector2Int(0,1) });
			resultPosNeg.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(1,0), new Vector2Int(0,-1) });
			resultNegNeg.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(-1,0), new Vector2Int(0,-1) });
		}

		[Test]
		public void GetBehindnessVectors_PositionHasOneZeroComponent_ReturnsCorrectVectors()
		{
			var postprocessor = new BasicFovPostprocessor();
			var positionPositiveZero = new Vector2Int(12,0);
			var positionZeroPositive = new Vector2Int(0, 14);
			var positionNegativeZero = new Vector2Int(-7,0);
			var positionZeroNegative = new Vector2Int(0, -5);
			var squareCenter = new Vector2Int(0,0);

			IEnumerable<Vector2Int> resultPosZero = postprocessor.GetBehindnessVectors(positionPositiveZero, squareCenter);
			IEnumerable<Vector2Int> resultZeroPos = postprocessor.GetBehindnessVectors(positionZeroPositive, squareCenter);
			IEnumerable<Vector2Int> resultNegZero = postprocessor.GetBehindnessVectors(positionNegativeZero, squareCenter);
			IEnumerable<Vector2Int> resultZeroNeg = postprocessor.GetBehindnessVectors(positionZeroNegative, squareCenter);

			resultPosZero.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(1,0)  });
			resultZeroPos.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(0,1)  });
			resultNegZero.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(-1,0)  });
			resultZeroNeg.ShouldBeEquivalentTo(new List<Vector2Int>{new Vector2Int(0,-1) });
		}

		[Test]
		public void PostprocessBasicFov_CorridorIsVisibleButSomeWallsNot_ReturnsWallsInVisibleSet()
		{
			/*
			 * Illustration (p = square center, i.e. viewer; dot = visible floor, W = visible wall, w = not visible wall):
			 * 
			 * WWwWWW
			 * p.....
			 * WWWwWW
			 * 
			 */

			var postprocessor = new BasicFovPostprocessor();
			Func<Vector2Int, bool> isWalkable = position => position.y == 0; // see picture, all tiles except those with y=0 are walls.
			var visibleBeforePostprocessing = new HashSet<Vector2Int>
			{
				new Vector2Int(0, 1), new Vector2Int(1, 1),                       new Vector2Int(3, 1), new Vector2Int(4 ,1), new Vector2Int(5 ,1), 
				new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4 ,0), new Vector2Int(5 ,0), 
				new Vector2Int(0,-1), new Vector2Int(1, -1),new Vector2Int(2, -1),                      new Vector2Int(4 ,-1),new Vector2Int(5 ,-1), 
			};

			IEnumerable<Vector2Int> postprocessingResult = postprocessor.PostprocessBasicFov(visibleBeforePostprocessing, new Vector2Int(0, 0), 5, isWalkable);

			postprocessingResult.ShouldBeEquivalentTo(new[]{new Vector2Int(2,1), new Vector2Int(3,-1) });
		}

		[Test]
		public void PostprocessBasicFov_CorridorIsVisibleButSomeWallsAreNot_WallsOutsideOfVisibilityRangeAreNotVisible()
		{
			/*
			 * Illustration (p = square center, i.e. viewer; dot = visible floor, W = visible wall, w = not visible wall):
			 * 
			 * WWwWWw
			 * p.....
			 * WWWWWw
			 * 
			 */

			var postprocessor = new BasicFovPostprocessor();
			Func<Vector2Int, bool> isWalkable = position => position.y == 0; // see picture, all tiles except those with y=0 are walls.
			var visibleBeforePostprocessing = new HashSet<Vector2Int>
			{
				new Vector2Int(0, 1), new Vector2Int(1, 1),                       new Vector2Int(3, 1), new Vector2Int(4 ,1), 
				new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4 ,0), new Vector2Int(5 ,0),
				new Vector2Int(0,-1), new Vector2Int(1, -1),new Vector2Int(2, -1),new Vector2Int(3, -1),new Vector2Int(4 ,-1),
			};
			int rayLength = 5; // not enough to reach the walls that are most to the right

			IEnumerable<Vector2Int> postprocessingResult = 
				postprocessor.PostprocessBasicFov(visibleBeforePostprocessing, new Vector2Int(0, 0), rayLength, isWalkable);

			postprocessingResult.Should().NotContain(new Vector2Int(5,1));
			postprocessingResult.Should().NotContain(new Vector2Int(5,-1));
			postprocessingResult.ShouldBeEquivalentTo(new[]{ new Vector2Int(2, 1)});
		}
	}
}