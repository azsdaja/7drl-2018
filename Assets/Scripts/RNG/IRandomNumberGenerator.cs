using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.RNG
{
	public interface IRandomNumberGenerator
	{
		int Next(int maxExclusive);
		int Next(int minInclusive, int maxExclusive);
		float NextFloat();
		TElement Choice<TElement>(IList<TElement> elementCollection);
		bool Check(float chance);
		TimeSpan NextTimeSpan(TimeSpan maxSpan);
		Vector2Int NextPosition(BoundsInt gridBounds);
		Vector2Int BiasedPosition(Vector2Int centralPosition, int variationRadius);
	}
}