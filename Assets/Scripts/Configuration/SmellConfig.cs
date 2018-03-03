using UnityEngine;

namespace Assets.Scripts.Configuration
{
	[CreateAssetMenu(fileName = "SmellConfig", menuName = "Configuration/SmellConfig", order = 1)]
	public class SmellConfig : ScriptableObject
	{
		public AnimationCurve DistanceToChance;
	}
}