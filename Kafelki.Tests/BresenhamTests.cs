using System.Collections.Generic;
using Assets.Scripts.FieldOfView;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests
{
	[TestFixture]
	public class BresenhamTests
	{
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