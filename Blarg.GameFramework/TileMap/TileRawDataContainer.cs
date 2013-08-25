using System;

namespace Blarg.GameFramework.TileMap
{
	public interface TileRawDataContainer
	{
		Tile[] Data { get; }

		int Width { get; }
		int Height { get; }
		int Depth { get; }

		Tile Get(int x, int y, int z);
		Tile GetSafe(int x, int y, int z);
	}
}

