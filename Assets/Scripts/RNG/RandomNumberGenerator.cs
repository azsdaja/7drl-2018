using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Assets.Scripts.RNG
{
	public class RandomNumberGenerator : IRandomNumberGenerator
	{
		private readonly Random _random;

		public RandomNumberGenerator()
		{
			_random = new Random();
		}

		[Inject]
		public RandomNumberGenerator(int seed)
		{
			_random = new Random(seed);
		}

		public int Next(int maxExclusive)
		{
			return _random.Next(maxExclusive);
		}

		public int Next(int minInclusive, int maxExclusive)
		{
			return _random.Next(minInclusive, maxExclusive);
		}

		public float NextFloat()
		{
			return (float)_random.NextDouble();
		}

		public TElement Choice<TElement>(IList<TElement> elementCollection)
		{
			int randomIndex = _random.Next(elementCollection.Count);
			TElement randomElement = elementCollection[randomIndex];
			return randomElement;
		}

		public bool Check(float chance)
		{
			float outcome = NextFloat();
			return outcome < chance;
		}

		public TimeSpan NextTimeSpan(TimeSpan maxSpan)
		{
			double maxSeconds = maxSpan.TotalSeconds;
			double randomSeconds = NextFloat() * maxSeconds;
			return TimeSpan.FromSeconds(randomSeconds);
		}

		public Vector2Int NextPosition(BoundsInt gridBounds)
		{
			int x = Next(gridBounds.xMin, gridBounds.xMax + 1);
			int y = Next(gridBounds.yMin, gridBounds.yMax + 1);
			return new Vector2Int(x, y);
		}

		public Vector2Int BiasedPosition(Vector2Int centralPosition, int variationRadius)
		{
			BoundsInt areaBounds = new BoundsInt(centralPosition.x - variationRadius, centralPosition.y - variationRadius, 0,
				2 * variationRadius, 2 * variationRadius, 1);
			Vector2Int candidatePosition;
			do
			{
				candidatePosition = NextPosition(areaBounds);
			} while (Vector2Int.Distance(centralPosition, candidatePosition) > variationRadius);

			return candidatePosition;
		}

		public IList<Vector2Int> Shuffle(IEnumerable<Vector2Int> candidateMovesToEnemy)
		{
			List<Vector2Int> shuffled = candidateMovesToEnemy.ToList();
			shuffled.Sort( (first, other) => NextFloat().CompareTo(NextFloat()));
			return shuffled;
		}
	}
}
