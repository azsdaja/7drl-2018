using UnityEngine;

namespace Assets.Scripts.GameLogic.GameCore
{
	[CreateAssetMenu(fileName = "DungeonConfig", menuName = "Configuration/DungeonConfig", order = 0)]
	public class DungeonConfig : ScriptableObject
	{
		public Vector2Int Size;
		public AnimationCurve ChanceToRoomPopulation;
		public ActorDefinition[] EnemiesToSpawn;
		public ActorDefinition BossToSpawn;
	}
}