using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration;
using UnityEngine;

namespace Assets.Scripts.GridRelated.TilemapAffecting
{
	public class FurthestRoomToStairsResolver
	{
		public static BoundsInt GetFurthestRoomToStairs(Dungeon currentDungeon)
		{
			var roomsSortedByDistanceToStairs = currentDungeon.Rooms.ToList();
			roomsSortedByDistanceToStairs.Sort(
				(first, second) => Vector2IntUtilities.WalkDistance(currentDungeon.StairsLocation, first.min.ToVector2Int())
					.CompareTo(
						Vector2IntUtilities.WalkDistance(currentDungeon.StairsLocation, second.min.ToVector2Int()))
			);
			BoundsInt furthestRoomToStairs = roomsSortedByDistanceToStairs.Last();
			return furthestRoomToStairs;
		}
	}
}