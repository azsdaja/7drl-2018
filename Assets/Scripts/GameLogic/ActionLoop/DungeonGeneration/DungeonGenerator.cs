﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.CSharpUtilities;
using Assets.Scripts.RNG;
using UnityEngine;

namespace Assets.Scripts.GameLogic.ActionLoop.DungeonGeneration
{
	public class Dungeon
	{

		// misc. messages to print
		const string MsgXSize = "X size of dungeon: \t";

		const string MsgYSize = "Y size of dungeon: \t";

		const string MsgMaxObjects = "max # of objects: \t";

		const string MsgNumObjects = "# of objects made: \t";

		// max size of the map
		int xmax = 300; //columns
		int ymax = 300; //rows

		// size of the map
		int _xsize;
		int _ysize;

		// number of "objects" to generate on the map
		int _objects;

		// define the %chance to generate either a room or a corridor on the map
		// BTW, rooms are 1st priority so actually it's enough to just define the chance
		// of generating a room
		const int ChanceRoom = 75;

		// our map
		GenTile[] _dungeonMap = { };

		private readonly IList<BoundsInt> _rooms = new List<BoundsInt>();

		readonly IRandomNumberGenerator _rnd;

		readonly Action<string> _logger;
		private Vector3Int _offset;
		private int _roomSizeMaxX = 10;
		private int _roomSizeMaxY = 10;
		private Vector2Int _stairsLocation;
		
		public Dungeon(IRandomNumberGenerator rnd, Action<string> logger)
		{
			_rnd = rnd;
			_logger = logger;
		}

		public int Corridors
		{
			get;
			private set;
		}

		public IList<BoundsInt> Rooms
		{
			get { return _rooms; }
		}

		public Vector2Int StairsLocation
		{
			get { return _stairsLocation; }
			set { _stairsLocation = value; }
		}

		public static bool IsWall(int x, int y, int xlen, int ylen, int xt, int yt, Direction d)
		{
			Func<int, int, int> a = GetFeatureLowerBound;

			Func<int, int, int> b = IsFeatureWallBound;
			switch (d)
			{
				case Direction.North:
					return xt == a(x, xlen) || xt == b(x, xlen) || yt == y || yt == y - ylen + 1;
				case Direction.East:
					return xt == x || xt == x + xlen - 1 || yt == a(y, ylen) || yt == b(y, ylen);
				case Direction.South:
					return xt == a(x, xlen) || xt == b(x, xlen) || yt == y || yt == y + ylen - 1;
				case Direction.West:
					return xt == x || xt == x - xlen + 1 || yt == a(y, ylen) || yt == b(y, ylen);
			}

			throw new InvalidOperationException();
		}

		public static int GetFeatureLowerBound(int c, int len)
		{
			return c - len / 2;
		}

		public static int IsFeatureWallBound(int c, int len)
		{
			return c + (len - 1) / 2;
		}

		public static int GetFeatureUpperBound(int c, int len)
		{
			return c + (len + 1) / 2;
		}

		public static IEnumerable<Vector2Int> GetRoomPoints(int x, int y, int xlen, int ylen, Direction d)
		{
			// north and south share the same x strategy
			// east and west share the same y strategy
			Func<int, int, int> a = GetFeatureLowerBound;
			Func<int, int, int> b = GetFeatureUpperBound;

			switch (d)
			{
				case Direction.North:
					for (var xt = a(x, xlen); xt < b(x, xlen); xt++) for (var yt = y; yt > y - ylen; yt--) yield return new Vector2Int(xt, yt);
					break;
				case Direction.East:
					for (var xt = x; xt < x + xlen; xt++) for (var yt = a(y, ylen); yt < b(y, ylen); yt++) yield return new Vector2Int(xt, yt);
					break;
				case Direction.South:
					for (var xt = a(x, xlen); xt < b(x, xlen); xt++) for (var yt = y; yt < y + ylen; yt++) yield return new Vector2Int(xt, yt);
					break;
				case Direction.West:
					for (var xt = x; xt > x - xlen; xt--) for (var yt = a(y, ylen); yt < b(y, ylen); yt++) yield return new Vector2Int(xt, yt);
					break;
				default:
					yield break;
			}
		}

