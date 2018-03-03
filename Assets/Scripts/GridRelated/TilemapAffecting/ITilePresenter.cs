using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GridRelated.TilemapAffecting
{
	public interface ITilePresenter
	{
		void Illuminate(HashSet<Vector2Int> visiblePositions);
	}
}