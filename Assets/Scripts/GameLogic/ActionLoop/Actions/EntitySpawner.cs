using System.Linq;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.RNG;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class EntitySpawner : IEntitySpawner
	{
		private readonly ItemBehaviour.Factory _itemBehaviourFactory;
		private readonly ActorBehaviour.Factory _actorBehaviourFactory;
		private readonly IGameContext _gameContext;
		private readonly IGameConfig _gameConfig;
		private IRandomNumberGenerator _rng;

		public EntitySpawner(ItemBehaviour.Factory itemBehaviourFactory, ActorBehaviour.Factory actorBehaviourFactory, 
			IGameContext gameContext, IGameConfig gameConfig, IRandomNumberGenerator rng)
		{
			_itemBehaviourFactory = itemBehaviourFactory;
			_actorBehaviourFactory = actorBehaviourFactory;
			_gameContext = gameContext;
			_gameConfig = gameConfig;
			_rng = rng;
		}

		public void SpawnItem(ItemType itemType, Vector2Int position)
		{
			ItemBehaviour instantiatedItem = _itemBehaviourFactory.Create();
			Sprite itemSprite;
			if (itemType == ItemType.DeadBody)
				itemSprite = Resources.Load<Sprite>("Sprites/Items/DeadBody");
			else
				itemSprite = Resources.Load<Sprite>("Sprites/Items/Key");

			instantiatedItem.GetComponent<SpriteRenderer>().sprite = itemSprite;
			instantiatedItem.ItemData.ItemType = itemType;

			instantiatedItem.ItemData.LogicalPosition = position;
			instantiatedItem.RefreshWorldPosition();
		}

		public ActorBehaviour SpawnActor(ActorType actorType, Vector2Int position)
		{
			ActorBehaviour instantiatedActor = _actorBehaviourFactory.Create();
			instantiatedActor.name = actorType.ToString();
			ActorData actorData = instantiatedActor.ActorData;
			actorData.ActorType = actorType;

			ActorDefinition actorDefinition = _gameConfig.ActorConfig.GetDefinition(actorType);
			instantiatedActor.GetComponent<SpriteRenderer>().sprite = actorDefinition.Sprite;
			actorData.Weapon = _rng.Choice(actorDefinition.WeaponPool);
			actorData.SwordsFromSkill = actorDefinition.SwordsFromSkill;
			actorData.VisionRayLength = actorDefinition.VisionRayLength;
			actorData.EnergyGain = actorDefinition.EnergyGain;
			actorData.Team = actorDefinition.Team;
			actorData.MaxHealth = actorDefinition.MaxHealth;
			actorData.Health = actorDefinition.MaxHealth;
			actorData.Accuracy = actorDefinition.Accuracy;
			actorData.XpGiven = actorDefinition.XpGiven;
			actorData.Level = actorDefinition.InitialLevel;
			actorData.Traits = actorDefinition.InitialTraits.ToArray().ToList();
			actorData.AiTraits = actorDefinition.AiTraits.ToArray().ToList();

			actorData.LogicalPosition = position;
			instantiatedActor.RefreshWorldPosition();
			_gameContext.Actors.Add(instantiatedActor);
			instantiatedActor.gameObject.SetActive(true);
			return instantiatedActor;
		}
	}
}