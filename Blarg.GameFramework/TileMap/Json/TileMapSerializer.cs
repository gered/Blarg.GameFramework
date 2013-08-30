using System;
using System.IO;
using Newtonsoft.Json;
using Blarg.GameFramework.IO;
using Blarg.GameFramework.TileMap;
using Blarg.GameFramework.TileMap.Lighting;
using Blarg.GameFramework.TileMap.Meshes;
using System.Collections.Generic;

namespace Blarg.GameFramework.TileMap.Json
{
	public static class TileMapSerializer
	{
		public static TileMap Load(string file, TileMeshCollection tileMeshes)
		{
			using (var stream = Framework.FileSystem.Open(file))
			{
				string path = null;
				if (file.Contains("/"))
					path = file.Substring(0, file.LastIndexOf('/') + 1);

				return Load(stream, tileMeshes, path);
			}
		}

		public static TileMap Load(Stream file, TileMeshCollection tileMeshes, string path = null)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			if (tileMeshes == null)
				throw new ArgumentNullException("tileMeshes");

			var reader = new StreamReader(file);
			var map = JsonConvert.DeserializeObject<JsonTileMap>(reader.ReadToEnd());
			reader.Dispose();

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

		public static void Save(TileMap tileMap, string file)
		{
			using (var stream = Framework.FileSystem.Open(file, FileOpenMode.Create))
			{
				Save(tileMap, stream);
			}
		}

		public static void Save(TileMap tileMap, Stream file)
		{
			if (tileMap == null)
				throw new ArgumentNullException("tileMap");
			if (file == null)
				throw new ArgumentNullException("file");

			var map = new JsonTileMap();
			map.ChunkWidth = tileMap.ChunkWidth;
			map.ChunkHeight = tileMap.ChunkHeight;
			map.ChunkDepth = tileMap.ChunkDepth;
			map.WidthInChunks = tileMap.WidthInChunks;
			map.HeightInChunks = tileMap.HeightInChunks;
			map.DepthInChunks = tileMap.DepthInChunks;

			// TODO: figure out real lighting mode from the types of vertex generator / lighter objects set
			map.LightingMode = null;

			// each serialized chunk will be the same size in bytes (same number of tiles in each)
			int chunkSizeInBytes = tileMap.Chunks[0].Data.Length * TileDataSerializer.TILE_SIZE_BYTES;

			map.Chunks = new List<string>(tileMap.Chunks.Length);
			for (int i = 0; i < tileMap.Chunks.Length; ++i)
			{
				var chunk = tileMap.Chunks[i];

				var buffer = new MemoryStream(chunkSizeInBytes);
				using (var byteWriter = new BinaryWriter(buffer))
				{
					TileDataSerializer.Serialize(chunk, byteWriter);
				}

				map.Chunks.Add(Convert.ToBase64String(buffer.ToArray()));
			}

			using (var writer = new StreamWriter(file))
			{
				writer.Write(JsonConvert.SerializeObject(map, Formatting.Indented));
			}
		}
	}
}

