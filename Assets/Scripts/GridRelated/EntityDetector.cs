using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.Configuration;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GridRelated
{
	public class EntityDetector : IEntityDetector
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private const float CellSizeEpsilon = .001f;

		public EntityDetector(IGridInfoProvider gridInfoProvider)
		{
			_gridInfoProvider = gridInfoProvider;
		}

		public IEnumerable<IGameEntity> DetectEntities(Vector2Int targetPosition, float cellsRangeInArea)
		{
			Collider2D[] hitActorColliders = GetCollidersInRange(targetPosition, cellsRangeInArea, new[] {LayerNames.Actor, LayerNames.Item});
			IEnumerable<IGameEntity> actorsHit = hitActorColliders.Select(c => c.GetComponent<IGameEntity>());
			return actorsHit;
		}

		public IEnumerable<IGameEntity> DetectEntities(Vector2Int targetPosition)
		{
			return DetectEntities(targetPosition, CellSizeEpsilon);
		}

		public IEnumerable<ActorData> DetectActors(Vector2Int targetPosition, float cellsRangeInArea)
		{
			Collider2D[] hitActorColliders = GetCollidersInRange(targetPosition, cellsRangeInArea, new[] {LayerNames.Actor});
			IEnumerable<ActorData> actorsHit = hitActorColliders.Select(c => c.GetComponent<ActorBehaviour>().ActorData);
			return actorsHit;
		}

		public IEnumerable<ActorData> DetectActors(Vector2Int targetPosition)
		{
			// couldn't use DetectActors(targetPosition, CellSizeEpsilon) because some actor that just moved may already not be in place.
			return DetectActors(targetPosition, 2).Where(a => a.LogicalPosition == targetPosition);
		}

		public IEnumerable<ItemData> DetectItems(Vector2Int targetPosition, float cellsRangeInArea)
		{
			Collider2D[] hitColliders = GetCollidersInRange(targetPosition, cellsRangeInArea, new[] {LayerNames.Item});
			IEnumerable<ItemData> itemsHit = hitColliders.Select(c => c.GetComponent<ItemBehaviour>().ItemData);
			return itemsHit;
		}

		public IEnumerable<ItemData> DetectItems(Vector2Int targetPosition)
		{
			return DetectItems(targetPosition, CellSizeEpsilon);
		}

		private Collider2D[] GetCollidersInRange(Vector2Int targetPosition, float cellsRangeInVision, IEnumerable<string> layerNames)
		{
			Vector3 targetWorldPosition2D = _gridInfoProvider.GetCellCenterWorld(targetPosition);
			int layerMask = 0;
			foreach (string layerName in layerNames)
			{
				layerMask |= 1 << LayerMask.NameToLayer(layerName);
			}
			float raycastLength = cellsRangeInVision * _gridInfoProvider.CellSize;
			Collider2D[] hitColliders = Physics2D.OverlapCircleAll(targetWorldPosition2D, raycastLength, layerMask);
			return hitColliders;
		}
	}
}