using System;
using System.Text;

namespace Blarg.GameFramework.Graphics
{
	public class SpriteBatch
	{
		static StringBuilder _buffer = new StringBuilder(8192);

		const int DefaultSpriteCount = 128;
		const int ResizeSpriteIncrement = 16;
		const int VerticesPerSprite = 4;
		const int IndicesPerSprite = 6;

		VertexBuffer _vertices;
		IndexBuffer _indices;
		Texture[] _textures;
		RenderState _defaultRenderState;
		RenderState _providedRenderState;
		BlendState _defaultBlendState;
		BlendState _providedBlendState;
		SpriteShader _shader;
		Matrix4x4 _previousProjection;
		Matrix4x4 _previousModelView;
		bool _isClipping;
		RectF _clipRegion;
		int _currentSpritePointer;
		bool _hasBegunRendering;
		Color _defaultSpriteColor = Color.White;

		public readonly GraphicsDevice GraphicsDevice;

		public SpriteBatch(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			GraphicsDevice = graphicsDevice;

			int numSprites = DefaultSpriteCount;

			_currentSpritePointer = 0;

			_vertices = new VertexBuffer(GraphicsDevice, VertexAttributeDeclarations.TextureColorPosition2D, (numSprites * VerticesPerSprite), BufferObjectUsage.Stream);
			_indices = new IndexBuffer(GraphicsDevice, (numSprites * IndicesPerSprite), BufferObjectUsage.Stream);
			_textures = new Texture[numSprites];

			FillSpriteIndicesFor(0, numSprites - 1);

			_defaultRenderState = RenderState.NoDepthTesting;
			_providedRenderState = null;

			_defaultBlendState = BlendState.AlphaBlend;
			_providedBlendState = null;

			_hasBegunRendering = false;
			_isClipping = false;
		}

		#region Begin/End

		public void Begin(SpriteShader shader = null)
		{
			InternalBegin(null, null, shader);
		}

		public void Begin(RenderState renderState, SpriteShader shader = null)
		{
			InternalBegin(renderState, null, shader);
		}

		public void Begin(BlendState blendState, SpriteShader shader = null)
		{
			InternalBegin(null, blendState, shader);
		}

		public void Begin(RenderState renderState, BlendState blendState, SpriteShader shader = null)
		{
			InternalBegin(renderState, blendState, shader);
		}

		public void End()
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			// don't do anything if nothing is to be rendered!
			if (_currentSpritePointer == 0)
			{
				EndClipping();
				_hasBegunRendering = false;
				return;
			}

			if (_providedRenderState != null)
				_providedRenderState.Apply(GraphicsDevice);
			else
				_defaultRenderState.Apply(GraphicsDevice);

			if (_providedBlendState != null)
				_providedBlendState.Apply(GraphicsDevice);
			else
				_defaultBlendState.Apply(GraphicsDevice);

			GraphicsDevice.BindShader(_shader);
			_shader.SetModelViewMatrix(Matrix4x4.Identity);
			_shader.SetProjectionMatrix(GraphicsDevice.ViewContext.OrthographicProjectionMatrix);
			RenderQueue();
			GraphicsDevice.UnbindShader();

			EndClipping();

			_hasBegunRendering = false;
		}

		private void InternalBegin(RenderState renderState, BlendState blendState, SpriteShader shader)
		{
			if (_hasBegunRendering)
				throw new InvalidOperationException();

			_previousProjection = GraphicsDevice.ViewContext.ProjectionMatrix;
			_previousModelView = GraphicsDevice.ViewContext.ModelViewMatrix;

			if (shader == null)
				_shader = GraphicsDevice.Sprite2DShader;
			else
			{
				if (!shader.IsReadyForUse)
					throw new InvalidOperationException("Shader not usable for rendering.");
				_shader = shader;
			}

			if (renderState != null)
				_providedRenderState = renderState;
			else
				_providedRenderState = null;

			if (blendState != null)
				_providedBlendState = blendState;
			else
				_providedBlendState = null;

			_currentSpritePointer = 0;
			_hasBegunRendering = true;
			_vertices.MoveToStart();
			_indices.MoveToStart();
		}

