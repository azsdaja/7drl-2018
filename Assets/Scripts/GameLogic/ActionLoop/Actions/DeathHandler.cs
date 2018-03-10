using Assets.Scripts.GameLogic.GameCore;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class DeathHandler : IDeathHandler
	{
		private readonly IUiConfig _uiConfig;
		private readonly IEntityRemover _entityRemover;
		private readonly IEntitySpawner _entitySpawner;

		public DeathHandler(IUiConfig uiConfig, IEntityRemover entityRemover, IEntitySpawner entitySpawner)
		{
			_uiConfig = uiConfig;
			_entityRemover = entityRemover;
			_entitySpawner = entitySpawner;
		}
	
		public void HandleDeath(ActorData actorData)
		{
			Weapon weaponToSpawn = actorData.Weapon;
			_entityRemover.CleanSceneAndGameContextAfterDeath(actorData);

			_entitySpawner.SpawnItem(ItemType.DeadBody, actorData.LogicalPosition);
			_entitySpawner.SpawnWeapon(weaponToSpawn, actorData.LogicalPosition);

			if (actorData.ControlledByPlayer)
			{
				_uiConfig.RestartButton.gameObject.SetActive(true);
			}
		}
	}
}