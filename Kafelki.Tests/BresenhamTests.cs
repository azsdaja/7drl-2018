﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.FieldOfView;
using Assets.Scripts.RNG;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests
{
	[TestFixture]
	public class BresenhamTests
	{
		[Test]
		public void shuffle()
		{
			var rng = new RandomNumberGenerator();
			var result = rng.Shuffle(new[]{ new Vector2Int(1, 1), new Vector2Int(2, 2), new Vector2Int(3, 3), new Vector2Int(4, 4) });
			foreach (var vector2Int in result)
			{
				Console.WriteLine(vector2Int);
			}
		}

		[Test]
		public void Cofanie()
		{
			var actorPosition = new Vector2Int(10, 10);
			var toEnemy = new Vector2Int(-1, -1);
			Vector2Int directionFromEnemy = Vector2IntUtilities.Normalized(toEnemy) * -1;
			IEnumerable<Vector2Int> positionsToStepBack = Vector2IntUtilities.GetCone(directionFromEnemy)
				.Select(coneVector => actorPosition + coneVector - directionFromEnemy);
			foreach (var vector2Int in positionsToStepBack)
			{
				Console.WriteLine(vector2Int);
			}
		}

		[Test]
		public void DwaDalej()
		{
			var position = new Vector2Int(3,10);
			foreach (var vector2Int in Vector2IntUtilities.Neighbours8(Vector2Int.zero)
				.Select(v => new Vector2Int(v.x * 2, v.y * 2))
				.Select(v => position + v))
			{
				Console.WriteLine(vector2Int);
			}
		}

		[Test]
		public void Pawel()
		{
			foreach (var direction in Vector2IntUtilities.Neighbours8(Vector2Int.zero))
			{
				Console.WriteLine(direction +", "+ ZRotationForLookAt2D(direction.x, direction.y));
			}
		}

		private static float ZRotationForLookAt2D(float x, float y)
		{
			var angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
			return angle - 45;
		}

		[Test]
		public void TwoLinesThereAndBackAreDifferent()
		{
			var bresenham = new BresenhamLineCreator();

			var from = new Vector2Int(0, 0);
			var to = new Vector2Int(2, 1);

			IList<Vector2Int> there = bresenham.GetBresenhamLine(from.x, from.y, to.x, to.y);
			IList<Vector2Int> back = bresenham.GetBresenhamLine(to.x, to.y, from.x, from.y);

			there.Should().NotBeEquivalentTo(back);
		}

		[Test]
		public void METHOD()
		{
			float x = 0.5f % 1;
			float y = 1.5f % 1;
		}
	}
}