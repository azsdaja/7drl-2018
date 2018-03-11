using System.Linq;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.RNG;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class DeathHandler : IDeathHandler
	{
		private readonly IGameConfig _gameConfig;
		private readonly IUiConfig _uiConfig;
		private readonly IEntityRemover _entityRemover;
		private readonly IEntitySpawner _entitySpawner;
		private readonly IRandomNumberGenerator _rng;

		public DeathHandler(IGameConfig gameConfig, IUiConfig uiConfig, IEntityRemover entityRemover, IEntitySpawner entitySpawner, 
			IRandomNumberGenerator rng)
		{
			_gameConfig = gameConfig;
			_uiConfig = uiConfig;
			_entityRemover = entityRemover;
			_entitySpawner = entitySpawner;
			_rng = rng;
		}
	
		public void HandleDeath(ActorData actorData)
		{
			ItemDefinition weaponToSpawn = actorData.WeaponWeld;
			_entityRemover.CleanSceneAndGameContextAfterDeath(actorData);
			if (actorData.ActorType != ActorType.Friend)
			{
				_entitySpawner.SpawnItem(_rng.Choice(_gameConfig.ItemConfig.Definitions), actorData.LogicalPosition);
			}
			
			if (!weaponToSpawn.WeaponDefinition.IsBodyPart)
			{
				_entitySpawner.SpawnItem(weaponToSpawn, actorData.LogicalPosition);
			}

			if (actorData.ControlledByPlayer)
			{
				_uiConfig.RestartButton.gameObject.SetActive(true);
				var postMortem = "You died at level " + actorData.Level + " after surviving " + actorData.RoundsCount +
				                 " rounds. <br>Restart (y\\n)?";
				_uiConfig.RestartButton.transform.GetComponentInChildren<Text>().text = postMortem;
			}
		}
	}
}