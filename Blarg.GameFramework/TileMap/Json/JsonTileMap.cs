using System;
using System.Collections.Generic;

namespace Blarg.GameFramework.TileMap.Json
{
	public class JsonTileMap
	{
		public int ChunkWidth;
		public int ChunkHeight;
		public int ChunkDepth;
		public int WidthInChunks;
		public int HeightInChunks;
		public int DepthInChunks;
		public string LightingMode;
		public List<string> Chunks;
	}
}

