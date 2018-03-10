using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public interface IEntitySpawner
	{
		void SpawnItem(ItemType itemType, Vector2Int position);
		ActorBehaviour SpawnActor(ActorType actorType, Vector2Int position);
		void SpawnWeapon(Weapon weaponToSpawn, Vector2Int position);
	}
}