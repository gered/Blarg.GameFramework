using System;

namespace Blarg.GameFramework.Graphics.Atlas
{
	public class CustomTextureAtlas : TextureAtlas
	{
		public CustomTextureAtlas(int textureWidth, int textureHeight, float texCoordEdgeOffset = TexCoordEdgeBleedOffset)
			: base(textureWidth, textureHeight, texCoordEdgeOffset)
		{
		}

		public CustomTextureAtlas(Texture texture, float texCoordEdgeOffset = TexCoordEdgeBleedOffset)
			: base(texture, texCoordEdgeOffset)
		{
		}

		public int Add(ref Rect region)
		{
			if (region.Left >= region.Right)
				throw new InvalidOperationException();
			if (region.Top >= region.Bottom)
				throw new InvalidOperationException();
			if (region.Right >= Width)
				throw new InvalidOperationException();
			if (region.Bottom >= Height)
				throw new InvalidOperationException();

			TextureRegion tile;

			// pixel location/dimensions
			tile.Dimensions = region;

			// texture coordinates
			// HACK: subtract TexCoordEdgeOffset from the bottom right edges to 
			//       get around floating point rounding errors (adjacent tiles will 
			//       slightly bleed in otherwise)
			tile.TexCoords.Left = ((float)region.Left + TexCoordEdgeOffset) / (float)Width;
			tile.TexCoords.Top = ((float)region.Top + TexCoordEdgeOffset) / (float)Height;
			tile.TexCoords.Right = ((float)region.Right - TexCoordEdgeOffset) / (float)Width;
			tile.TexCoords.Bottom = ((float)region.Bottom - TexCoordEdgeOffset) / (float)Height;

			Tiles.Add(tile);

			return Tiles.Count - 1;
		}

		public int Add(Rect region)
		{
			return Add(ref region);
		}

		public int Add(int left, int top, int right, int bottom)
		{
			var region = new Rect(left, top, right, bottom);
			return Add(ref region);
		}

		public int AddGrid(int startX, int startY, int tileWidth, int tileHeight, int numTilesX, int numTilesY, int tileBorder = 0)
		{
			int actualTileWidth = tileWidth + tileBorder;
			int actualTileHeight = tileHeight + tileBorder;

			int numAdded = 0;

			for (int y = 0; y < numTilesY; ++y)
			{
				for (int x = 0; x < numTilesX; ++x)
				{
					var tile = new TextureRegion();

					// pixel location/dimensions
					tile.Dimensions.Left = startX + tileBorder + x * actualTileWidth;
					tile.Dimensions.Top = startY + tileBorder + y * actualTileHeight;
					tile.Dimensions.Right = tile.Dimensions.Left + actualTileWidth - tileBorder;
					tile.Dimensions.Bottom = tile.Dimensions.Top + actualTileHeight - tileBorder;

					// texture coordinates
					// HACK: subtract TexCoordEdgeOffset from the bottom right edges to
					//       get around floating point rounding errors (adjacent tiles will
					//       slightly bleed in otherwise)
					tile.TexCoords.Left = (tile.Dimensions.Left - tileBorder + TexCoordEdgeOffset) / (float)Width;
					tile.TexCoords.Top = (tile.Dimensions.Top - tileBorder + TexCoordEdgeOffset) / (float)Height;
					tile.TexCoords.Right = ((float)tile.Dimensions.Right + tileBorder - TexCoordEdgeOffset) / (float)Width;
					tile.TexCoords.Bottom = ((float)tile.Dimensions.Bottom + tileBorder - TexCoordEdgeOffset) / (float)Height;

					Tiles.Add(tile);
					++numAdded;
				}
			}

			return numAdded;
		}

		public void Reset()
		{
			Tiles.Clear();
		}
	}
}
