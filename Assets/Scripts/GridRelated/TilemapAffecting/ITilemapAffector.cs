using UnityEngine;

namespace Assets.Scripts.GridRelated.TilemapAffecting
{
	public interface ITilemapAffector
	{
		void SetColorOnTilemap(TilemapType tilemapType, Vector2Int tilePosition, Color newTileColor);
	}
}