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
			if (actorData.ActorType == ActorType.Basher)
			{
				_gameContext.BasherDead = true;
			}
			if (actorData.IsBoss && _gameContext.CurrentDungeonIndex == 0)
			{
				_entitySpawner.SpawnItem(_gameConfig.ItemConfig.Definitions.First(i => i.ItemType == ItemType.PotionOfBuddy), actorData.LogicalPosition);
			}
			else if (actorData.ActorType != ActorType.Buddy && actorData.ActorType != ActorType.Friend && _rng.Check(0.42f))
			{
				ItemDefinition[] itemPool = actorData.XpGiven < 15
					? _gameConfig.ItemConfig.ItemPoolWeak
					: actorData.XpGiven < 26 ? _gameConfig.ItemConfig.ItemPoolMedium : _gameConfig.ItemConfig.ItemPoolStrong;
				ItemDefinition item = _rng.Choice(itemPool);

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
				                 " rounds. \r\nIf you have trouble playing, check out \"Combat help\" to the right. Restart?";
				_uiConfig.RestartButton.transform.GetComponentInChildren<Text>().text = postMortem;
			}
		}
	}
}