		public GenTile GetCellType(int x, int y)
		{
			try
			{
				int xAfterOffset = x - _offset.x;
				int yAfterOffset = y - _offset.y;
				return this._dungeonMap[xAfterOffset + this._xsize * yAfterOffset];
			}
			catch (IndexOutOfRangeException)
			{
				//new { x, y }.Dump("exceptional");
				throw;
			}
		}

		private GenTile GetCellTypeNoOffset(int x, int y)
		{
			try
			{
				return this._dungeonMap[x + this._xsize * y];
			}
			catch (IndexOutOfRangeException)
			{
				//new { x, y }.Dump("exceptional");
				throw;
			}
		}

		public int GetRand(int min, int max)
		{
			return _rnd.Next(min, max+1);
		}

		public bool MakeCorridor(int x, int y, int length, Direction direction)
		{
			// define the dimensions of the corridor (er.. only the width and height..)
			int len = this.GetRand(2, length);
			const GenTile Floor = GenTile.Corridor;

			int xtemp;
			int ytemp = 0;

			switch (direction)
			{
				case Direction.North:
					// north
					// check if there's enough space for the corridor
					// start with checking it's not out of the boundaries
					if (x < 0 || x > this._xsize) return false;
					xtemp = x;

					// same thing here, to make sure it's not out of the boundaries
					for (ytemp = y; ytemp > (y - len); ytemp--)
					{
						if (ytemp < 0 || ytemp > this._ysize) return false; // oh boho, it was!
						if (GetCellTypeNoOffset(xtemp, ytemp) != GenTile.Unused) return false;
					}

					// if we're still here, let's start building
					Corridors++;
					for (ytemp = y; ytemp > (y - len); ytemp--)
					{
						this.SetCell(xtemp, ytemp, Floor);
					}

					break;

				case Direction.East:
					// east
					if (y < 0 || y > this._ysize) return false;
					ytemp = y;

					for (xtemp = x; xtemp < (x + len); xtemp++)
					{
						if (xtemp < 0 || xtemp > this._xsize) return false;
						if (GetCellTypeNoOffset(xtemp, ytemp) != GenTile.Unused) return false;
					}

					Corridors++;
					for (xtemp = x; xtemp < (x + len); xtemp++)
					{
						this.SetCell(xtemp, ytemp, Floor);
					}

					break;

				case Direction.South:
					// south
					if (x < 0 || x > this._xsize) return false;
					xtemp = x;

					for (ytemp = y; ytemp < (y + len); ytemp++)
					{
						if (ytemp < 0 || ytemp > this._ysize) return false;
						if (GetCellTypeNoOffset(xtemp, ytemp) != GenTile.Unused) return false;
					}

					Corridors++;
					for (ytemp = y; ytemp < (y + len); ytemp++)
					{
						this.SetCell(xtemp, ytemp, Floor);
					}

					break;
				case Direction.West:
					// west
					if (ytemp < 0 || ytemp > this._ysize) return false;
					ytemp = y;

					for (xtemp = x; xtemp > (x - len); xtemp--)
					{
						if (xtemp < 0 || xtemp > this._xsize) return false;
						if (GetCellTypeNoOffset(xtemp, ytemp) != GenTile.Unused) return false;
					}

					Corridors++;
					for (xtemp = x; xtemp > (x - len); xtemp--)
					{
						this.SetCell(xtemp, ytemp, Floor);
					}

					break;
			}

			// woot, we're still here! let's tell the other guys we're done!!
			return true;
		}

		public IEnumerable<Tuple<Vector2Int, Direction>> GetSurroundingPoints(Vector2Int v)
		{
			var points = new[]
								  {
											Tuple.Create(new Vector2Int(v.x, v.y + 1), Direction.North),
											Tuple.Create(new Vector2Int(v.x - 1, v.y), Direction.East),
											Tuple.Create(new Vector2Int(v.x, v.y - 1), Direction.South),
											Tuple.Create(new Vector2Int(v.x + 1, v.y), Direction.West),

									  };
			return points.Where(p => InBounds(p.Item1));
		}

