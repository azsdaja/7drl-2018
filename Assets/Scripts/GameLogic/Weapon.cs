using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[CreateAssetMenu(fileName = "Weapon", menuName = "7DRL Assets/Weapon", order = 0)]
	public class Weapon : ScriptableObject
	{
		public int RecoveryTime;
		public int MaxDamage;
	}
}