using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[CreateAssetMenu(fileName = "WeaponAnimationData", menuName = "7DRL Assets/WeaponAnimationData", order = 0)]
	public class WeaponAnimationData : ScriptableObject
	{
		public float NormalAnimationDuration = .3f; // remember that the time of blocking the 
		// attacked and attacking player is defined elsewhere, in AttackAction. Fix?
		public float AggressiveAnimationDuration = .4f;
		public AnimationCurve NormalMovementCurve;
		public AnimationCurve AggressiveMovementCurve;
		public AnimationCurve[] DeviationCurves;
		public AnimationCurve[] DeviationCurvesForRotation;
	}
}