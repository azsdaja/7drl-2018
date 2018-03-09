using Assets.Scripts.GameLogic.Configuration;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[CreateAssetMenu(fileName = "ActorDefinition", menuName = "7DRL Assets/ActorDefinition", order = 0)]
	public class ActorDefinition : ScriptableObject
	{
		public ActorType ActorType;
		public Sprite Sprite;
		public Weapon[] WeaponPool;
		public int SwordsFromSkill;
		public int VisionRayLength;
		public float EnergyGain = 0.1f;
		public Team Team = Team.Neutral;
		public int MaxHealth;
		public float Accuracy = 0.6f;
		public int XpGiven;
		public int InitialLevel;
	}
}