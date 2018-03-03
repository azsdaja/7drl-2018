using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class EntitySpawner : IEntitySpawner
	{
		private readonly ItemBehaviour.Factory _itemBehaviourFactory;
		private readonly ActorBehaviour.Factory _actorBehaviourFactory;
		private readonly IGameContext _gameContext;

		public EntitySpawner(ItemBehaviour.Factory itemBehaviourFactory, ActorBehaviour.Factory actorBehaviourFactory, IGameContext gameContext)
		{
			_itemBehaviourFactory = itemBehaviourFactory;
			_actorBehaviourFactory = actorBehaviourFactory;
			_gameContext = gameContext;
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
			Sprite actorSprite;
			if(actorType == ActorType.HerdAnimalFather)
				actorSprite = Resources.Load<Sprite>("Sprites/Characters/_deer male calciumtrice single");
			else if (actorType == ActorType.HerdAnimalMother)
				actorSprite = Resources.Load<Sprite>("Sprites/Characters/_deer female calciumtrice");
			else
				actorSprite = Resources.Load<Sprite>("Sprites/Characters/_deer immature calciumtrice");

			instantiatedActor.GetComponent<SpriteRenderer>().sprite = actorSprite;
			instantiatedActor.ActorData.ActorType = actorType;
			instantiatedActor.ActorData.LogicalPosition = position;
			instantiatedActor.RefreshWorldPosition();

			_gameContext.Actors.Add(instantiatedActor);

			instantiatedActor.gameObject.SetActive(true);
			return instantiatedActor;
		}
	}
}