using System.Linq;
using Assets.Scripts.GameLogic.GameCore;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class DeathHandler : IDeathHandler
	{
		private readonly IGameConfig _gameConfig;
		private readonly IUiConfig _uiConfig;
		private readonly IEntityRemover _entityRemover;
		private readonly IEntitySpawner _entitySpawner;

		public DeathHandler(IGameConfig gameConfig, IUiConfig uiConfig, IEntityRemover entityRemover, IEntitySpawner entitySpawner)
		{
			_gameConfig = gameConfig;
			_uiConfig = uiConfig;
			_entityRemover = entityRemover;
			_entitySpawner = entitySpawner;;
		}
	
		public void HandleDeath(ActorData actorData)
		{
			ItemDefinition weaponToSpawn = actorData.WeaponWeld;
			_entityRemover.CleanSceneAndGameContextAfterDeath(actorData);

			_entitySpawner.SpawnItem(_gameConfig.ItemConfig.Definitions.First(), actorData.LogicalPosition);
			if (!weaponToSpawn.WeaponDefinition.IsBodyPart)
			{
				_entitySpawner.SpawnItem(weaponToSpawn, actorData.LogicalPosition);
			}

			if (actorData.ControlledByPlayer)
			{
				_uiConfig.RestartButton.gameObject.SetActive(true);
			}
		}
	}
}