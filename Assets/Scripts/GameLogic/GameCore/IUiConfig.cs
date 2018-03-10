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

		Button WalkAbilityButton { get; }
		Button DaringBlowAbilityButton { get; }
		Button PushAbilityButton { get; }
		TooltipPresenter TooltipPresenter { get; }
	}
}