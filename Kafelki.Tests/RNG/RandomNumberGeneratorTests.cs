using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.RNG;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.RNG
{
	[TestFixture]
	public class RandomNumberGeneratorTests
	{
		[Test]
		public void NextPosition_ReturnsPositionsWithinBoundsAndAllPossiblePositionsAreGeneratedEventually()
		{
			var rng = new RandomNumberGenerator(9438);
			var results = new List<Vector2Int>();
			var bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
			var possibleResults = new[]
			{
				new Vector2Int(-1, -1), new Vector2Int(+0, -1), new Vector2Int(+1, -1), new Vector2Int(+2, -1),
				new Vector2Int(-1, +0), new Vector2Int(+0, +0), new Vector2Int(+1, +0), new Vector2Int(+2, +0),
				new Vector2Int(-1, +1), new Vector2Int(+0, +1), new Vector2Int(+1, +1), new Vector2Int(+2, +1),
				new Vector2Int(-1, +2), new Vector2Int(+0, +2), new Vector2Int(+1, +2), new Vector2Int(+2, +2),
			};

			for (int i = 0; i < 100; i++)
			{
				var result = rng.NextPosition(bounds);
				results.Add(result);
			}

			foreach (Vector2Int result in results)
			{
				possibleResults.Should().Contain(result);
			}
			foreach (Vector2Int possibleResult in possibleResults)
			{
				results.Should().Contain(possibleResult);
			}
		}

		[TestCase(0,0)]
		[TestCase(3,0)]
		[TestCase(0,5)]
		[TestCase(-12,21)]
		public void BiasedPosition_WhenRadiusIs0_ReturnsInputPosition(int x, int y)
		{
			var rng = new RandomNumberGenerator(2131235);
			var testedVector = new Vector2Int(x, y);
			var results = new List<Vector2Int>();

			for (int i = 0; i < 100; i++)
			{
				var newResult = rng.BiasedPosition(testedVector, 0);
				results.Add(newResult);
			}

			foreach (Vector2Int result in results)
			{
				result.Should().Be(testedVector);
			}
		}

		[TestCase(0, 0)]
		[TestCase(3, 0)]
		[TestCase(0, 5)]
		[TestCase(-12, 21)]
		public void BiasedPosition_WhenRadiusIs1_ReturnsInputPositionOrItsNeighboursAndAllNeighboursAreEventuallyReturned(int x, int y)
		{
			var rng = new RandomNumberGenerator(2131235);
			var testedVector = new Vector2Int(x, y);
			List<Vector2Int> possibleResults = new[] {testedVector}.Union(Vector2IntUtilities.Neighbours4(testedVector)).ToList();
			var results = new List<Vector2Int>();

			for (int i = 0; i < 100; i++)
			{
				var newResult = rng.BiasedPosition(testedVector, 1);
				results.Add(newResult);
			}

			foreach (Vector2Int possibleResult in possibleResults)
			{
				results.Should().Contain(possibleResult);
			}
			foreach (Vector2Int result in results)
			{
				possibleResults.Should().Contain(result);
			}
		}

		[TestCase(0, 0)]
		[TestCase(3, 0)]
		[TestCase(0, 5)]
		[TestCase(-12, 21)]
		public void BiasedPosition_WhenRadiusIs5_ReturnsPositionsWithin5RangeFromInputPosition(int x, int y)
		{
			var rng = new RandomNumberGenerator(2131235);
			var testedVector = new Vector2Int(x, y);
			var results = new List<Vector2Int>();

			for (int i = 0; i < 100; i++)
			{
				var newResult = rng.BiasedPosition(testedVector, 1);
				results.Add(newResult);
			}

			foreach (Vector2Int result in results)
			{
				Vector2Int.Distance(testedVector, result).Should().BeLessOrEqualTo(5);
			}
		}
	}
}