using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.GameCore;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.GridRelated.TilemapAffecting
{
	public class TilemapAffector : ITilemapAffector
	{
		private Dictionary<TilemapType, UnityEngine.Tilemaps.Tilemap> _tilemapTypesToTilemaps;
		private bool _tilemapsInitialized;
		private readonly IGameContext _gameContext;

		public TilemapAffector(IGameContext gameContext)
		{
			_gameContext = gameContext;
		}

		public void SetColorOnTilemap(TilemapType tilemapType, Vector2Int tilePosition, Color newTileColor)
		{
			if (!_tilemapsInitialized) InitializeTilemaps();

			TilemapType tilemapTypeFlagged = tilemapType | TilemapType.None;
			IEnumerable<UnityEngine.Tilemaps.Tilemap> tilemapsToModify = _tilemapTypesToTilemaps.Where(kvp => tilemapTypeFlagged.HasFlag(kvp.Key)).Select(kvp => kvp.Value);
			foreach (var tilemap in tilemapsToModify)
			{
				tilemap.SetTileFlags(tilePosition.ToVector3Int(), TileFlags.None);
				tilemap.SetColor(tilePosition.ToVector3Int(), newTileColor);
			}
		}

		// todo: looks like overengineering — maybe list of tilemaps should be passed to SetColorOnTilemap?
		private void InitializeTilemaps()
		{
			_tilemapTypesToTilemaps = new Dictionary<TilemapType, UnityEngine.Tilemaps.Tilemap>
			{
				{TilemapType.Dirt, _gameContext.DirtTilemap},
				{TilemapType.Floors, _gameContext.FloorsTilemap},
				{TilemapType.Walls, _gameContext.WallsTilemap},
				{TilemapType.Environment, _gameContext.EnvironmentTilemap},
				{TilemapType.Objects, _gameContext.ObjectsTilemap},
			};
			_tilemapsInitialized = true;
		}
	}
}