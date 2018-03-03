using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class NeedData
	{
		private NeedType _currentNeed;
	
		private readonly IDictionary<NeedType, float> _satisfactions;

		private readonly IDictionary<NeedFactor, float> _factors;

		[SerializeField, HideInInspector] private IDictionary<NeedType, int> _needsToCounts;

		public NeedData(IDictionary<NeedType, float> satisfactions, IDictionary<NeedFactor, float> factors)
		{
			_satisfactions = satisfactions;
			_factors = factors;
			_needsToCounts = new Dictionary<NeedType, int>();
		}

		public NeedType CurrentNeed
		{
			get { return _currentNeed; }
			set { _currentNeed = value; }
		}

		public IDictionary<NeedType, int> NeedsToCounts
		{
			get { return _needsToCounts; }
			set { _needsToCounts = value; }
		}
	
		public float GetSatisfaction(NeedType needType)
		{
			return Mathf.Clamp01(_satisfactions[needType]);
		}

		public void ModifySatisfaction(NeedType needType, float delta)
		{
			_satisfactions[needType] += delta;
			_satisfactions[needType] = Mathf.Clamp01(_satisfactions[needType]);
		}

		public void ModifyNeedWithFactor(NeedType needType, NeedFactor needFactor, float delta)
		{
			float previousFactorValue = _factors[needFactor];
			float satisfactionChangeFromFactor = delta - previousFactorValue;

			_factors[needFactor] = delta;

			_satisfactions[needType] += satisfactionChangeFromFactor;
		}
	
		public NeedType GetLeastSatisfied()
		{
			var satisfactionsSorted = _satisfactions.ToList();
			satisfactionsSorted.Sort((first, second) => first.Value.CompareTo(second.Value));
			return satisfactionsSorted.First().Key;
		}
	}
}