		private void RenderQueue()
		{
			GraphicsDevice.BindVertexBuffer(_vertices);
			GraphicsDevice.BindIndexBuffer(_indices);

			int firstSpriteIndex = 0;
			int lastSpriteIndex = 0;

			for (int i = 0; i < _currentSpritePointer; ++i)
			{
				if (_textures[lastSpriteIndex] != _textures[i])
				{
					// if the next texture is different then the last range's
					// texture, then we need to render the last range now
					RenderQueueRange(firstSpriteIndex, lastSpriteIndex);

					// switch to the new range with this new texture
					firstSpriteIndex = i;
				}

				lastSpriteIndex = i;
			}

			// we'll have one last range to render at this point (the loop would have
			// ended before it was caught by the checks inside the loop)
			RenderQueueRange(firstSpriteIndex, lastSpriteIndex);

			// clear out the texture's array so it's not holding references
			// to stuff that might need to be collected by the GC
			// (e.g. if we render a lot of sprites one frame, and then the next
			// bunch of frames we don't render as much, the end of the array
			// will still hold old references to Texture objects used previously)
			for (int i = 0; i < _textures.Length; ++i)
				_textures[i] = null;

			GraphicsDevice.UnbindIndexBuffer();
			GraphicsDevice.UnbindVertexBuffer();
		}

		private void RenderQueueRange(int firstSpriteIndex, int lastSpriteIndex)
		{
			int startVertexIndex = firstSpriteIndex * IndicesPerSprite;
			int lastVertexIndex = (lastSpriteIndex + 1) * IndicesPerSprite;   // render up to and including the last sprite

			// take the texture from anywhere in this range (doesn't matter where, it should all be the same texture)
			Texture spriteTexture = _textures[firstSpriteIndex];
			bool hasAlphaOnly = spriteTexture.Format == TextureFormat.Alpha ? true : false;

			GraphicsDevice.BindTexture(spriteTexture);
			_shader.SetTextureHasAlphaOnly(hasAlphaOnly);

			GraphicsDevice.RenderTriangles(startVertexIndex, (lastVertexIndex - startVertexIndex) / 3);
		}

		#endregion

		#region Render: Sprites

