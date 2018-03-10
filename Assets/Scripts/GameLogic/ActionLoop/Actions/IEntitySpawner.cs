using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public interface IEntitySpawner
	{
		void SpawnItem(ItemDefinition item, Vector2Int spawnPosition);
		ActorBehaviour SpawnActor(ActorType actorType, Vector2Int position);
		void SpawnWeapon(WeaponDefinition weaponToSpawn, Vector2Int spawnPosition);
	}
}