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

		public void SpawnItem(ItemDefinition item, Vector2Int spawnPosition)
		{
			ItemBehaviour instantiatedItem = _itemBehaviourFactory.Create();

			instantiatedItem.GetComponent<SpriteRenderer>().sprite = item.Sprite;
			instantiatedItem.ItemData.ItemType = item.ItemType;
			instantiatedItem.ItemData.ItemDefinition = item;

			Vector2Int positionToPlaceItem = GetPositionToPlaceItem(spawnPosition);
			instantiatedItem.ItemData.LogicalPosition = positionToPlaceItem;
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
			actorData.WeaponWeld = _rng.Choice(actorDefinition.WeaponPool);
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

			Vector2Int freePosition = GetPositionToPlaceActor(position);
			actorData.LogicalPosition = freePosition;
			instantiatedActor.RefreshWorldPosition();
			_gameContext.Actors.Add(instantiatedActor);
			instantiatedActor.gameObject.SetActive(true);
			return instantiatedActor;
		}

		public void SpawnWeapon(WeaponDefinition weaponToSpawn, Vector2Int spawnPosition)
		{
			ItemBehaviour instantiatedItem = _itemBehaviourFactory.Create();

			instantiatedItem.GetComponent<SpriteRenderer>().sprite = weaponToSpawn.Sprite;
			instantiatedItem.ItemData.ItemType = ItemType.Weapon;
			instantiatedItem.ItemData.WeaponDefinition = weaponToSpawn;
			

			Vector2Int positionToPlaceItem = GetPositionToPlaceItem(spawnPosition);
			instantiatedItem.ItemData.LogicalPosition = positionToPlaceItem;
			instantiatedItem.RefreshWorldPosition();
		}

		private Vector2Int GetPositionToPlaceItem(Vector2Int position)
		{
			if (_entityDetector.DetectItems(position).Any())
			{
				List<Vector2Int> candidates = Vector2IntUtilities.Neighbours8(position);
				var candidatesFurther = Vector2IntUtilities.Neighbours8(Vector2Int.zero)
					.Select(v => new Vector2Int(v.x*2, v.y*2))
					.Select(v => position + v);
				candidates.AddRange(candidatesFurther);
				foreach (Vector2Int neighbour in candidates)
				{
					if (_gridInfoProvider.IsWalkable(neighbour) && !_entityDetector.DetectItems(neighbour).Any())
					{
						position = neighbour;
						break;
					}
				}
			}
			return position;
		}

		private Vector2Int GetPositionToPlaceActor(Vector2Int position)
		{
			List<Vector2Int> candidates = Vector2IntUtilities.Neighbours8(position);
			var candidatesFurther = Vector2IntUtilities.Neighbours8(Vector2Int.zero)
				.Select(v => new Vector2Int(v.x*2, v.y*2))
				.Select(v => position + v);
			candidates.AddRange(candidatesFurther);
			foreach (Vector2Int neighbour in candidates)
			{
				if (_gridInfoProvider.IsWalkable(neighbour) && !_entityDetector.DetectActors(neighbour).Any())
				{
					return neighbour;
				}
			}
			return position;
		}
	}
}