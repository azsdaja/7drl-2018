using Assets.Scripts.GameLogic.GameCore;
using Assets.Scripts.GridRelated;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.Actions
{
	public class EntityRemover : IEntityRemover
	{
		private readonly IGameContext _gameContext;
		private readonly IEntityPresenter _entityPresenter;
	

		public EntityRemover(IGameContext gameContext, IEntityPresenter entityPresenter)
		{
			_gameContext = gameContext;
			_entityPresenter = entityPresenter;
		}

		public void CleanSceneAndGameContextAfterDeath(ActorData actorData)
		{
			// todo: get rid of this ugly cast
			_gameContext.Actors.Remove(actorData.Entity as ActorBehaviour);

			_entityPresenter.RemoveDestroyedEntity(actorData.Entity);

			GameObject.Destroy(actorData.Entity.gameObject);
		}

		public void RemoveItem(ItemData foodItemToEat)
		{
			_entityPresenter.RemoveDestroyedEntity(foodItemToEat.Entity);
			GameObject.Destroy(foodItemToEat.Entity.gameObject);
		}
	}
}