		public void Render(Texture texture, int x, int y)
		{
			Render(texture, x, y, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, int x, int y, Color color)
		{
			Render(texture, x, y, ref color);
		}

		public void Render(Texture texture, int x, int y, ref Color color)
		{
			y = FixYCoord(y, texture.Height);
			AddSprite(texture, x, y, x + texture.Width, y + texture.Height, 0, 0, texture.Width, texture.Height, ref color);
		}

		public void Render(Texture texture, int x, int y, int width, int height)
		{
			Render(texture, x, y, width, height, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, int x, int y, int width, int height, Color color)
		{
			Render(texture, x, y, width, height, ref color);
		}

		public void Render(Texture texture, int x, int y, int width, int height, ref Color color)
		{
			y = FixYCoord(y, height);
			AddSprite(texture, x, y, x + width, y + height, 0, 0, width, height, ref color);
		}

		public void Render(Texture texture, int x, int y, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom)
		{
			Render(texture, x, y, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, int x, int y, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, Color color)
		{
			Render(texture, x, y, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		public void Render(Texture texture, int x, int y, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			y = FixYCoord(y, texture.Height);
			AddSprite(texture, x, y, x + texture.Width, y + texture.Height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		public void Render(Texture texture, int x, int y, int width, int height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom)
		{
			Render(texture, x, y, width, height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, int x, int y, int width, int height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, Color color)
		{
			Render(texture, x, y, width, height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		public void Render(Texture texture, int x, int y, int width, int height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			y = FixYCoord(y, height);
			AddSprite(texture, x, y, x + width, y + height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		public void Render(Texture texture, Vector3 worldPosition)
		{
			Render(texture, ref worldPosition, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, ref Vector3 worldPosition)
		{
			Render(texture, ref worldPosition, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, Vector3 worldPosition, Color color)
		{
			Render(texture, ref worldPosition, ref color);
		}

		public void Render(Texture texture, ref Vector3 worldPosition, ref Color color)
		{
			var screenCoordinates = GraphicsDevice.ViewContext.Camera.Project(ref worldPosition, ref _previousModelView, ref _previousProjection);

			screenCoordinates.X -= texture.Width / 2;
			screenCoordinates.Y -= texture.Height / 2;

			Render(texture, screenCoordinates.X, screenCoordinates.Y, ref color);
		}

		public void Render(Texture texture, Vector3 worldPosition, int width, int height)
		{
			Render(texture, ref worldPosition, width, height, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, ref Vector3 worldPosition, int width, int height)
		{
			Render(texture, ref worldPosition, width, height, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, Vector3 worldPosition, int width, int height, Color color)
		{
			Render(texture, ref worldPosition, width, height, ref color);
		}

		public void Render(Texture texture, ref Vector3 worldPosition, int width, int height, ref Color color)
		{
			var screenCoordinates = GraphicsDevice.ViewContext.Camera.Project(ref worldPosition, ref _previousModelView, ref _previousProjection);

			screenCoordinates.X -= width / 2;
			screenCoordinates.Y -= height / 2;

			Render(texture, screenCoordinates.X, screenCoordinates.Y, width, height, ref color);
		}

		public void Render(Texture texture, Vector3 worldPosition, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom)
		{
			Render(texture, ref worldPosition, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, ref Vector3 worldPosition, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom)
		{
			Render(texture, ref worldPosition, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, Vector3 worldPosition, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, Color color)
		{
			Render(texture, ref worldPosition, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		public void Render(Texture texture, ref Vector3 worldPosition, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			var screenCoordinates = GraphicsDevice.ViewContext.Camera.Project(ref worldPosition, ref _previousModelView, ref _previousProjection);

			screenCoordinates.X -= texture.Width / 2;
			screenCoordinates.Y -= texture.Height / 2;

			Render(texture, screenCoordinates.X, screenCoordinates.Y, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		public void Render(Texture texture, Vector3 worldPosition, int width, int height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom)
		{
			Render(texture, ref worldPosition, width, height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, ref Vector3 worldPosition, int width, int height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom)
		{
			Render(texture, ref worldPosition, width, height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, Vector3 worldPosition, int width, int height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, Color color)
		{
			Render(texture, ref worldPosition, width, height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		public void Render(Texture texture, ref Vector3 worldPosition, int width, int height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			var screenCoordinates = GraphicsDevice.ViewContext.Camera.Project(ref worldPosition, ref _previousModelView, ref _previousProjection);

			screenCoordinates.X -= width / 2;
			screenCoordinates.Y -= height / 2;

			Render(texture, screenCoordinates.X, screenCoordinates.Y, width, height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		public void Render(TextureAtlas atlas, int index, int x, int y)
		{
			Render(atlas, index, x, y, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, int x, int y, Color color)
		{
			Render(atlas, index, x, y, ref color);
		}

		public void Render(TextureAtlas atlas, int index, int x, int y, ref Color color)
		{
			RectF texCoords;
			Rect dimensions;
			atlas.GetTileTexCoords(index, out texCoords);
			atlas.GetTileDimensions(index, out dimensions);

			y = FixYCoord(y, dimensions.Height);
			AddSprite(atlas.Texture, x, y, x + dimensions.Width, y + dimensions.Height, texCoords.Left, texCoords.Top, texCoords.Right, texCoords.Bottom, ref color);
		}

		public void Render(TextureAtlas atlas, int index, int x, int y, int width, int height)
		{
			Render(atlas, index, x, y, width, height, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, int x, int y, int width, int height, Color color)
		{
			Render(atlas, index, x, y, width, height, ref color);
		}

		public void Render(TextureAtlas atlas, int index, int x, int y, int width, int height, ref Color color)
		{
			RectF texCoords;
			atlas.GetTileTexCoords(index, out texCoords);

			y = FixYCoord(y, height);
			AddSprite(atlas.Texture, x, y, x + width, y + height, texCoords.Left, texCoords.Top, texCoords.Right, texCoords.Bottom, ref color);
		}

		public void Render(TextureAtlas atlas, int index, Vector3 worldPosition)
		{
			Render(atlas, index, ref worldPosition, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, ref Vector3 worldPosition)
		{
			Render(atlas, index, ref worldPosition, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, Vector3 worldPosition, Color color)
		{
			Render(atlas, index, ref worldPosition, ref color);
		}

		public void Render(TextureAtlas atlas, int index, ref Vector3 worldPosition, ref Color color)
		{
			var screenCoordinates = GraphicsDevice.ViewContext.Camera.Project(ref worldPosition, ref _previousModelView, ref _previousProjection);

			var tile = atlas.GetTile(index);
			screenCoordinates.X -= tile.Dimensions.Width / 2;
			screenCoordinates.Y -= tile.Dimensions.Height / 2;

			Render(atlas, index, screenCoordinates.X, screenCoordinates.Y, ref color);
		}

		public void Render(TextureAtlas atlas, int index, Vector3 worldPosition, int width, int height)
		{
			Render(atlas, index, ref worldPosition, width, height, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, ref Vector3 worldPosition, int width, int height)
		{
			Render(atlas, index, ref worldPosition, width, height, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, Vector3 worldPosition, int width, int height, Color color)
		{
			Render(atlas, index, ref worldPosition, width, height, ref color);
		}

		public void Render(TextureAtlas atlas, int index, ref Vector3 worldPosition, int width, int height, ref Color color)
		{
			var screenCoordinates = GraphicsDevice.ViewContext.Camera.Project(ref worldPosition, ref _previousModelView, ref _previousProjection);

			screenCoordinates.X -= width / 2;
			screenCoordinates.Y -= height / 2;

			Render(atlas, index, screenCoordinates.X, screenCoordinates.Y, width, height, ref color);
		}

		#endregion

		#region Render: Fonts

		public void Render(SpriteFont font, int x, int y, Color color, string text)
		{
			Render(font, x, y, ref color, 1.0f, text);
		}

		public void Render(SpriteFont font, int x, int y, ref Color color, string text)
		{
			Render(font, x, y, ref color, 1.0f, text);
		}

		public void Render(SpriteFont font, int x, int y, Color color, float scale, string text)
		{
			Render(font, x, y, ref color, scale, text);
		}

		public void Render(SpriteFont font, int x, int y, ref Color color, float scale, string text)
		{
			float scaledLetterHeight = (float)font.LetterHeight * scale;

			y = (int)FixYCoord(y, scaledLetterHeight);

			float drawX = (float)x;
			float drawY = (float)y;

			for (int i = 0; i < text.Length; ++i)
			{
				char c = text[i];
				if (c == '\n')
				{
					// new line
					drawX = (float)x;
					drawY -= scaledLetterHeight;
				}
				else
				{
					RectF texCoords;
					Rect dimensions;
					font.GetCharTexCoords(c, out texCoords);
					font.GetCharDimensions(c, out dimensions);

					float scaledGlyphWidth = (float)dimensions.Width * scale;
					float scaledGlyphHeight = (float)dimensions.Height * scale;

					AddSprite(
						font.Texture, 
						drawX, drawY, drawX + scaledGlyphWidth, drawY + scaledGlyphHeight, 
						texCoords.Left, texCoords.Top, texCoords.Right, texCoords.Bottom, 
						ref color
						);

					drawX += scaledGlyphWidth;
				}
			}
		}

		public void Render(SpriteFont font, int x, int y, Color color, StringBuilder text)
		{
			Render(font, x, y, ref color, 1.0f, text);
		}

		public void Render(SpriteFont font, int x, int y, ref Color color, StringBuilder text)
		{
			Render(font, x, y, ref color, 1.0f, text);
		}

		public void Render(SpriteFont font, int x, int y, Color color, float scale, StringBuilder text)
		{
			Render(font, x, y, ref color, scale, text);
		}

		public void Render(SpriteFont font, int x, int y, ref Color color, float scale, StringBuilder text)
		{
			float scaledLetterHeight = (float)font.LetterHeight * scale;

			y = (int)FixYCoord(y, scaledLetterHeight);

			float drawX = (float)x;
			float drawY = (float)y;

			for (int i = 0; i < text.Length; ++i)
			{
				char c = text[i];
				if (c == '\n')
				{
					// new line
					drawX = (float)x;
					drawY -= scaledLetterHeight;
				}
				else
				{
					RectF texCoords;
					Rect dimensions;
					font.GetCharTexCoords(c, out texCoords);
					font.GetCharDimensions(c, out dimensions);

					float scaledGlyphWidth = (float)dimensions.Width * scale;
					float scaledGlyphHeight = (float)dimensions.Height * scale;

					AddSprite(
						font.Texture, 
						drawX, drawY, drawX + scaledGlyphWidth, drawY + scaledGlyphHeight, 
						texCoords.Left, texCoords.Top, texCoords.Right, texCoords.Bottom, 
						ref color
						);

					drawX += scaledGlyphWidth;
				}
			}
		}

		public void Render(SpriteFont font, Vector3 worldPosition, Color color, string text)
		{
			Render(font, ref worldPosition, ref color, 1.0f, text);
		}

		public void Render(SpriteFont font, ref Vector3 worldPosition, ref Color color, string text)
		{
			Render(font, ref worldPosition, ref color, 1.0f, text);
		}

		public void Render(SpriteFont font, Vector3 worldPosition, Color color, float scale, string text)
		{
			Render(font, ref worldPosition, ref color, scale, text);
		}

		public void Render(SpriteFont font, ref Vector3 worldPosition, ref Color color, float scale, string text)
		{
			var screenCoordinates = GraphicsDevice.ViewContext.Camera.Project(ref worldPosition, ref _previousModelView, ref _previousProjection);

			int textWidth;
			int textHeight;
			font.MeasureString(out textWidth, out textHeight, scale, text);

			screenCoordinates.X -= textWidth / 2;
			screenCoordinates.Y -= textHeight / 2;

			Render(font, screenCoordinates.X, screenCoordinates.Y, ref color, scale, text);
		}

		public void Printf(SpriteFont font, int x, int y, Color color, string format, params object[] args)
		{
			Printf(font, x, y, ref color, 1.0f, format, args);
		}

		public void Printf(SpriteFont font, int x, int y, ref Color color, string format, params object[] args)
		{
			Printf(font, x, y, ref color, 1.0f, format, args);
		}

		public void Printf(SpriteFont font, int x, int y, Color color, float scale, string format, params object[] args)
		{
			Printf(font, x, y, ref color, scale, format, args);
		}

		public void Printf(SpriteFont font, int x, int y, ref Color color, float scale, string format, params object[] args)
		{
			_buffer.Clear();
			_buffer.AppendFormat(format, args);

			Render(font, x, y, ref color, scale, _buffer.ToString());
		}

		public void Printf(SpriteFont font, Vector3 worldPosition, Color color, string format, params object[] args)
		{
			Printf(font, ref worldPosition, ref color, 1.0f, format, args);
		}

		public void Printf(SpriteFont font, ref Vector3 worldPosition, ref Color color, string format, params object[] args)
		{
			Printf(font, ref worldPosition, ref color, 1.0f, format, args);
		}

		public void Printf(SpriteFont font, Vector3 worldPosition, Color color, float scale, string format, params object[] args)
		{
			Printf(font, ref worldPosition, ref color, scale, format, args);
		}

		public void Printf(SpriteFont font, ref Vector3 worldPosition, ref Color color, float scale, string format, params object[] args)
		{
			_buffer.Clear();
			_buffer.AppendFormat(format, args);

			Render(font, ref worldPosition, ref color, scale, _buffer.ToString());
		}

		public void Render(SpriteFont font, Vector3 worldPosition, Color color, StringBuilder text)
		{
			Render(font, ref worldPosition, ref color, 1.0f, text);
		}

		public void Render(SpriteFont font, ref Vector3 worldPosition, ref Color color, StringBuilder text)
		{
			Render(font, ref worldPosition, ref color, 1.0f, text);
		}

		public void Render(SpriteFont font, Vector3 worldPosition, Color color, float scale, StringBuilder text)
		{
			Render(font, ref worldPosition, ref color, scale, text);
		}

		public void Render(SpriteFont font, ref Vector3 worldPosition, ref Color color, float scale, StringBuilder text)
		{
			var screenCoordinates = GraphicsDevice.ViewContext.Camera.Project(ref worldPosition, ref _previousModelView, ref _previousProjection);

			int textWidth;
			int textHeight;
			font.MeasureString(out textWidth, out textHeight, scale, text);

			screenCoordinates.X -= textWidth / 2;
			screenCoordinates.Y -= textHeight / 2;

			Render(font, screenCoordinates.X, screenCoordinates.Y, ref color, scale, text);
		}

		#endregion

		#region Internal Sprite Addition / Management

		private void AddSprite(Texture texture, int destLeft, int destTop, int destRight, int destBottom, int sourceLeft, int sourceTop, int sourceRight, int sourceBottom, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			int width = sourceRight - sourceLeft;
			if (width < 1)
				throw new InvalidOperationException("Zero-length width");

			int height = sourceBottom - sourceTop;
			if (height < 1)
				throw new InvalidOperationException("Zero-length height.");

			float texLeft = sourceLeft / (float)width;
			float texTop = sourceTop / (float)height;
			float texRight = sourceRight / (float)width;
			float texBottom = sourceBottom / (float)height;
			float destLeftF = (float)destLeft;
			float destTopF = (float)destTop;
			float destRightF = (float)destRight;
			float destBottomF = (float)destBottom;

			if (_isClipping)
			{
				if (!ClipSpriteCoords(ref destLeftF, ref destTopF, ref destRightF, ref destBottomF, ref texLeft, ref texTop, ref texRight, ref texBottom))
					return;
			}

			if (GetRemainingSpriteSpaces() < 1)
				AddMoreSpriteSpace(ResizeSpriteIncrement);

			SetSpriteInfo(_currentSpritePointer, texture, destLeftF, destTopF, destRightF, destBottomF, texLeft, texTop, texRight, texBottom, ref color);
			++_currentSpritePointer;
		}

		private void AddSprite(Texture texture, int destLeft, int destTop, int destRight, int destBottom, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			float destLeftF = (float)destLeft;
			float destTopF = (float)destTop;
			float destRightF = (float)destRight;
			float destBottomF = (float)destBottom;

			if (_isClipping)
			{
				if (!ClipSpriteCoords(ref destLeftF, ref destTopF, ref destRightF, ref destBottomF, ref texCoordLeft, ref texCoordTop, ref texCoordRight, ref texCoordBottom))
					return;
			}

			if (GetRemainingSpriteSpaces() < 1)
				AddMoreSpriteSpace(ResizeSpriteIncrement);

			SetSpriteInfo(_currentSpritePointer, texture, destLeftF, destTopF, destRightF, destBottomF, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
			++_currentSpritePointer;
		}

		private void AddSprite(Texture texture, float destLeft, float destTop, float destRight, float destBottom, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			if (_isClipping)
			{
				if (!ClipSpriteCoords(ref destLeft, ref destTop, ref destRight, ref destBottom, ref texCoordLeft, ref texCoordTop, ref texCoordRight, ref texCoordBottom))
					return;
			}

			if (GetRemainingSpriteSpaces() < 1)
				AddMoreSpriteSpace(ResizeSpriteIncrement);

			SetSpriteInfo(_currentSpritePointer, texture, destLeft, destTop, destRight, destBottom, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
			++_currentSpritePointer;
		}

		private void SetSpriteInfo(int spriteIndex, Texture texture, float destLeft, float destTop, float destRight, float destBottom, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			int baseVertexIndex = spriteIndex * VerticesPerSprite;

			_vertices.SetPosition2D(baseVertexIndex + 0, destLeft,  destTop);
			_vertices.SetPosition2D(baseVertexIndex + 1, destRight, destTop);
			_vertices.SetPosition2D(baseVertexIndex + 2, destRight, destBottom);
			_vertices.SetPosition2D(baseVertexIndex + 3, destLeft,  destBottom);

			_vertices.SetTexCoord(baseVertexIndex + 0, texCoordLeft,  texCoordBottom);
			_vertices.SetTexCoord(baseVertexIndex + 1, texCoordRight, texCoordBottom);
			_vertices.SetTexCoord(baseVertexIndex + 2, texCoordRight, texCoordTop);
			_vertices.SetTexCoord(baseVertexIndex + 3, texCoordLeft,  texCoordTop);

			_vertices.SetColor(baseVertexIndex + 0, ref color);
			_vertices.SetColor(baseVertexIndex + 1, ref color);
			_vertices.SetColor(baseVertexIndex + 2, ref color);
			_vertices.SetColor(baseVertexIndex + 3, ref color);

			_textures[spriteIndex] = texture;
		}

		private int GetRemainingSpriteSpaces()
		{
			int currentMaxSprites = _vertices.NumElements / VerticesPerSprite;
			return currentMaxSprites - _currentSpritePointer;
		}

		private void AddMoreSpriteSpace(int numSprites)
		{
			int numVerticesToAdd = numSprites * VerticesPerSprite;
			int numIndicesToAdd = numSprites * IndicesPerSprite;
			int newTextureArraySize = _textures.Length + numSprites;

			int oldSpriteCount = _vertices.NumElements / VerticesPerSprite;

			_vertices.Extend(numVerticesToAdd);
			_indices.Extend(numIndicesToAdd);
			Array.Resize(ref _textures, newTextureArraySize);

			int newSpriteCount = _vertices.NumElements / VerticesPerSprite;

			FillSpriteIndicesFor(oldSpriteCount - 1, newSpriteCount - 1);
		}

		private void FillSpriteIndicesFor(int firstSprite, int lastSprite)
		{
			for (int i = firstSprite; i <= lastSprite; ++i)
			{
				int indicesStart = i * IndicesPerSprite;
				int verticesStart = i * VerticesPerSprite;

				_indices.Set(indicesStart + 0, (ushort)(verticesStart + 0));
				_indices.Set(indicesStart + 1, (ushort)(verticesStart + 1));
				_indices.Set(indicesStart + 2, (ushort)(verticesStart + 2));
				_indices.Set(indicesStart + 3, (ushort)(verticesStart + 0));
				_indices.Set(indicesStart + 4, (ushort)(verticesStart + 2));
				_indices.Set(indicesStart + 5, (ushort)(verticesStart + 3));
			}
		}

		private int FixYCoord(int y, int sourceHeight)
		{
			return GraphicsDevice.ViewContext.PixelScaler.ScaledHeight - y - sourceHeight;
		}

		private float FixYCoord(int y, float sourceHeight)
		{
			return (float)GraphicsDevice.ViewContext.PixelScaler.ScaledHeight - (float)y - sourceHeight;
		}

		#endregion

		#region Clipping

		public void BeginClipping(ref Rect region)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();
			if (_isClipping)
				throw new InvalidOperationException();

			_isClipping = true;

			int fixedTop = ((int)GraphicsDevice.ViewContext.PixelScaler.ScaledHeight - region.Top - region.Height);
			int fixedBottom = fixedTop + region.Height;

			_clipRegion.Left = (float)region.Left;
			_clipRegion.Top = (float)fixedTop;
			_clipRegion.Right = (float)region.Right;
			_clipRegion.Bottom = (float)fixedBottom;
		}

		public void BeginClipping(int left, int top, int right, int bottom)
		{
			Rect r = new Rect();
			r.Left = left;
			r.Top = top;
			r.Right = right;
			r.Bottom = bottom;
			BeginClipping(ref r);
		}

		public void EndClipping()
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			_isClipping = false;
		}

		private bool ClipSpriteCoords(ref float left, ref float top, ref float right, ref float bottom, ref float texCoordLeft, ref float texCoordTop, ref float texCoordRight, ref float texCoordBottom)
		{
			// check for completely out of bounds scenarios first
			if (left >= _clipRegion.Right)
				return false;
			if (right < _clipRegion.Left)
				return false;
			if (top >= _clipRegion.Bottom)
				return false;
			if (bottom < _clipRegion.Top)
				return false;

			float clippedLeft = left;
			float clippedTop = top;
			float clippedRight = right;
			float clippedBottom = bottom;
			float clippedTexCoordLeft = texCoordLeft;
			float clippedTexCoordTop = texCoordTop;
			float clippedTexCoordRight = texCoordRight;
			float clippedTexCoordBottom = texCoordBottom;

			if (clippedLeft < _clipRegion.Left)
			{
				clippedLeft = _clipRegion.Left;
				float t = MathHelpers.InverseLerp(left, right, clippedLeft);
				clippedTexCoordLeft = MathHelpers.Lerp(texCoordLeft, texCoordRight, t);
			}
			if (clippedRight > _clipRegion.Right)
			{
				clippedRight = _clipRegion.Right;
				float t = MathHelpers.InverseLerp(right, left, clippedRight);
				clippedTexCoordRight = MathHelpers.Lerp(texCoordRight, texCoordLeft, t);
			}
			if (clippedTop < _clipRegion.Top)
			{
				clippedTop = _clipRegion.Top;
				float t = MathHelpers.InverseLerp(top, bottom, clippedTop);
				clippedTexCoordBottom = MathHelpers.Lerp(texCoordBottom, texCoordTop, t);
			}
			if (clippedBottom > _clipRegion.Bottom)
			{
				clippedBottom = _clipRegion.Bottom;
				float t = MathHelpers.InverseLerp(bottom, top, clippedBottom);
				clippedTexCoordTop = MathHelpers.Lerp(texCoordTop, texCoordBottom, t);
			}

			left = clippedLeft;
			top = clippedTop;
			right = clippedRight;
			bottom = clippedBottom;
			texCoordLeft = clippedTexCoordLeft;
			texCoordTop = clippedTexCoordTop;
			texCoordRight = clippedTexCoordRight;
			texCoordBottom = clippedTexCoordBottom;

			return true;
		}

		#endregion
	}
}
