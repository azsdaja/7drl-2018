using System;
using Assets.Scripts.Scent;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Kafelki.Tests.Scent
{
	[TestFixture]
	public class PathScentScoreCalculatorTests
	{
		[Test]
		public void Calculate_NullInput_ThrowsArgumentNullException()
		{
			var scentFragmentDistanceCalculatorMock = new Mock<IScentFragmentDistanceCalculator>();
			var calculator = new PathScentScoreCalculator(scentFragmentDistanceCalculatorMock.Object);
			
			Action action = () => calculator.Calculate(null);

			action.ShouldThrow<ArgumentNullException>();
		}

		[TestCase(0)]
		[TestCase(1)]
		public void Calculate_LessThanTwoJumpPoints_ThrowsArgumentException(int numberOfJumpPoints)
		{
			var scentFragmentDistanceCalculatorMock = new Mock<IScentFragmentDistanceCalculator>();
			var calculator = new PathScentScoreCalculator(scentFragmentDistanceCalculatorMock.Object);
			var jumpPoints = new Vector2Int[numberOfJumpPoints];

			Action action = () => calculator.Calculate(jumpPoints);

			action.ShouldThrow<ArgumentException>();
		}
		
		[Test]
		public void SumOfDistances_ReturnsSumOfFragmentDistancesFromScentFragmentDistanceCalculator()
		{
			var scentFragmentDistanceCalculatorMock = new Mock<IScentFragmentDistanceCalculator>();
			var firstPathJumpPoint = new Vector2Int(1,0);
			var secondPathJumpPoint = new Vector2Int(2,0);
			var thirdPathJumpPoint = new Vector2Int(3,0);
			scentFragmentDistanceCalculatorMock.Setup(c => c.CalculateFragmentDistance(firstPathJumpPoint, secondPathJumpPoint)).Returns(10f);
			scentFragmentDistanceCalculatorMock.Setup(c => c.CalculateFragmentDistance(secondPathJumpPoint, thirdPathJumpPoint)).Returns(20f);
			var calculator = new PathScentScoreCalculator(scentFragmentDistanceCalculatorMock.Object);
			
			float sumOfDistances = calculator.SumOfDistances(new[]{firstPathJumpPoint, secondPathJumpPoint, thirdPathJumpPoint});

			sumOfDistances.Should().Be(30f);
		}
	}
}