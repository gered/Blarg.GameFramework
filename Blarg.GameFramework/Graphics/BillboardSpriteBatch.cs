using System;
using System.Text;

namespace Blarg.GameFramework.Graphics
{
	public enum BillboardSpriteType
	{
		Spherical,
		Cylindrical,
		ScreenAligned,
		ScreenAndAxisAligned
	}

	public class BillboardSpriteBatch
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
		Vector3 _cameraPosition;
		Vector3 _cameraForward;
		SpriteShader _shader;
		int _currentSpritePointer;
		bool _hasBegunRendering;
		Color _defaultSpriteColor = Color.White;

		// since it's not valid C# to use 'ref' with a static readonly field...
		Vector3 _zeroVector = Vector3.Zero;
		Vector3 _yAxis = Vector3.YAxis;
		Vector3 _up = Vector3.Up;

		public readonly GraphicsDevice GraphicsDevice;

		public BillboardSpriteBatch(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			GraphicsDevice = graphicsDevice;

			int numSprites = DefaultSpriteCount;

			_currentSpritePointer = 0;

			_vertices = new VertexBuffer(GraphicsDevice, VertexAttributeDeclarations.TextureColorPosition3D, (numSprites * VerticesPerSprite), BufferObjectUsage.Stream);
			_indices = new IndexBuffer(GraphicsDevice, (numSprites * IndicesPerSprite), BufferObjectUsage.Stream);
			_textures = new Texture[numSprites];

			FillSpriteIndicesFor(0, numSprites - 1);

			_defaultRenderState = RenderState.Default;
			_providedRenderState = null;

			_defaultBlendState = BlendState.AlphaBlend;
			_providedBlendState = null;

			_hasBegunRendering = false;
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
			_shader.SetModelViewMatrix(GraphicsDevice.ViewContext.ModelViewMatrix);
			_shader.SetProjectionMatrix(GraphicsDevice.ViewContext.ProjectionMatrix);
			RenderQueue();
			GraphicsDevice.UnbindShader();

			_hasBegunRendering = false;
		}

