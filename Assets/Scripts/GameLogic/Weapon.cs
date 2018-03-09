using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[CreateAssetMenu(fileName = "Weapon", menuName = "7DRL Assets/Weapon", order = 0)]
	public class Weapon : ScriptableObject
	{
		public bool IsBodyPart;

		public Sprite Sprite;

		public int RecoveryTime;
		public int MaxDamage;
		public int Swords;

		public int CloseCombatModifier;
		public bool AllowsFarCombat;

		public WeaponAnimationData WeaponAnimationData;
	}
}