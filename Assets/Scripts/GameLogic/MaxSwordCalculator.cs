using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.UI;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.GameLogic
{
	public class MaxSwordCalculator : IMaxSwordCalculator
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IEntityDetector _entityDetector;
		private readonly ITextEffectPresenter _textEffectPresenter;

		public MaxSwordCalculator(IGridInfoProvider gridInfoProvider, IEntityDetector entityDetector, ITextEffectPresenter textEffectPresenter)
		{
			_gridInfoProvider = gridInfoProvider;
			_entityDetector = entityDetector;
			_textEffectPresenter = textEffectPresenter;
		}

		public int Calculate(ActorData actorData)
		{
			int sum = 0;
			sum += actorData.SwordsFromSkill;
			sum += actorData.Weapon.Swords;
			if (actorData.Health < actorData.MaxHealth * .5f)
				sum -= 1;
			if (actorData.IsInCloseCombat)
			{
				sum += actorData.Weapon.CloseCombatModifier;
			}
			if (actorData.HasLittleSpace && !actorData.Traits.Contains(Trait.Nimble))
			{
				--sum;
			}
			return sum;
		}


	}
}