using UnityEngine;

namespace Assets.Scripts.Configuration
{
	[CreateAssetMenu(fileName = "ModeConfig", menuName = "Configuration/ModeConfig", order = 1)]
	public class ModeConfig : ScriptableObject
	{
		public bool ShowNeedBars;
		public bool ShowPaths;
		public bool ShowCurrentNeed;
		public bool ShowActorTooltip;
	}
}