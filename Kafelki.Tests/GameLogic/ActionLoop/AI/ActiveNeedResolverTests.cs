using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.ActionLoop.AI;
using FluentAssertions;
using NUnit.Framework;

namespace Kafelki.Tests.GameLogic.ActionLoop.AI
{
	[TestFixture]
	public class ActiveNeedResolverTests
	{
		[TestCase(NeedType.Hunger)]
		[TestCase(NeedType.Rest)]
		public void ResolveActiveNeed_NeedDataIsUninitialized_BiggestNeedIsNonDefault_ReturnsBiggestNeed(NeedType biggestNeed)
		{
			var resolver = new ActiveNeedResolver();
			var needData = new NeedData(new Dictionary<NeedType, float> { { biggestNeed, 0.0f } }, null);
			needData.ModifySatisfaction(biggestNeed, -1f);

			NeedType result = resolver.ResolveActiveNeed(needData);

			result.Should().Be(biggestNeed);
		}

		[TestCase(NeedType.Care)]
		[TestCase(NeedType.Rest)]
		public void ResolveActiveNeed_CurrentNeedIsBiggest_ReturnsCurrentNeed(NeedType currentNeed)
		{
			var resolver = new ActiveNeedResolver();
			var needData = new NeedData(new Dictionary<NeedType, float>{{currentNeed, 0f}}, null);
			needData.ModifySatisfaction(currentNeed, -1f);

			NeedType result = resolver.ResolveActiveNeed(needData);

			result.Should().Be(currentNeed);
		}

		[TestCase(NeedType.Care, NeedType.Rest)]
		[TestCase(NeedType.Rest, NeedType.Hunger)]
		public void ResolveActiveNeed_BiggestAndCurrentNeedDifferenceDoesNotExceedThreshold_ReturnsCurrentNeed(NeedType currentNeed, NeedType biggestNeed)
		{
			const float threshold = 0.1f;
			var resolver = new ActiveNeedResolver();
			var needData = new NeedData(new Dictionary<NeedType, float>{{currentNeed, 0.5f},{biggestNeed, 0.5f - threshold + 0.001f}}, null);
			needData.CurrentNeed = currentNeed;

			NeedType result = resolver.ResolveActiveNeed(needData);

			result.Should().Be(currentNeed);
		}

		[TestCase(NeedType.Care, NeedType.Rest)]
		[TestCase(NeedType.Rest, NeedType.Hunger)]
		public void ResolveActiveNeed_BiggestAndCurrentNeedDifferenceExceedsThreshold_ReturnsBiggestNeed(NeedType currentNeed, NeedType biggestNeed)
		{
			const float threshold = 0.1f;
			var resolver = new ActiveNeedResolver();
			var needData = new NeedData(new Dictionary<NeedType, float> { { currentNeed, 0.5f }, { biggestNeed, 0.5f - threshold - 0.001f } }, null);
			needData.CurrentNeed = currentNeed;

			NeedType result = resolver.ResolveActiveNeed(needData);

			result.Should().Be(biggestNeed);
		}
	}
}