		public IEnumerable<Tuple<Vector2Int, Tuple<Direction, GenTile>>> GetSurroundings(Vector2Int v)
		{
			return
				 this.GetSurroundingPoints(v)
					  .Select(r => Tuple.Create(r.Item1, Tuple.Create(r.Item2, this.GetCellTypeNoOffset(r.Item1.x, r.Item1.y))));
		}

		public bool InBounds(int x, int y)
		{
			return x > 0 && x < this.xmax && y > 0 && y < this.ymax;
		}

		public bool InBounds(Vector2Int v)
		{
			return this.InBounds(v.x, v.y);
		}

		public bool  MakeRoom(int x, int y, int xlength, int ylength, Direction direction)
		{
			// define the dimensions of the room, it should be at least 4x4 tiles (2x2 for walking on, the rest is walls)
			int xlen = this.GetRand(4, xlength);
			int ylen = this.GetRand(4, ylength);

			// the tile type it's going to be filled with
			const GenTile Floor = GenTile.DirtFloor;

			const GenTile Wall = GenTile.DirtWall;
			// choose the way it's Vector2Intng at

			var points = GetRoomPoints(x, y, xlen, ylen, direction).ToArray();

			// Check if there's enough space left for it
			if (
				 points.Any(
					  s =>
					  s.y < 0 || s.y > this._ysize || s.x < 0 || s.x > this._xsize || this.GetCellTypeNoOffset(s.x, s.y) != GenTile.Unused)) return false;
			_logger(
						 string.Format(
							  "Making room:int x={0}, int y={1}, int xlength={2}, int ylength={3}, int direction={4}",
							  x,
							  y,
							  xlength,
							  ylength,
							  direction));

			foreach (var p in points)
			{
				this.SetCell(p.x, p.y, IsWall(x, y, xlen, ylen, p.x, p.y, direction) ? Wall : Floor);
			}
			Vector2Int minPoint = points.Aggregate((previous,current) => previous.x+previous.y < current.x+current.y ? previous : current);
			Vector2Int maxPoint = points.Aggregate((previous,current) => previous.x+previous.y > current.x+current.y ? previous : current);

			BoundsInt roomBoundsAfterOffset = 
				new BoundsInt
				{
					min = new Vector3Int(minPoint.x, minPoint.y, 0) + _offset,
					max = new Vector3Int(maxPoint.x+1, maxPoint.y+1, 1) + _offset
				};
			Rooms.Add(roomBoundsAfterOffset);

			// yay, all done
			return true;
		}

		public GenTile[] GetDungeon()
		{
			return this._dungeonMap;
		}

		public char GetCellTile(int x, int y)
		{
			switch (GetCellTypeNoOffset(x, y))
			{
				case GenTile.Unused:
					return ' ';
				case GenTile.DirtWall:
					return '#';
				case GenTile.DirtFloor:
					return '.';
				case GenTile.StoneWall:
					return '#';
				case GenTile.Corridor:
					return '_';
				case GenTile.Door:
					return '+';
				case GenTile.Upstairs:
					return '<';
				case GenTile.Downstairs:
					return '>';
				case GenTile.Chest:
					return 'C';
				default:
					throw new ArgumentOutOfRangeException("x,y");
			}
		}

		//used to print the map on the screen
		public void ShowDungeon()
		{
			for (int y = 0; y < this._ysize; y++)
			{
				for (int x = 0; x < this._xsize; x++)
				{
					Console.Write(GetCellTile(x, y));
				}

				if (this._xsize <= xmax) Console.WriteLine();
			}
		}

		public Direction RandomDirection()
		{
			int dir = this.GetRand(0, 3);
			switch (dir)
			{
				case 0:
					return Direction.North;
				case 1:
					return Direction.East;
				case 2:
					return Direction.South;
				case 3:
					return Direction.West;
				default:
					throw new InvalidOperationException();
			}
		}

