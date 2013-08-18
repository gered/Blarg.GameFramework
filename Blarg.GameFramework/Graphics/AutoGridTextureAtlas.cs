using System;

namespace Blarg.GameFramework.Graphics
{
	public class AutoGridTextureAtlas : TextureAtlas
	{
		public int TileWidth { get; private set; }
		public int TileHeight { get; private set; }

		public AutoGridTextureAtlas(int textureWidth, int textureHeight, int tileWidth, int tileHeight, int tileBorder = 0, float texCoordEdgeOffset = TexCoordEdgeBleedOffset)
			: base(textureWidth, textureHeight, texCoordEdgeOffset)
		{
			Generate(tileWidth, tileHeight, tileBorder);
		}

		public AutoGridTextureAtlas(Texture texture, int tileHeight, int tileWidth, int tileBorder = 0, float texCoordEdgeOffset = TexCoordEdgeBleedOffset)
			: base(texture, texCoordEdgeOffset)
		{
			Generate(tileWidth, tileHeight, tileBorder);
		}

		private void Generate(int tileWidth, int tileHeight, int tileBorder)
		{
			TileWidth = tileWidth;
			TileHeight = tileHeight;

			tileWidth += tileBorder;
			tileHeight += tileBorder;

			int tilesX = (Width - tileBorder) / (TileWidth + tileBorder);
			int tilesY = (Height - tileBorder) / (TileHeight + tileBorder);

			for (int y = 0; y < tilesY; ++y)
			{
				for (int x = 0; x < tilesX; ++x)
				{
					var tile = new TextureRegion();

					// set pixel location/dimensions
					tile.Dimensions.Left = tileBorder + x * tileWidth;
					tile.Dimensions.Top = tileBorder + y * tileHeight;
					tile.Dimensions.Right = tile.Dimensions.Left + tileWidth - tileBorder;
					tile.Dimensions.Bottom = tile.Dimensions.Top + tileHeight - tileBorder;

					// set texture coordinates
					// HACK: subtract TexCoordEdgeOffset from the bottom right edges to 
					//       get around floating point rounding errors (adjacent tiles will 
					//       slightly bleed in otherwise)
					tile.TexCoords.Left = (tile.Dimensions.Left - tileBorder + TexCoordEdgeOffset) / (float)Width;
					tile.TexCoords.Top = (tile.Dimensions.Top - tileBorder + TexCoordEdgeOffset) / (float)Height;
					tile.TexCoords.Right = ((float)tile.Dimensions.Right + tileBorder - TexCoordEdgeOffset) / (float)Width;
					tile.TexCoords.Bottom = ((float)tile.Dimensions.Bottom + tileBorder - TexCoordEdgeOffset) / (float)Height;

					// with the particular order that our for loops are nested in, this will
					// be the same as if we were using ((y * tilesX) + x) to manually set an
					// index each loop iteration (but using a List<> doesn't let us preallocate
					// in such a way that allows us to avoid using Add() ...)
					Tiles.Add(tile);
				}
			}
		}
	}
}
