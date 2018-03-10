using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
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
		private readonly IRandomNumberGenerator _rng;
		private readonly IEntityDetector _entityDetector;
		private IGridInfoProvider _gridInfoProvider;

		public EntitySpawner(ItemBehaviour.Factory itemBehaviourFactory, ActorBehaviour.Factory actorBehaviourFactory, 
			IGameContext gameContext, IGameConfig gameConfig, IRandomNumberGenerator rng, IEntityDetector entityDetector, IGridInfoProvider gridInfoProvider)
		{
			_itemBehaviourFactory = itemBehaviourFactory;
			_actorBehaviourFactory = actorBehaviourFactory;
			_gameContext = gameContext;
			_gameConfig = gameConfig;
			_rng = rng;
			_entityDetector = entityDetector;
			_gridInfoProvider = gridInfoProvider;
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

		public void SpawnWeapon(Weapon weaponToSpawn, Vector2Int position)
		{
			ItemBehaviour instantiatedItem = _itemBehaviourFactory.Create();

			instantiatedItem.GetComponent<SpriteRenderer>().sprite = weaponToSpawn.Sprite;
			instantiatedItem.ItemData.ItemType = ItemType.Weapon;
			instantiatedItem.ItemData.Weapon = weaponToSpawn;

			Vector2Int positionToPlaceItem = position;
			if (_entityDetector.DetectItems(positionToPlaceItem).Any())
			{
				List<Vector2Int> neighbours = Vector2IntUtilities.Neighbours8(positionToPlaceItem);
				foreach (Vector2Int neighbour in neighbours)
				{
					if (_gridInfoProvider.IsWalkable(neighbour) && !_entityDetector.DetectItems(positionToPlaceItem).Any())
					{
						positionToPlaceItem = neighbour;
						break;
					}
				}
			}
			instantiatedItem.ItemData.LogicalPosition = positionToPlaceItem;
			instantiatedItem.RefreshWorldPosition();
		}
	}
}