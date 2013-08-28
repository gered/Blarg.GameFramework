using System;
using Blarg.GameFramework.TileMap.Meshes;

namespace Blarg.GameFramework.TileMap.Lighting
{
	public class LightSpreadingTileMapLighter : BaseTileMapLighter
	{
		bool _doSkyLight;
		bool _doTileLight;

		public LightSpreadingTileMapLighter(bool doSkyLight, bool doTileLight)
		{
			_doSkyLight = doSkyLight;
			_doTileLight = doTileLight;
		}

		public LightSpreadingTileMapLighter()
			: this(true, true)
		{
		}

		private void spreadSkyLight(int x, int y, int z, Tile tile, byte light, TileMap tileMap)
		{
			if (light > 0)
			{
				tile.SkyLight = light;
				--light;

				var left = tileMap.GetSafe(x - 1, y, z);
				var right = tileMap.GetSafe(x + 1, y, z);
				var forward = tileMap.GetSafe(x, y, z - 1);
				var backward = tileMap.GetSafe(x, y, z + 1);
				var up = tileMap.GetSafe(x, y + 1, z);
				var down = tileMap.GetSafe(x, y - 1, z);

				if (left != null && (left.IsEmptySpace || !tileMap.TileMeshes.Get(left).IsOpaque(TileMesh.SIDE_RIGHT)) && left.SkyLight < light)
				{
					byte spreadLight = light;
					if (!left.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(left).Translucency);
					spreadSkyLight(x - 1, y, z, left, spreadLight, tileMap);
				}
				if (right != null && (right.IsEmptySpace || !tileMap.TileMeshes.Get(right).IsOpaque(TileMesh.SIDE_LEFT)) && right.SkyLight < light)
				{
					byte spreadLight = light;
					if (!right.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(right).Translucency);
					spreadSkyLight(x + 1, y, z, right, spreadLight, tileMap);
				}
				if (forward != null && (forward.IsEmptySpace || !tileMap.TileMeshes.Get(forward).IsOpaque(TileMesh.SIDE_BACK)) && forward.SkyLight < light)
				{
					byte spreadLight = light;
					if (!forward.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(forward).Translucency);
					spreadSkyLight(x, y, z - 1, forward, spreadLight, tileMap);
				}
				if (backward != null && (backward.IsEmptySpace || !tileMap.TileMeshes.Get(backward).IsOpaque(TileMesh.SIDE_FRONT)) && backward.SkyLight < light)
				{
					byte spreadLight = light;
					if (!backward.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(backward).Translucency);
					spreadSkyLight(x, y, z + 1, backward, spreadLight, tileMap);
				}
				if (up != null && (up.IsEmptySpace || !tileMap.TileMeshes.Get(up).IsOpaque(TileMesh.SIDE_BOTTOM)) && up.SkyLight < light)
				{
					byte spreadLight = light;
					if (!up.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(up).Translucency);
					spreadSkyLight(x, y + 1, z, up, spreadLight, tileMap);
				}
				if (down != null && (down.IsEmptySpace || !tileMap.TileMeshes.Get(down).IsOpaque(TileMesh.SIDE_TOP)) && down.SkyLight < light)
				{
					byte spreadLight = light;
					if (!down.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(down).Translucency);
					spreadSkyLight(x, y - 1, z, down, spreadLight, tileMap);
				}
			}
		}

		private void spreadTileLight(int x, int y, int z, Tile tile, byte light, TileMap tileMap)
		{
			if (light > 0)
			{
				tile.TileLight = light;
				--light;

				Tile left = tileMap.GetSafe(x - 1, y, z);
				Tile right = tileMap.GetSafe(x + 1, y, z);
				Tile forward = tileMap.GetSafe(x, y, z - 1);
				Tile backward = tileMap.GetSafe(x, y, z + 1);
				Tile up = tileMap.GetSafe(x, y + 1, z);
				Tile down = tileMap.GetSafe(x, y - 1, z);

				if (left != null && (left.IsEmptySpace || !tileMap.TileMeshes.Get(left).IsOpaque(TileMesh.SIDE_RIGHT)) && left.TileLight < light)
				{
					byte spreadLight = light;
					if (!left.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(left).Translucency);
					spreadTileLight(x - 1, y, z, left, spreadLight, tileMap);
				}
				if (right != null && (right.IsEmptySpace || !tileMap.TileMeshes.Get(right).IsOpaque(TileMesh.SIDE_LEFT)) && right.TileLight < light)
				{
					byte spreadLight = light;
					if (!right.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(right).Translucency);
					spreadTileLight(x + 1, y, z, right, spreadLight, tileMap);
				}
				if (forward != null && (forward.IsEmptySpace || !tileMap.TileMeshes.Get(forward).IsOpaque(TileMesh.SIDE_BACK)) && forward.TileLight < light)
				{
					byte spreadLight = light;
					if (!forward.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(forward).Translucency);
					spreadTileLight(x, y, z - 1, forward, spreadLight, tileMap);
				}
				if (backward != null && (backward.IsEmptySpace || !tileMap.TileMeshes.Get(backward).IsOpaque(TileMesh.SIDE_FRONT)) && backward.TileLight < light)
				{
					byte spreadLight = light;
					if (!backward.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(backward).Translucency);
					spreadTileLight(x, y, z + 1, backward, spreadLight, tileMap);
				}
				if (up != null && (up.IsEmptySpace || !tileMap.TileMeshes.Get(up).IsOpaque(TileMesh.SIDE_BOTTOM)) && up.TileLight < light)
				{
					byte spreadLight = light;
					if (!up.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(up).Translucency);
					spreadTileLight(x, y + 1, z, up, spreadLight, tileMap);
				}
				if (down != null && (down.IsEmptySpace || !tileMap.TileMeshes.Get(down).IsOpaque(TileMesh.SIDE_TOP)) && down.TileLight < light)
				{
					byte spreadLight = light;
					if (!down.IsEmptySpace)
						spreadLight = Tile.AdjustLightForTranslucency(spreadLight, tileMap.TileMeshes.Get(down).Translucency);
					spreadTileLight(x, y - 1, z, down, spreadLight, tileMap);
				}
			}
		}

		public override void Light(TileMap tileMap)
		{
			ResetLightValues(tileMap);

			if (_doSkyLight)
				CastSkyLightDown(tileMap);

			// for each light source (sky or not), recursively go through and set
			// appropriate lighting for each adjacent tile
			for (int y = 0; y < tileMap.Height; ++y)
			{
				for (int z = 0; z < tileMap.Depth; ++z)
				{
					for (int x = 0; x < tileMap.Width; ++x)
					{
						Tile tile = tileMap.Get(x, y, z);
						if (tile.IsEmptySpace)
						{
							if (_doSkyLight && tile.IsSkyLit)
								spreadSkyLight(x, y, z, tile, tile.SkyLight, tileMap);
						}
						else
						{
							if (_doTileLight)
							{
								TileMesh mesh = tileMap.TileMeshes.Get(tile);
								if (mesh.IsLightSource)
									spreadTileLight(x, y, z, tile, mesh.LightValue, tileMap);
							}
						}
					}
				}
			}
		}
	}
}