		//and here's the one generating the whole map
		public bool CreateDungeon(Vector3Int gridBoundsMin, int inx, int iny, int inobj)
		{
			_offset = gridBoundsMin;
			this._objects = inobj < 1 ? 10 : inobj;

			// adjust the size of the map, if it's smaller or bigger than the limits
			if (inx < 3) this._xsize = 3;
			else if (inx > xmax) this._xsize = xmax;
			else this._xsize = inx;

			if (iny < 3) this._ysize = 3;
			else if (iny > ymax) this._ysize = ymax;
			else this._ysize = iny;

			Console.WriteLine(MsgXSize + this._xsize);
			Console.WriteLine(MsgYSize + this._ysize);
			Console.WriteLine(MsgMaxObjects + this._objects);

			// redefine the map var, so it's adjusted to our new map size
			this._dungeonMap = new GenTile[this._xsize * this._ysize];

			// start with making the "standard stuff" on the map
			this.Initialize();

			/*******************************************************************************
			And now the code of the random-map-generation-algorithm begins!
			*******************************************************************************/

			// start with making a room in the middle, which we can start building upon
			this.MakeRoom(this._xsize / 2, this._ysize / 2, _roomSizeMaxX, _roomSizeMaxY, RandomDirection()); // getrand saken f????r att slumpa fram riktning p?? rummet

			// keep count of the number of "objects" we've made
			int currentFeatures = 1; // +1 for the first room we just made

			// then we sart the main loop
			for (int countingTries = 0; countingTries < 1000; countingTries++)
			{
				// check if we've reached our quota
				if (currentFeatures == this._objects)
				{
					break;
				}

				// start with a random wall
				int newx = 0;
				int xmod = 0;
				int newy = 0;
				int ymod = 0;
				Direction? validTile = null;

				// 1000 chances to find a suitable object (room or corridor)..
				for (int testing = 0; testing < 1000; testing++)
				{
					newx = this.GetRand(1, this._xsize - 1);
					newy = this.GetRand(1, this._ysize - 1);

					if (GetCellTypeNoOffset(newx, newy) == GenTile.DirtWall || GetCellTypeNoOffset(newx, newy) == GenTile.Corridor)
					{
						var surroundings = this.GetSurroundings(new Vector2Int(newx, newy));

						// check if we can reach the place
						var canReach =
							 surroundings.FirstOrDefault(s => s.Item2.Item2 == GenTile.Corridor || s.Item2.Item2 == GenTile.DirtFloor);
						if (canReach == null)
						{
							continue;
						}
						validTile = canReach.Item2.Item1;
						switch (canReach.Item2.Item1)
						{
							case Direction.North:
								xmod = 0;
								ymod = -1;
								break;
							case Direction.East:
								xmod = 1;
								ymod = 0;
								break;
							case Direction.South:
								xmod = 0;
								ymod = 1;
								break;
							case Direction.West:
								xmod = -1;
								ymod = 0;
								break;
							default:
								throw new InvalidOperationException();
						}


						// check that we haven't got another door nearby, so we won't get alot of openings besides
						// each other

						if (GetCellTypeNoOffset(newx, newy + 1) == GenTile.Door) // north
						{
							validTile = null;

						}

						else if (GetCellTypeNoOffset(newx - 1, newy) == GenTile.Door) // east
							validTile = null;
						else if (GetCellTypeNoOffset(newx, newy - 1) == GenTile.Door) // south
							validTile = null;
						else if (GetCellTypeNoOffset(newx + 1, newy) == GenTile.Door) // west
							validTile = null;


						// if we can, jump out of the loop and continue with the rest
						if (validTile.HasValue) break;
					}
				}

				if (validTile.HasValue)
				{
					// choose what to build now at our newly found place, and at what direction
					int feature = this.GetRand(0, 100);
					if (feature <= ChanceRoom)
					{ // a new room
						if (this.MakeRoom(newx + xmod, newy + ymod, 8, 6, validTile.Value))
						{
							currentFeatures++; // add to our quota

							// then we mark the wall opening with a door
							this.SetCell(newx, newy, GenTile.Door);

							// clean up infront of the door so we can reach it
							this.SetCell(newx + xmod, newy + ymod, GenTile.DirtFloor);
						}
					}
					else if (feature >= ChanceRoom)
					{ // new corridor
						if (this.MakeCorridor(newx + xmod, newy + ymod, 6, validTile.Value))
						{
							// same thing here, add to the quota and a door
							currentFeatures++;

							this.SetCell(newx, newy, GenTile.Door);
						}
					}
				}
			}

			/*******************************************************************************
			All done with the building, let's finish this one off
			*******************************************************************************/
			AddSprinkles();

			// all done with the map generation, tell the user about it and finish
			Console.WriteLine(MsgNumObjects + currentFeatures);

			for (int y = 0; y < this._ysize; y++)
			{
				for (int x = 0; x < this._xsize; x++)
				{
					if (GetCellTypeNoOffset(x, y) == GenTile.Corridor)
					{
						List<Vector2Int> neighbours = Vector2IntUtilities.Neighbours8(new Vector2Int(x, y));
						foreach (Vector2Int neighbour in neighbours)
						{
							GenTile neighbourCell = GetCellTypeNoOffset(neighbour.x, neighbour.y);
							if (neighbourCell == GenTile.Unused)
							{
								SetCell(neighbour.x, neighbour.y, GenTile.StoneWall);
							}
						}
					}
				}
			}

			return true;
		}

