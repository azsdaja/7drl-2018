using System.Collections.Generic;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;

namespace Assets.Scripts.GridRelated.TilemapAffecting
{
	public class TilePresenter : ITilePresenter
	{
		private readonly ITilemapAffector _tilemapAffector;
		private readonly IGameContext _gameContext;


		public TilePresenter(ITilemapAffector tilemapAffector, IGameContext gameContext)
		{
			_tilemapAffector = tilemapAffector;
			_gameContext = gameContext;
		}

		internal HashSet<Vector2Int> LitPositionsSaved
		{
			get { return _gameContext.VisiblePositions; }
		}

		public void Illuminate(HashSet<Vector2Int> visiblePositions)
		{
			var newlyLitPositions = new HashSet<Vector2Int>();
			foreach (Vector2Int oldPosition in LitPositionsSaved)
			{
				if (visiblePositions.Contains(oldPosition)) continue;
				_tilemapAffector.SetColorOnTilemap(TilemapType.Floors | TilemapType.Walls | TilemapType.Environment | TilemapType.Dirt, 
					oldPosition, TileColors.UnlitColor);
			}
			foreach (Vector2Int currentPosition in visiblePositions)
			{
				if (LitPositionsSaved.Contains(currentPosition)) continue;
				newlyLitPositions.Add(currentPosition);
				_tilemapAffector.SetColorOnTilemap(TilemapType.Floors | TilemapType.Walls | TilemapType.Environment | TilemapType.Dirt, 
					currentPosition, TileColors.LitColor);
			}

			_gameContext.VisiblePositions = visiblePositions;
		}
	}
}