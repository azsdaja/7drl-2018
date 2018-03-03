namespace Assets.Scripts.GameLogic.ActionLoop.AI
{
	public class ActiveNeedResolver : IActiveNeedResolver
	{
		public NeedType ResolveActiveNeed(NeedData needData)
		{
			NeedType currentNeed = needData.CurrentNeed;
			NeedType biggestNeed = needData.GetLeastSatisfied();

			bool biggestNeedShouldBecomeCurrent;
			if (currentNeed == NeedType.Default || currentNeed == biggestNeed)
				biggestNeedShouldBecomeCurrent = true;
			else
			{
				float currentNeedSatisfaction = needData.GetSatisfaction(currentNeed);
				float biggestNeedSatisfaction = needData.GetSatisfaction(biggestNeed);
				const float threshold = 0.10f;
				bool thresholdIsExceeded = currentNeedSatisfaction - biggestNeedSatisfaction > threshold;
				biggestNeedShouldBecomeCurrent = thresholdIsExceeded;
			}

			if (biggestNeedShouldBecomeCurrent)
				currentNeed = biggestNeed;
			return currentNeed;
		}
	}
}