using System;
using System.Linq;
using Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration;
using Assets.Scripts.RNG;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests
{
	[TestFixture]
	public class DungeonGeneratorTests
	{
		[Test]
		public void GeneratorDumpDungeonToConsole()
		{
			var rng = new RandomNumberGenerator();
			var generator = new Dungeon(rng, Console.WriteLine);

			generator.CreateDungeon(Vector3Int.zero, 60, 60, 25);

			generator.ShowDungeon();
		}

		[Test]
		public void GeneratorWithOffsettingAndNonZeroMinPoint_MinAndMaxPointsAreGottenWithoutException()
		{
			var rng = new RandomNumberGenerator();
			var generator = new Dungeon(rng, Console.WriteLine);

			generator.CreateDungeon(new Vector3Int(-1, -1, 0), 60, 60, 25);

			// correct bounds
			generator.GetCellType(-1, -1);
			generator.GetCellType(58, 58);

			// out of range
			Action action = () => generator.GetCellType(-2, -2);
			Action action2 = () => generator.GetCellType(59, 59);
			action.ShouldThrow<IndexOutOfRangeException>();
			action2.ShouldThrow<IndexOutOfRangeException>();
		}

		[Test]
		public void TestingMinAndMaxWithAggregate()
		{
			var points = new Vector2Int[]
				{new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),};
			var min = points.Aggregate((previous, current) => previous.x + previous.y < current.x + current.y ? previous : current);
			var max = points.Aggregate((previous, current) => previous.x + previous.y > current.x + current.y ? previous : current);
			min.Should().Be(new Vector2Int(0, -1));
			max.Should().Be(new Vector2Int(1, 0));
		}
	}
}