using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GridRelated;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop
{
	public class PlayerSpaceResolver : IPlayerSpaceResolver
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IEntityDetector _entityDetector;

		public PlayerSpaceResolver(IGridInfoProvider gridInfoProvider, IEntityDetector entityDetector)
		{
			_gridInfoProvider = gridInfoProvider;
			_entityDetector = entityDetector;
		}

		public bool ResolveIfPlayerHasLittleSpace(ActorData actorData)
		{
			int emptyPositionsAround = GetEmptyPositionsAround(actorData);
			return emptyPositionsAround <= 5;
		}

		private int GetEmptyPositionsAround(ActorData actorData)
		{
			int emptyPositions = 8;
			var positionsAround = Vector2IntUtilities.Neighbours8(actorData.LogicalPosition);
			foreach (Vector2Int position in positionsAround)
			{
				if (!_gridInfoProvider.IsWalkable(position) || _entityDetector.DetectActors(position).Any())
				{
					--emptyPositions;
				}
			}
			return emptyPositions;
		}
	}
}