using UnityEngine;

namespace Assets.Scripts.GridRelated
{
	public interface ITileVisibilityUpdater
	{
		void UpdateTileVisibility(Vector2Int actorPosition, int cellsRangeInVision);
	}
}