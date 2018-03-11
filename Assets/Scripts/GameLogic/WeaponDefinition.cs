using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[CreateAssetMenu(fileName = "WeaponDefinition", menuName = "7DRL Assets/WeaponDefinition", order = 0)]
	public class WeaponDefinition : ScriptableObject
	{
		public bool IsBodyPart;

		public Sprite Sprite;

		public RecoveryTime RecoveryTime;
		public int MaxDamage;
		public int Swords;

		public int CloseCombatModifier;
		public bool AllowsFarCombat;
		
		public WeaponAnimationData WeaponAnimationData;
	}

	public enum RecoveryTime
	{
		ThreePerFive,
			OnePerTwo,
			ThreePerSeven
	}
}