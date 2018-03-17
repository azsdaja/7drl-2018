using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic.GameCore
{
	public interface IUiConfig
	{
		Canvas TooltipPooled { get; }
		ProgressBar HealthBar { get; }
		Button RestartButton { get; }
		GameObject Arrows { get;}
		AdvanceManager AdvanceManager { get; }
		ItemHolder ItemHolder { get; }
		GameObject CurrentWeaponHolder { get; }
		GameFinisher GameFinisher { get; }
		BasherMessage BasherMessage { get; }

		Button WalkAbilityButton { get; }
		Button DaringBlowAbilityButton { get; }
		Button PushAbilityButton { get; }
		TooltipPresenter TooltipPresenter { get; }
		TooltipPresenter TooltipCurrentWeaponPresenter { get; }
	}
}