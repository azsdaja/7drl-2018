using System.Linq;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.RNG;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class DeathHandler : IDeathHandler
	{
		private readonly IGameConfig _gameConfig;
		private readonly IGameContext _gameContext;
		private readonly IUiConfig _uiConfig;
		private readonly IEntityRemover _entityRemover;
		private readonly IEntitySpawner _entitySpawner;
		private readonly IRandomNumberGenerator _rng;

		public DeathHandler(IGameConfig gameConfig, IUiConfig uiConfig, IEntityRemover entityRemover, IEntitySpawner entitySpawner, 
			IRandomNumberGenerator rng, IGameContext gameContext)
		{
			_gameConfig = gameConfig;
			_uiConfig = uiConfig;
			_entityRemover = entityRemover;
			_entitySpawner = entitySpawner;
			_rng = rng;
			_gameContext = gameContext;
		}
	
		public void HandleDeath(ActorData actorData)
		{
			ItemDefinition weaponToSpawn = actorData.WeaponWeld;
			_entityRemover.CleanSceneAndGameContextAfterDeath(actorData);
			//_entitySpawner.SpawnItem(_gameConfig.ItemConfig.Definitions.FirstOrDefault(i => i.ItemType == ItemType.PotionOfFriend), actorData.LogicalPosition);
			if (actorData.ActorType != ActorType.Buddy && actorData.ActorType != ActorType.Friend && _rng.Check(0.65f))
			{
				ItemDefinition item = _rng.Choice(_gameConfig.ItemConfig.Definitions);
				if (item.ItemType == ItemType.PotionOfBuddy)
				{
					if(_gameContext.CurrentDungeonIndex >= 1)
						_entitySpawner.SpawnItem(item, actorData.LogicalPosition);
				}
				else if (item.ItemType == ItemType.PotionOfHealing)
				{
					if(_gameContext.CurrentDungeonIndex >= 2 && _rng.Check(0.8f))
						_entitySpawner.SpawnItem(item, actorData.LogicalPosition);
				}
				else if (item.ItemType == ItemType.PotionOfFriend)
				{
					if (
						(_gameContext.CurrentDungeonIndex == 2 && _rng.Check(0.5f))
						|| (_gameContext.CurrentDungeonIndex == 3 && _rng.Check(0.6f))
						|| (_gameContext.CurrentDungeonIndex == 4 && _rng.Check(0.8f))
						|| (_gameContext.CurrentDungeonIndex >= 5)
						)
						_entitySpawner.SpawnItem(item, actorData.LogicalPosition);
				}
				else
					_entitySpawner.SpawnItem(item, actorData.LogicalPosition);
			}
			
			if (!weaponToSpawn.WeaponDefinition.IsBodyPart && actorData.ActorType != ActorType.Buddy && actorData.ActorType != ActorType.Friend)
			{
				_entitySpawner.SpawnItem(weaponToSpawn, actorData.LogicalPosition);
			}

			if (actorData.ControlledByPlayer)
			{
				_uiConfig.RestartButton.gameObject.SetActive(true);
				var postMortem = "You died at level " + actorData.Level + " after surviving " + actorData.RoundsCount +
				                 " rounds. \r\nRestart?";
				_uiConfig.RestartButton.transform.GetComponentInChildren<Text>().text = postMortem;
			}
		}
	}
}