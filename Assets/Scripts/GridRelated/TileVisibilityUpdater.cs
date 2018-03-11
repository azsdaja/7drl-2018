using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.FieldOfView;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GridRelated.TilemapAffecting;
using Assets.Scripts.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.GridRelated
{
	public class TileVisibilityUpdater : ITileVisibilityUpdater
	{
		private readonly IGridInfoProvider _gridInfoProvider;
		private readonly IFovCalculator _fovCalculator;
		private readonly ITilePresenter _tilePresenter;
		private readonly IEntityPresenter _entityPresenter;
		private readonly IEntityDetector _entityDetector;

		public TileVisibilityUpdater(IGridInfoProvider gridInfoProvider, IFovCalculator fovCalculator, 
			ITilePresenter tilePresenter, IEntityPresenter entityPresenter, IEntityDetector entityDetector)
		{
			_fovCalculator = fovCalculator;
			_tilePresenter = tilePresenter;
			_entityPresenter = entityPresenter;
			_entityDetector = entityDetector;
			_gridInfoProvider = gridInfoProvider;
		}

		public void UpdateTileVisibility(Vector2Int actorPosition, int cellsRangeInVision)
		{
			List<IGameEntity> entitiesHit = _entityDetector.DetectEntities(actorPosition, cellsRangeInVision).ToList();

			Vector2Int fovCenter = actorPosition;
			HashSet<Vector2Int> visibleTiles = _fovCalculator.CalculateFov(fovCenter, cellsRangeInVision, _gridInfoProvider.IsPassingLight);

			_tilePresenter.Illuminate(visibleTiles);
			_entityPresenter.Illuminate(visibleTiles, entitiesHit);
		}
	}
}