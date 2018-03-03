using UnityEngine;

namespace Assets.Scripts.Configuration
{
	[CreateAssetMenu(fileName = "NeedConfig", menuName = "Configuration/GameConfig", order = 0)]
	public class NeedConfig : ScriptableObject
	{
		public AnimationCurve ProximityToDanger;
		public AnimationCurve ProximityToSafety;
		public AnimationCurve ProximityToChildCare;
		public AnimationCurve ProtectorProximityToSafety;

	}
}