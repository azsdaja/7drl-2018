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
	}
}