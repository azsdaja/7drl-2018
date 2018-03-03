using System;

namespace Assets.Scripts.GridRelated.TilemapAffecting
{
	[Flags]
	public enum TilemapType
	{
		None = 0,
		Dirt = 1,
		Floors = 2,
		Walls = 4,
		Environment = 8,
	}
}