using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic.GameCore
{
	public class UiConfig : MonoBehaviour, IUiConfig
	{
		public ProgressBar HealthBar;
		public Canvas TooltipPooled;
		public Button RestartButton;
	
		Canvas IUiConfig.TooltipPooled{ get { return TooltipPooled; }}
		ProgressBar IUiConfig.HealthBar { get { return HealthBar; }}
		Button IUiConfig.RestartButton { get { return RestartButton; }}
	}
}