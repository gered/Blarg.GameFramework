using System;
using System.IO;

namespace Blarg.GameFramework.TileMap
{
	public static class TileDataSerializer
	{
		public static void Serialize(TileRawDataContainer src, BinaryWriter writer)
		{
			var tiles = src.Data;
			for (int i = 0; i < tiles.Length; ++i)
				Serialize(tiles[i], writer);
		}

		public static void Deserialize(BinaryReader reader, TileRawDataContainer dest)
		{
			var tiles = dest.Data;
			for (int i = 0; i < tiles.Length; ++i)
				Deserialize(reader, tiles[i]);
		}

		public static void Serialize(Tile src, BinaryWriter writer)
		{
			writer.Write(src.TileIndex);
			writer.Write(src.Flags);
			writer.Write(src.TileLight);
			writer.Write(src.SkyLight);
			writer.Write(src.Rotation);
			writer.Write(src.ParentTileOffsetX);
			writer.Write(src.ParentTileOffsetY);
			writer.Write(src.ParentTileOffsetZ);
			writer.Write(src.ParentTileWidth);
			writer.Write(src.ParentTileHeight);
			writer.Write(src.ParentTileDepth);
			writer.Write(src.Color);
		}

		public static void Deserialize(BinaryReader reader, Tile dest)
		{
			dest.TileIndex = reader.ReadInt16();
			dest.Flags = reader.ReadInt16();
			dest.TileLight = reader.ReadByte();
			dest.SkyLight = reader.ReadByte();
			dest.Rotation = reader.ReadByte();
			dest.ParentTileOffsetX = reader.ReadByte();
			dest.ParentTileOffsetY = reader.ReadByte();
			dest.ParentTileOffsetZ = reader.ReadByte();
			dest.ParentTileWidth = reader.ReadByte();
			dest.ParentTileHeight = reader.ReadByte();
			dest.ParentTileDepth = reader.ReadByte();
			dest.Color = reader.ReadInt32();
		}
	}
}

