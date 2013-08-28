using System;
using Blarg.GameFramework.Support;
using Blarg.GameFramework.TileMap.Meshes;

namespace Blarg.GameFramework.TileMap.Lighting
{
	public abstract class BaseTileMapLighter : ITileMapLighter
	{
		public abstract void Light(TileMap tileMap);

		protected void ResetLightValues(TileMap tileMap)
		{
			for (int y = 0; y < tileMap.Height; ++y)
			{
				for (int z = 0; z < tileMap.Depth; ++z)
				{
					for (int x = 0; x < tileMap.Width; ++x)
					{
						var tile = tileMap.Get(x, y, z);

						// sky lighting will be recalculated, and other types of light sources
						// info stays as they were
						tile.Flags = tile.Flags.ClearBit(Tile.FLAG_LIGHT_SKY);
						tile.SkyLight = 0;
						tile.TileLight = tileMap.AmbientLightValue;
					}
				}
			}
		}

		protected void CastSkyLightDown(TileMap tileMap)
		{
			// go through each vertical column one at a time from top to bottom
			for (int x = 0; x < tileMap.Width; ++x)
			{
				for (int z = 0; z < tileMap.Depth; ++z)
				{
					bool stillSkyLit = true;
					byte currentSkyLightValue = tileMap.SkyLightValue;

					for (int y = tileMap.Height - 1; y >= 0 && stillSkyLit; --y)
					{
						var tile = tileMap.Get(x, y, z);
						var mesh = tileMap.TileMeshes.Get(tile);
						if (mesh == null || (mesh != null && !mesh.IsOpaque(TileMesh.SIDE_TOP) && !mesh.IsOpaque(TileMesh.SIDE_BOTTOM)))
						{
							// tile is partially transparent or this tile is empty space
							tile.Flags = tile.Flags.SetBit(Tile.FLAG_LIGHT_SKY);

							if (mesh != null)
								currentSkyLightValue = Tile.AdjustLightForTranslucency(currentSkyLightValue, mesh.Translucency);

							tile.SkyLight = currentSkyLightValue;
						}
						else
						{
							// tile is present and is fully solid, sky lighting stops
							// at the tile above this one
							stillSkyLit = false;
						}
					}
				}
			}
		}
	}
}