		private void InternalBegin(RenderState renderState, BlendState blendState, SpriteShader shader)
		{
			if (_hasBegunRendering)
				throw new InvalidOperationException();

			_cameraPosition = GraphicsDevice.ViewContext.Camera.Position;
			_cameraForward = GraphicsDevice.ViewContext.Camera.Forward;

			if (shader == null)
				_shader = GraphicsDevice.Sprite3DShader;
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

		public void Render(Texture texture, float x, float y, float z, float width, float height, BillboardSpriteType type)
		{
			Render(texture, x, y, z, width, height, type, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, float x, float y, float z, float width, float height, BillboardSpriteType type, Color color)
		{
			Render(texture, x, y, z, width, height, type, ref color);
		}

		public void Render(Texture texture, float x, float y, float z, float width, float height, BillboardSpriteType type, ref Color color)
		{
			var position = new Vector3(x, y, z);
			AddSprite(type, texture, ref position, width, height, 0, 0, texture.Width, texture.Height, ref color);
		}

		public void Render(Texture texture, Vector3 position, float width, float height, BillboardSpriteType type)
		{
			Render(texture, ref position, width, height, type, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, ref Vector3 position, float width, float height, BillboardSpriteType type)
		{
			Render(texture, ref position, width, height, type, ref _defaultSpriteColor);
		}

		public void Render(Texture texture, Vector3 position, float width, float height, BillboardSpriteType type, Color color)
		{
			Render(texture, ref position, width, height, type, ref color);
		}

		public void Render(Texture texture, ref Vector3 position, float width, float height, BillboardSpriteType type, ref Color color)
		{
			AddSprite(type, texture, ref position, width, height, 0, 0, texture.Width, texture.Height, ref color);
		}

		public void Render(TextureAtlas atlas, int index, float x, float y, float z, float width, float height, BillboardSpriteType type)
		{
			Render(atlas, index, x, y, z, width, height, type, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, float x, float y, float z, float width, float height, BillboardSpriteType type, Color color)
		{
			Render(atlas, index, x, y, z, width, height, type, ref color);
		}

		public void Render(TextureAtlas atlas, int index, float x, float y, float z, float width, float height, BillboardSpriteType type, ref Color color)
		{
			RectF texCoords;
			atlas.GetTileTexCoords(index, out texCoords);

			var position = new Vector3(x, y, z);
			AddSprite(type, atlas.Texture, ref position, width, height, texCoords.Left, texCoords.Top, texCoords.Right, texCoords.Bottom, ref color);
		}

		public void Render(TextureAtlas atlas, int index, Vector3 position, float width, float height, BillboardSpriteType type)
		{
			Render(atlas, index, ref position, width, height, type, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, ref Vector3 position, float width, float height, BillboardSpriteType type)
		{
			Render(atlas, index, ref position, width, height, type, ref _defaultSpriteColor);
		}

		public void Render(TextureAtlas atlas, int index, Vector3 position, float width, float height, BillboardSpriteType type, Color color)
		{
			Render(atlas, index, ref position, width, height, type, ref color);
		}

		public void Render(TextureAtlas atlas, int index, ref Vector3 position, float width, float height, BillboardSpriteType type, ref Color color)
		{
			RectF texCoords;
			atlas.GetTileTexCoords(index, out texCoords);

			AddSprite(type, atlas.Texture, ref position, width, height, texCoords.Left, texCoords.Top, texCoords.Right, texCoords.Bottom, ref color);
		}

		#endregion

		#region Render: Fonts

		public void Render(SpriteFont font, float x, float y, float z, BillboardSpriteType type, Color color, float pixelScale, string text)
		{
			var position = new Vector3(x, y, z);
			Render(font, ref position, type, ref color, pixelScale, text);
		}

		public void Render(SpriteFont font, float x, float y, float z, BillboardSpriteType type, ref Color color, float pixelScale, string text)
		{
			var position = new Vector3(x, y, z);
			Render(font, ref position, type, ref color, pixelScale, text);
		}

		public void Render(SpriteFont font, Vector3 position, BillboardSpriteType type, Color color, float pixelScale, string text)
		{
			Render(font, ref position, type, ref color, pixelScale, text);
		}

		public void Render(SpriteFont font, ref Vector3 position, BillboardSpriteType type, ref Color color, float pixelScale, string text)
		{
			int textWidth;
			int textHeight;
			font.MeasureString(out textWidth, out textHeight, text);

			// the x,y,z coordinate specified is used as the position to center the
			// text billboard around. we start drawing the text at the top-left of this
			float startX = -(float)((textWidth / 2) * pixelScale);
			float startY = -(float)((textHeight / 2) * pixelScale);

			float drawX = startX;
			float drawY = startY;
			float lineHeight = (float)(font.LetterHeight * pixelScale);

			Matrix4x4 transform;
			GetTransformFor(type, ref position, out transform);

			RectF texCoords;
			Rect dimensions;
			var drawCoordinates = new Vector3();

			for (int i = 0; i < text.Length; ++i)
			{
				char c = text[i];
				if (c == '\n')
				{
					// new line
					drawX = startX;
					drawY += lineHeight;
				}
				else
				{
					font.GetCharTexCoords(c, out texCoords);
					font.GetCharDimensions(c, out dimensions);

					float glyphWidth = (float)(dimensions.Width * pixelScale);
					float glyphHeight = (float)(dimensions.Height * pixelScale);

					drawCoordinates.X = -drawX;
					drawCoordinates.Y = -drawY;

					AddSprite(
						type,
						ref transform,
						font.Texture, 
						ref drawCoordinates,
						glyphWidth, glyphHeight,
						texCoords.Left, texCoords.Top, texCoords.Right, texCoords.Bottom, 
						ref color
						);

					drawX += glyphWidth;
				}
			}
		}

		public void Printf(SpriteFont font, float x, float y, float z, BillboardSpriteType type, Color color, float pixelScale, string format, params object[] args)
		{
			var position = new Vector3(x, y, z);
			Printf(font, ref position, type, ref color, pixelScale, format, args);
		}

		public void Printf(SpriteFont font, float x, float y, float z, BillboardSpriteType type, ref Color color, float pixelScale, string format, params object[] args)
		{
			var position = new Vector3(x, y, z);
			Printf(font, ref position, type, ref color, pixelScale, format, args);
		}

		public void Printf(SpriteFont font, Vector3 position, BillboardSpriteType type, Color color, float pixelScale, string format, params object[] args)
		{
			Printf(font, ref position, type, ref color, pixelScale, format, args);
		}

		public void Printf(SpriteFont font, ref Vector3 position, BillboardSpriteType type, ref Color color, float pixelScale, string format, params object[] args)
		{
			_buffer.Clear();
			_buffer.AppendFormat(format, args);

			Render(font, ref position, type, ref color, pixelScale, _buffer.ToString());
		}

		#endregion

		#region Internal Sprite Addition / Management

		private void AddSprite(BillboardSpriteType type, Texture texture, ref Vector3 position, float width, float height, int sourceLeft, int sourceTop, int sourceRight, int sourceBottom, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			Matrix4x4 transform;
			GetTransformFor(type, ref position, out transform);

			// zero vector used as offset since the transform will translate the billboard
			// to the specified position
			AddSprite(type, ref transform, texture, ref _zeroVector, width, height, sourceLeft, sourceTop, sourceRight, sourceBottom, ref color);
		}

		private void AddSprite(BillboardSpriteType type, Texture texture, ref Vector3 position, float width, float height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			Matrix4x4 transform;
			GetTransformFor(type, ref position, out transform);

			// zero vector used as offset since the transform will translate the billboard
			// to the specified position
			AddSprite(type, ref transform, texture, ref _zeroVector, width, height, texCoordLeft, texCoordTop, texCoordRight, texCoordBottom, ref color);
		}

		private void AddSprite(BillboardSpriteType type, ref Matrix4x4 transform, Texture texture, ref Vector3 offset, float width, float height, int sourceLeft, int sourceTop, int sourceRight, int sourceBottom, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			int sourceWidth = sourceRight - sourceLeft;
			if (sourceWidth < 1)
				throw new InvalidOperationException("Zero-length width");

			int sourceHeight = sourceBottom - sourceTop;
			if (sourceHeight < 1)
				throw new InvalidOperationException("Zero-length height.");

			float texLeft = sourceLeft / (float)sourceWidth;
			float texTop = sourceTop / (float)sourceHeight;
			float texRight = sourceRight / (float)sourceWidth;
			float texBottom = sourceBottom / (float)sourceHeight;

			if (GetRemainingSpriteSpaces() < 1)
				AddMoreSpriteSpace(ResizeSpriteIncrement);

			SetSpriteInfo(_currentSpritePointer, type, ref transform, texture, ref offset, width, height, texLeft, texTop, texRight, texBottom, ref color);
			++_currentSpritePointer;
		}

		private void AddSprite(BillboardSpriteType type, ref Matrix4x4 transform, Texture texture, ref Vector3 offset, float width, float height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();
		}

		private void SetSpriteInfo(int spriteIndex, BillboardSpriteType type, ref Matrix4x4 transform, Texture texture, ref Vector3 offset, float width, float height, float texCoordLeft, float texCoordTop, float texCoordRight, float texCoordBottom, ref Color color)
		{
			int baseVertexIndex = spriteIndex * VerticesPerSprite;

			float halfWidth = width / 2.0f;
			float halfHeight = height / 2.0f;

			// TODO: come back to this and re-figure out why I needed to reverse this like so...
			float left = halfWidth;
			float top = -halfHeight;
			float right = -halfWidth;
			float bottom = halfHeight;

			// TODO: I'm unsure if all of this is better, or if putting the transformation matrix
			//       in the VBO as an extra vertex attribute to do the transform in the
			//       shader would be better
			//       transforming 4 vertices on the CPU vs copying 4 matrices into a VBO...

			Vector3 v1 = new Vector3(left +  offset.X, top +    offset.Y, 0.0f + offset.Z);
			Vector3 v2 = new Vector3(right + offset.X, top +    offset.Y, 0.0f + offset.Z);
			Vector3 v3 = new Vector3(right + offset.X, bottom + offset.Y, 0.0f + offset.Z);
			Vector3 v4 = new Vector3(left +  offset.X, bottom + offset.Y, 0.0f + offset.Z);

			Matrix4x4.Transform(ref transform, ref v1, out v1);
			Matrix4x4.Transform(ref transform, ref v2, out v2);
			Matrix4x4.Transform(ref transform, ref v3, out v3);
			Matrix4x4.Transform(ref transform, ref v4, out v4);

			//

			_vertices.SetPosition3D(baseVertexIndex + 0, ref v1);
			_vertices.SetPosition3D(baseVertexIndex + 1, ref v2);
			_vertices.SetPosition3D(baseVertexIndex + 2, ref v3);
			_vertices.SetPosition3D(baseVertexIndex + 3, ref v4);

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

		private void GetTransformFor(BillboardSpriteType type, ref Vector3 position, out Matrix4x4 transform)
		{
			switch (type)
			{
				case BillboardSpriteType.Spherical:
					Matrix4x4.CreateBillboard(ref position, ref _cameraPosition, ref _up, ref _cameraForward, out transform);
					break;
				case BillboardSpriteType.Cylindrical:
					Matrix4x4.CreateCylindricalBillboard(ref position, ref _cameraPosition, ref _cameraForward, ref _yAxis, out transform);
					break;
				case BillboardSpriteType.ScreenAligned:
					Matrix4x4.CreateScreenAlignedBillboard(ref position, ref _up, ref _cameraForward, out transform);
					break;
				case BillboardSpriteType.ScreenAndAxisAligned:
					Matrix4x4.CreateScreenAndAxisAlignedBillboard(ref position, ref _cameraForward, ref _yAxis, out transform);
					break;
				default:
					throw new NotImplementedException();
			}
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

		#endregion
	}
}

