using System;
using Blarg.GameFramework.TileMap.Meshes;
using Blarg.GameFramework.TileMap;
using System.IO;
using Newtonsoft.Json;
using Blarg.GameFramework.TileMap.Lighting;

namespace Blarg.GameFramework.TileMap.Json
{
	public static class TileMapLoader
	{
		public static TileMap Load(string file, TileMeshCollection tileMeshes)
		{
			var stream = Framework.FileSystem.Open(file);
			string path = null;
			if (file.Contains("/"))
				path = file.Substring(0, file.LastIndexOf('/') + 1);

			return Load(stream, tileMeshes, path);
		}

		public static TileMap Load(Stream file, TileMeshCollection tileMeshes, string path = null)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			if (tileMeshes == null)
				throw new ArgumentNullException("tileMeshes");

			var reader = new StreamReader(file);
			var map = JsonConvert.DeserializeObject<JsonTileMap>(reader.ReadToEnd());

			if (map.Chunks == null || map.Chunks.Count == 0)
				throw new ConfigFileException("Invalid map: no chunks.");

			int numChunks = map.WidthInChunks * map.HeightInChunks * map.DepthInChunks;
			if (map.Chunks.Count != numChunks)
				throw new ConfigFileException("Inconsistent map dimensions and actual number of chunks found.");

			ChunkVertexGenerator chunkVertexGenerator = null;
			ITileMapLighter lighter = null;

			if (String.IsNullOrEmpty(map.LightingMode))
				chunkVertexGenerator = new ChunkVertexGenerator();
			else if (map.LightingMode.ToLower() == "simple")
				throw new NotImplementedException();
			else if (map.LightingMode.ToLower() == "skyAndSources")
				throw new NotImplementedException();
			else
				throw new ConfigFileException("Invalid lighting mode.");

			var tileMap = new TileMap(map.ChunkWidth, map.ChunkHeight, map.ChunkDepth,
			                          map.WidthInChunks, map.HeightInChunks, map.DepthInChunks,
			                          tileMeshes,
			                          chunkVertexGenerator,
			                          lighter);

			for (int i = 0; i < numChunks; ++i)
			{
				string encodedChunk = map.Chunks[i];
				var outputChunk = tileMap.Chunks[i];

				var buffer = new MemoryStream(Convert.FromBase64String(encodedChunk));
				using (var byteReader = new BinaryReader(buffer))
				{
					TileDataSerializer.Deserialize(byteReader, outputChunk);
				}
			}

			return tileMap;
		}
	}
}

