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
	public class NaturalLineCalculatorTests
	{
		[Test]
		public void GetNaturalJumpPoints_OneJumpPoint_ReturnsCorrectResult()
		{
			var jumpPoints = new[] {new Vector2Int(0, 0)};
			var calculator = new NaturalLineCalculator(It.IsAny<IBresenhamLineCreator>());

			IList<Vector2Int> result = calculator.GetNaturalJumpPoints(jumpPoints);

			result.ShouldBeEquivalentTo(jumpPoints);
		}

		[Test]
		public void GetNaturalJumpPoints_TwoJumpPoints_ReturnsCorrectResult()
		{
			var jumpPoints = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0) };
			var calculator = new NaturalLineCalculator(It.IsAny<IBresenhamLineCreator>());

			IList<Vector2Int> result = calculator.GetNaturalJumpPoints(jumpPoints);

			result.ShouldBeEquivalentTo(jumpPoints);
		}

		[Test]
		public void GetNaturalJumpPoints_ThreeJumpPointsInLine_ReturnsCorrectResultWithFirstAndLast()
		{
			var jumpPoints = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
			var calculator = new NaturalLineCalculator(It.IsAny<IBresenhamLineCreator>());

			IList<Vector2Int> result = calculator.GetNaturalJumpPoints(jumpPoints);

			result.ShouldBeEquivalentTo(new[] { new Vector2Int(0, 0), new Vector2Int(2, 0) }, options => options.WithStrictOrdering());
		}

		[Test]
		public void GetNaturalJumpPoints_FiveJumpPointsInLine_ReturnsCorrectResultWithFirstAndLast()
		{
			var jumpPoints = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0) };
			var calculator = new NaturalLineCalculator(It.IsAny<IBresenhamLineCreator>());

			IList<Vector2Int> result = calculator.GetNaturalJumpPoints(jumpPoints);

			result.ShouldBeEquivalentTo(new[] { new Vector2Int(0, 0), new Vector2Int(4, 0) }, options => options.WithStrictOrdering());
		}

		[Test]
		public void GetNaturalJumpPoints_ThreeJumpPointsInLineAndThenOneDiagonal_ReturnsCorrectResultWithFirstAndSecondLastAndLast()
		{
			var jumpPoints = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 1)};
			var calculator = new NaturalLineCalculator(It.IsAny<IBresenhamLineCreator>());

			IList<Vector2Int> result = calculator.GetNaturalJumpPoints(jumpPoints);

			result.ShouldBeEquivalentTo(new[] { new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(3, 1) }, 
				options => options.WithStrictOrdering());
		}

		[Test]
		public void GetNaturalJumpPoints_MultipleJumpPoints_ReturnsCorrectResult()
		{
			// input:
			//......*
			//...***.
			//***....

			// expected:
			//......*
			//...*.*.
			//*.*....
			
			var jumpPoints = new[]
			{
	new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 1), new Vector2Int(4, 1),new Vector2Int(5, 1),new Vector2Int(6, 2),
			};
			var calculator = new NaturalLineCalculator(It.IsAny<IBresenhamLineCreator>());

			IList<Vector2Int> result = calculator.GetNaturalJumpPoints(jumpPoints);

			result.ShouldBeEquivalentTo(
	new[] { new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(3, 1), new Vector2Int(5, 1), new Vector2Int(6, 2) },
				options => options.WithStrictOrdering());
		}

		[Test]
		public void GetFirstLongestNaturalLine_ReturnsCorrectResult()
		{
			// For:
			// ..2****3
			// .*..####
			// 1....... (where 1 is (0,0), 2 is (2,2) and 3 is (7,2))
			// 
			// Should give:
			// ....**..
			// ..**####
			// **......
			var calculator = new NaturalLineCalculator(new BresenhamLineCreator());

			Func<Vector2Int, bool> isWalkable = position =>
			{
				if (position == new Vector2Int(4, 1) || position == new Vector2Int(5, 1)
				    || position == new Vector2Int(6, 1) || position == new Vector2Int(7, 1))
					return false;
				return true;
			};

			IList<Vector2Int> result 
				= calculator.GetFirstLongestNaturalLine(new[] {new Vector2Int(0, 0), new Vector2Int(2, 2), new Vector2Int(7, 2)}, isWalkable);

			result.ShouldBeEquivalentTo(new[]
			{
				new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(2,1),new Vector2Int(3,1),new Vector2Int(4,2),new Vector2Int(5,2),
			}, options => options.WithStrictOrdering());
		}
	}
}