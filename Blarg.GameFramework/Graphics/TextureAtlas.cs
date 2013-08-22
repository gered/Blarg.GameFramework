using System;
using System.Collections.Generic;

namespace Blarg.GameFramework.Graphics
{
	public abstract class TextureAtlas
	{
		public const float TexCoordEdgeBleedOffset = 0.02f;

		Texture _texture;

		public int Width { get; protected set; }
		public int Height { get; protected set; }

		protected float TexCoordEdgeOffset { get; private set; }
		protected List<TextureRegion> Tiles { get; private set; }

		public Texture Texture
		{
			get
			{
				return _texture;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException();

				if (value.Width != Width || value.Height != Height)
					throw new InvalidOperationException();

				_texture = value;
			}
		}

		public TextureAtlas(int textureWidth, int textureHeight, float texCoordEdgeOffset = TexCoordEdgeBleedOffset)
		{
			if (textureWidth < 0)
				throw new ArgumentOutOfRangeException("textureWidth");
			if (textureHeight < 0)
				throw new ArgumentOutOfRangeException("textureHeight");

			Width = textureWidth;
			Height = textureHeight;
			TexCoordEdgeOffset = texCoordEdgeOffset;

			Tiles = new List<TextureRegion>();
		}

		public TextureAtlas(Texture texture, float texCoordEdgeOffset = TexCoordEdgeBleedOffset)
		{
			if (texture == null)
				throw new ArgumentNullException("texture");

			_texture = texture;
			Width = _texture.Width;
			Height = _texture.Height;
			TexCoordEdgeOffset = texCoordEdgeOffset;

			Tiles = new List<TextureRegion>();
		}

		public int NumTiles
		{
			get { return Tiles.Count; }
		}

		public TextureRegion GetTile(int index)
		{
			return Tiles[index];
		}

		public void GetTileDimensions(int index, out Rect dimensions)
		{
			dimensions = Tiles[index].Dimensions;
		}

		public void GetTileTexCoords(int index, out RectF texCoords)
		{
			texCoords = Tiles[index].TexCoords;
		}
	}
}
