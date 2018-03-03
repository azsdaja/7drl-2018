using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using UnityEngine;

namespace Assets.Scripts.GridRelated
{
	public interface IEntityPresenter
	{
		void Illuminate(HashSet<Vector2Int> visibleTiles, IEnumerable<IGameEntity> potentiallyVisibleEntities);
		void RemoveDestroyedEntity(IGameEntity actorDataEntity);
	}
}