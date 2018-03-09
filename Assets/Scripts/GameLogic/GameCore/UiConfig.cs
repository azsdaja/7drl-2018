using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class UiConfig : MonoBehaviour, IUiConfig
	{
		public ProgressBar HealthBar;
		public Canvas TooltipPooled;
		public Button RestartButton;
		public GameObject Arrows;
		public AdvanceManager AdvanceManager;
		public Button WalkAbilityButton;
		public Button DaringBlowAbilityButton;
		public Button PushAbilityButton;
	
		Canvas IUiConfig.TooltipPooled{ get { return TooltipPooled; }}
		ProgressBar IUiConfig.HealthBar { get { return HealthBar; }}
		Button IUiConfig.RestartButton { get { return RestartButton; }}
		GameObject IUiConfig.Arrows { get { return Arrows; } }
		AdvanceManager IUiConfig.AdvanceManager { get { return AdvanceManager; } }
		Button IUiConfig.WalkAbilityButton { get { return WalkAbilityButton; } }
		Button IUiConfig.DaringBlowAbilityButton { get { return DaringBlowAbilityButton; } }
		Button IUiConfig.PushAbilityButton { get { return PushAbilityButton; } }
	}
}