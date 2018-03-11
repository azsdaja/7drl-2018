using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using Assets.Scripts.UI;

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
			sum += actorData.WeaponWeld.WeaponDefinition.Swords;
			if (actorData.Health < actorData.MaxHealth * .5f)
				sum -= 1;
			if (actorData.IsInCloseCombat)
			{
				sum += actorData.WeaponWeld.WeaponDefinition.CloseCombatModifier;
			}
			if (actorData.WeaponWeld.WeaponDefinition.AllowsFarCombat == false)
			{
				if (actorData.Traits.Contains(Trait.ShortWeaponsExpert))
					sum += 1;
				if (actorData.Traits.Contains(Trait.ShortWeaponsMaster))
					sum += 1;
			}
			if (actorData.WeaponWeld.WeaponDefinition.AllowsFarCombat == true)
			{
				if (actorData.Traits.Contains(Trait.LongWeaponsExpert))
					sum += 1;
				if (actorData.Traits.Contains(Trait.LongWeaponsMaster))
					sum += 1;
			}
			if (actorData.HasLittleSpace && !actorData.Traits.Contains(Trait.Nimble))
			{
				--sum;
			}
			return sum;
		}


	}
}