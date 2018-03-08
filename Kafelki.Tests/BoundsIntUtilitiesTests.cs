
using Assets.Scripts.CSharpUtilities;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests
{
	[TestFixture]
	public class BoundsIntUtilitiesTests
	{
		[Test]
		public void Center_ReturnsCenter()
		{
			var bounds = new BoundsInt(-2,-2,0,4,4,1);

			Vector2Int center = BoundsIntUtilities.Center(bounds);

			center.Should().Be(Vector2Int.zero);
		}	

		[Test]
		public void Center_ReturnsCenter_Case2()
		{
			var bounds = new BoundsInt(0,0,0,8,6,1);

			Vector2Int center = BoundsIntUtilities.Center(bounds);

			center.Should().Be(new Vector2Int(4,3));
		}		
	}
}