		void Initialize()
		{
			for (int y = 0; y < this._ysize; y++)
			{
				for (int x = 0; x < this._xsize; x++)
				{
					// ie, making the borders of unwalkable walls
					if (y == 0 || y == this._ysize - 1 || x == 0 || x == this._xsize - 1)
					{
						this.SetCell(x, y, GenTile.StoneWall);
					}
					else
					{                        // and fill the rest with dirt
						this.SetCell(x, y, GenTile.Unused);
					}
				}
			}
		}

		// setting a tile's type
		void SetCell(int x, int y, GenTile celltype)
		{
			this._dungeonMap[x + this._xsize * y] = celltype;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		void AddSprinkles()
		{
			// sprinkle out the bonusstuff (stairs, chests etc.) over the map
			int state = 0; // the state the loop is in, start with the stairs
			while (state != 10)
			{
				for (int testing = 0; testing < 1000; testing++)
				{
					var newx = this.GetRand(1, this._xsize - 1);
					int newy = this.GetRand(1, this._ysize - 2);

					// Console.WriteLine("x: " + newx + "\ty: " + newy);
					int ways = 4; // from how many directions we can reach the random spot from

					// check if we can reach the spot
					if (GetCellTypeNoOffset(newx, newy + 1) == GenTile.DirtFloor || GetCellTypeNoOffset(newx, newy + 1) == GenTile.Corridor)
					{
						// north
						if (GetCellTypeNoOffset(newx, newy + 1) != GenTile.Door)
							ways--;
					}

					if (GetCellTypeNoOffset(newx - 1, newy) == GenTile.DirtFloor || GetCellTypeNoOffset(newx - 1, newy) == GenTile.Corridor)
					{
						// east
						if (GetCellTypeNoOffset(newx - 1, newy) != GenTile.Door)
							ways--;
					}

					if (GetCellTypeNoOffset(newx, newy - 1) == GenTile.DirtFloor || GetCellTypeNoOffset(newx, newy - 1) == GenTile.Corridor)
					{
						// south
						if (GetCellTypeNoOffset(newx, newy - 1) != GenTile.Door)
							ways--;
					}

					if (GetCellTypeNoOffset(newx + 1, newy) == GenTile.DirtFloor || GetCellTypeNoOffset(newx + 1, newy) == GenTile.Corridor)
					{
						// west
						if (GetCellTypeNoOffset(newx + 1, newy) != GenTile.Door)
							ways--;
					}

					if (state == 0)
					{
						if (ways == 0)
						{
							// we're in state 0, let's place a "upstairs" thing
							this.SetCell(newx, newy, GenTile.Upstairs);
							StairsLocation = new Vector2Int(newx, newy) + _offset.ToVector2Int();
							state = 1;
							break;
						}
					}
					else if (state == 1)
					{
						if (ways == 0)
						{
							// state 1, place a "downstairs"
							this.SetCell(newx, newy, GenTile.Downstairs);
							state = 10;
							break;
						}
					}
				}
			}
		}
	}

	public enum GenTile
	{
		Downstairs,
		Upstairs,
		Door,
		DirtFloor,
		Corridor,
		Unused,
		DirtWall,
		StoneWall,
		Chest
	}

	public enum Direction
	{
		Default, North, South, West, East
	}
}
