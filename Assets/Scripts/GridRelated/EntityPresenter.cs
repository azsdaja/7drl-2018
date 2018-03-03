using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic;
using UnityEngine;

namespace Assets.Scripts.GridRelated
{
	public class EntityPresenter : IEntityPresenter
	{
		private HashSet<IGameEntity> _visibleEntitiesSaved = new HashSet<IGameEntity>();

		public void Illuminate(HashSet<Vector2Int> visibleTiles, IEnumerable<IGameEntity> potentiallyVisibleEntities)
		{
			IEnumerable<IGameEntity> visibleEntities = GetVisibleEntities(visibleTiles, potentiallyVisibleEntities);
			var newlyLitEntities = new HashSet<IGameEntity>();
			foreach (IGameEntity oldEntity in _visibleEntitiesSaved)
			{
				if (visibleEntities.Contains(oldEntity)) continue;
				oldEntity.Hide();
			}
			foreach (IGameEntity currentEntity in visibleEntities)
			{
				if (_visibleEntitiesSaved.Contains(currentEntity)) continue;
				newlyLitEntities.Add(currentEntity);
				currentEntity.Show();
			}
			_visibleEntitiesSaved = new HashSet<IGameEntity>(visibleEntities);
		}

		public void RemoveDestroyedEntity(IGameEntity actorDataEntity)
		{
			_visibleEntitiesSaved.Remove(actorDataEntity);
		}

		internal IEnumerable<IGameEntity> GetVisibleEntities(HashSet<Vector2Int> visibleTiles, IEnumerable<IGameEntity> potentiallyVisibleEntities)
		{
			foreach (IGameEntity entity in potentiallyVisibleEntities)
			{
				Vector2Int entityPositionOnGrid = entity.EntityData.LogicalPosition;
				if (visibleTiles.Contains(entityPositionOnGrid))
					yield return entity;
			}
		}
	}
}