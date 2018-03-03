using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using UnityEngine;

namespace Assets.Scripts.GridRelated
{
	public interface IEntityDetector
	{
		IEnumerable<IGameEntity> DetectEntities(Vector2Int targetPosition, float cellsRangeInArea);
		IEnumerable<IGameEntity> DetectEntities(Vector2Int targetPosition);

		IEnumerable<ActorData> DetectActors(Vector2Int targetPosition, float cellsRangeInArea);
		IEnumerable<ActorData> DetectActors(Vector2Int targetPosition);
		IEnumerable<ItemData> DetectItems(Vector2Int targetPosition, float cellsRangeInArea);
		IEnumerable<ItemData> DetectItems(Vector2Int targetPosition);
	}
}