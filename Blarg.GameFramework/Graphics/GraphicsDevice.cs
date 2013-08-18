using System;
using System.Collections.Generic;
using PortableGL;
using Blarg.GameFramework;
using Blarg.GameFramework.Graphics.BuiltinShaders;
using Blarg.GameFramework.Resources;

namespace Blarg.GameFramework.Graphics
{
	public class GraphicsDevice : IDisposable
	{
		private const int MaxTextureUnits = 8;
		private const int MaxGpuAttributeSlots = 8;
		private const int SolidColorTextureWidth = 8;
		private const int SolidColorTextureHeight = 8;

		private IList<GraphicsContextResource> _managedResources;
		private IDictionary<int, Texture> _solidColorTextures;
		private VertexBuffer _boundVertexBuffer;
		private IndexBuffer _boundIndexBuffer;
		private Texture[] _boundTextures;
		private Renderbuffer _boundRenderbuffer;
		private Framebuffer _boundFramebuffer;
		private Shader _boundShader;
		private bool _isShaderVertexAttribsSet;
		private Stack<int> _enabledVertexAttribIndices;
		private ViewContext _defaultViewContext;
		private ViewContext _activeViewContext;
		private TextureParameters _currentTextureParams;
		private TextureParameters _solidColorTextureParams;

		public ScreenOrientation ScreenOrientation { get; private set; }

		public bool IsNonPowerOfTwoTextureSupported { get; private set; }
		public bool IsDepthTextureSupported { get; private set; }

		public StandardShader DebugShader { get; private set; }
		public StandardShader SimpleColorShader { get; private set; }
		public StandardShader SimpleColorTextureShader { get; private set; }
		public StandardShader SimpleTextureShader { get; private set; }
		public VertexLerpShader SimpleTextureVertexLerpShader { get; private set; }
		public VertexSkinningShader SimpleTextureVertexSkinningShader { get; private set; }
		public SpriteShader Sprite2DShader { get; private set; }
		public SpriteShader Sprite3DShader { get; private set; }

		public SpriteFont SansSerifFont { get; private set; }
		public SpriteFont MonospaceFont { get; private set; }

		public GeometryDebugRenderer DebugRenderer { get; private set; }

		public ViewContext ViewContext
		{ 
			get
			{
				return _activeViewContext;
			}
			set
			{
				if (value == _activeViewContext)
					return;

				if (value == null)
					_activeViewContext = _defaultViewContext;
				else
					_activeViewContext = value;

				Rect r = Platform.Application.Window.ClientRectangle;
				_activeViewContext.OnApply(ref r, ScreenOrientation);
			}
		}

		public GraphicsDevice()
		{
			ScreenOrientation = ScreenOrientation.Rotation0;

			_boundTextures = new Texture[MaxTextureUnits];
			_enabledVertexAttribIndices = new Stack<int>(MaxGpuAttributeSlots);

			string vendor = Platform.GL.glGetString(GL20.GL_VENDOR);
			string renderer = Platform.GL.glGetString(GL20.GL_RENDERER);
			string version = Platform.GL.glGetString(GL20.GL_VERSION);
			string extensions = Platform.GL.glGetString(GL20.GL_EXTENSIONS);
			string shadingLangVersion = Platform.GL.glGetString(GL20.GL_SHADING_LANGUAGE_VERSION);

			Platform.Logger.Info("Graphics", "GL_VENDOR = {0}", vendor);
			Platform.Logger.Info("Graphics", "GL_RENDERER = {0}", renderer);
			Platform.Logger.Info("Graphics", "GL_VERSION = {0}", version);
			Platform.Logger.Info("Graphics", "GL_EXTENSIONS = {0}", extensions);
			Platform.Logger.Info("Graphics", "GL_SHADING_LANGUAGE_VERSION = {0}", shadingLangVersion);

			if (Platform.Type == PlatformType.Mobile)
			{
				IsNonPowerOfTwoTextureSupported = extensions.Contains("OES_texture_npot");
				IsDepthTextureSupported = extensions.Contains("OES_depth_texture");
			}
			else
			{
				IsNonPowerOfTwoTextureSupported = extensions.Contains("ARB_texture_non_power_of_two");
				IsDepthTextureSupported = extensions.Contains("ARB_depth_texture");
			}

			_defaultViewContext = new ViewContext(this);
			_activeViewContext = _defaultViewContext;

			_currentTextureParams = TextureParameters.Default;
			_solidColorTextureParams = TextureParameters.Pixelated;  // no filtering

			_managedResources = new List<GraphicsContextResource>();
			_solidColorTextures = new Dictionary<int, Texture>();

			LoadStandardShaders();
			LoadStandardFonts();

			DebugRenderer = new GeometryDebugRenderer(this);
		}

		private void LoadStandardShaders()
		{
			DebugShader = new DebugShader(this);
			SimpleColorShader = new SimpleColorShader(this);
			SimpleColorTextureShader = new SimpleColorTextureShader(this);
			SimpleTextureShader = new SimpleTextureShader(this);
			SimpleTextureVertexLerpShader = new SimpleTextureVertexLerpShader(this);
			SimpleTextureVertexSkinningShader = new SimpleTextureVertexSkinningShader(this);
			Sprite2DShader = new Sprite2DShader(this);
			Sprite3DShader = new Sprite3DShader(this);
		}

		private void LoadStandardFonts()
		{
			var sansSerifFontStream = ResourceUtils.GetResource("Blarg.GameFramework.Resources.Fonts.Vera.ttf");
			SansSerifFont = SpriteFontTrueTypeLoader.Load(this, sansSerifFontStream, 16, SansSerifFont);

			var monospaceFontStream = ResourceUtils.GetResource("Blarg.GameFramework.Resources.Fonts.VeraMono.ttf");
			MonospaceFont = SpriteFontTrueTypeLoader.Load(this, monospaceFontStream, 16, MonospaceFont);
		}

		public void OnLostContext()
		{
			Platform.Logger.Info("Graphics", "Cleaning up objects/state specific to the lost OpenGL context.");

			_activeViewContext.OnLostContext();

			Platform.Logger.Info("Graphics", "Invoking OnLostContext callback for managed resources.");

			foreach (var resource in _managedResources)
				resource.OnLostContext();

			Platform.Logger.Info("Graphics", "Finished cleaning up lost managed resources.");
		}

		public void OnNewContext()
		{
			Platform.Logger.Info("Graphics", "Initializing default state for new OpenGL context.");

			_activeViewContext.OnNewContext();

			RenderState.Default.Apply();
			BlendState.Default.Apply();

			UnbindVertexBuffer();
			UnbindIndexBuffer();
			for (int i = 0; i < MaxTextureUnits; ++i)
				UnbindTexture(i);
			UnbindShader();
			UnbindRenderbuffer();
			UnbindFramebuffer();

			Platform.Logger.Info("Graphics", "Invoking OnNewContext callback for managed resources.");

			foreach (var resource in _managedResources)
				resource.OnNewContext();

			Platform.Logger.Info("Graphics", "Finished restoring managed resources.");

			Platform.Logger.Info("Graphics", "Restoring image data for solid color texture cache.");

			foreach (var texture in _solidColorTextures)
			{
				Color color = Color.FromInt(texture.Key);
				FillSolidColorTexture(texture.Value, ref color);
			}

			Platform.Logger.Info("Graphics", "Restoring standard fonts.");
			LoadStandardFonts();
		}

		public void OnUnload()
		{
			Platform.Logger.Info("Graphics", "Unloading managed resources.");
			while (_managedResources.Count > 0)
			{
				var resource = _managedResources[0];
				resource.Dispose();
			}
			_managedResources.Clear();
		}

		public void OnRender(float delta)
		{
			int error = Platform.GL.glGetError();
			System.Diagnostics.Debug.Assert(error == GL20.GL_NO_ERROR);
			if (error != GL20.GL_NO_ERROR)
			{
				Platform.Logger.Error("OpenGL", "OpenGL error \"{0}\"", error.ToString());

				// keep checking for and reporting errors until there are no more left
				while ((error = Platform.GL.glGetError()) != GL20.GL_NO_ERROR)
					Platform.Logger.Error("OpenGL", "OpenGL error \"{0}\"", error.ToString());
			}

			_activeViewContext.OnRender(delta);
		}

		public void OnResize(ref Rect rect, ScreenOrientation orientation)
		{
			Platform.Logger.Info("Graphics", "Window resized ({0}, {1}) - ({2}, {3}).", rect.Left, rect.Top, rect.Width, rect.Height);
			if (orientation != ScreenOrientation.Rotation0)
				Platform.Logger.Info("Graphics", "Screen is rotated (angle = {0}).", (int)orientation);

			ScreenOrientation = orientation;
			_activeViewContext.OnResize(ref rect, orientation);
		}

		public void Clear(float r, float g, float b, float a = Color.AlphaOpaque)
		{
			var color = new Color(r, g, b, a);
			Clear(ref color);
			Platform.GL.glClearColor(r, g, b, a);
		}

		public void Clear(Color color)
		{
			Clear(ref color);
		}

		public void Clear(ref Color color)
		{
			Platform.GL.glClearColor(color.R, color.G, color.B, color.A);
			Platform.GL.glClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT);
		}

		public void SetTextureParameters(TextureParameters parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters");

			_currentTextureParams = (TextureParameters)parameters.Clone();
		}

		internal TextureParameters GetCopyOfTextureParameters()
		{
			return (TextureParameters)_currentTextureParams.Clone();
		}

		#region Solid Color Textures

		public Texture GetSolidColorTexture(Color color)
		{
			return GetSolidColorTexture(ref color);
		}

		public Texture GetSolidColorTexture(ref Color color)
		{
			int rgba = color.RGBA;
			Texture result;
			_solidColorTextures.TryGetValue(rgba, out result);
			if (result == null)
			{
				result = CreateSolidColorTexture(ref color);
				_solidColorTextures.Add(rgba, result);
			}

			return result;
		}

		private Texture CreateSolidColorTexture(ref Color color)
		{
			Platform.Logger.Info("Graphics", "Creating texture for solid color 0x{0:x}.", color.RGBA);

			var solidColorImage = new Image(SolidColorTextureWidth, SolidColorTextureHeight, ImageFormat.RGBA);
			solidColorImage.Clear(ref color);

			var texture = new Texture(this, solidColorImage, _solidColorTextureParams);
			return texture;
		}

		private void FillSolidColorTexture(Texture texture, ref Color color)
		{
			Platform.Logger.Info("Graphics", "Filling image data for solid color texture using color 0x{0:x}.", color.RGBA);

			if (texture == null || texture.IsInvalidated || texture.Width != SolidColorTextureWidth || texture.Height != SolidColorTextureHeight)
				throw new ArgumentException("Invalid texture.");

			var solidColorImage = new Image(SolidColorTextureWidth, SolidColorTextureHeight, ImageFormat.RGBA);
			solidColorImage.Clear(ref color);

			texture.Update(solidColorImage);
		}

		#endregion

		#region Binding

		public void BindTexture(Texture texture, int unit = 0)
		{
			if (unit < 0 || unit >= MaxTextureUnits)
				throw new ArgumentOutOfRangeException("unit");
			if (texture == null || texture.IsInvalidated)
				throw new ArgumentException("Invalid texture.");

			if (texture != _boundTextures[unit])
			{
				Platform.GL.glActiveTexture(GL20.GL_TEXTURE0 + unit);
				Platform.GL.glBindTexture(GL20.GL_TEXTURE_2D, texture.ID);

				_boundTextures[unit] = texture;
			}
		}

		public void UnbindTexture(int unit = 0)
		{
			if (unit < 0 || unit >= MaxTextureUnits)
				throw new ArgumentOutOfRangeException("unit");

			Platform.GL.glActiveTexture(GL20.GL_TEXTURE0 + unit);
			Platform.GL.glBindTexture(GL20.GL_TEXTURE_2D, 0);

			_boundTextures[unit] = null;
		}

		public void UnbindTexture(Texture texture)
		{
			if (texture == null || texture.IsInvalidated)
				throw new ArgumentException("Invalid texture.");

			for (int i = 0; i < MaxTextureUnits; ++i)
			{
				if (_boundTextures[i] == texture)
					UnbindTexture(i);
			}
		}

		public void BindRenderbuffer(Renderbuffer renderbuffer)
		{
			if (renderbuffer == null || renderbuffer.IsInvalidated)
				throw new ArgumentException("Invalid renderbuffer.");

			if (_boundRenderbuffer != renderbuffer)
			{
				Platform.GL.glBindRenderbuffer(GL20.GL_RENDERBUFFER, renderbuffer.ID);
				_boundRenderbuffer = renderbuffer;
			}
		}

		public void UnbindRenderbuffer()
		{
			Platform.GL.glBindRenderbuffer(GL20.GL_RENDERBUFFER, 0);
			_boundRenderbuffer = null;
		}

		public void UnbindRenderbuffer(Renderbuffer renderbuffer)
		{
			if (renderbuffer == null)
				throw new ArgumentNullException("renderbuffer");

			if (renderbuffer == _boundRenderbuffer)
				UnbindRenderbuffer();
		}

		public void BindFramebuffer(Framebuffer framebuffer)
		{
			if (framebuffer == null || framebuffer.IsInvalidated)
				throw new ArgumentException("Invalid framebuffer.");

			if (_boundFramebuffer != framebuffer)
			{
				Platform.GL.glBindFramebuffer(GL20.GL_FRAMEBUFFER, framebuffer.ID);
				_boundFramebuffer = framebuffer;
			}
		}

		public void UnbindFramebuffer()
		{
			Platform.GL.glBindFramebuffer(GL20.GL_FRAMEBUFFER, 0);
			_boundFramebuffer = null;
		}

		public void UnbindFramebuffer(Framebuffer framebuffer)
		{
			if (framebuffer == null)
				throw new ArgumentNullException("framebuffer");

			if (framebuffer == _boundFramebuffer)
				UnbindFramebuffer();
		}

		public void BindVertexBuffer(VertexBuffer buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (buffer.NumElements <= 0)
				throw new InvalidOperationException();

			if (_boundVertexBuffer == buffer)
				return;

			if (buffer.IsClientSide)
				BindVertexClientArrays(buffer);
			else
				BindVBO(buffer);

			_boundVertexBuffer = buffer;

			if (_isShaderVertexAttribsSet)
				ClearSetShaderVertexAttributes();
		}

		public void UnbindVertexBuffer()
		{
			Platform.GL.glBindBuffer(GL20.GL_ARRAY_BUFFER, 0);
			_boundVertexBuffer = null;

			if (_isShaderVertexAttribsSet)
				ClearSetShaderVertexAttributes();
		}

		private void BindVBO(VertexBuffer buffer)
		{
			if (buffer.IsDirty)
				buffer.Update();

			Platform.GL.glBindBuffer(GL20.GL_ARRAY_BUFFER, buffer.ID);
		}

		private void BindVertexClientArrays(VertexBuffer buffer)
		{
			Platform.GL.glBindBuffer(GL20.GL_ARRAY_BUFFER, 0);
		}

		public void BindIndexBuffer(IndexBuffer buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (buffer.NumElements <= 0)
				throw new InvalidOperationException();

			if (_boundIndexBuffer == buffer)
				return;

			if (buffer.IsClientSide)
				BindIndexClientArray(buffer);
			else
				BindIBO(buffer);

			_boundIndexBuffer = buffer;
		}

		public void UnbindIndexBuffer()
		{
			Platform.GL.glBindBuffer(GL20.GL_ELEMENT_ARRAY_BUFFER, 0);
			_boundIndexBuffer = null;
		}

		private void BindIBO(IndexBuffer buffer)
		{
			if (buffer.IsDirty)
				buffer.Update();

			Platform.GL.glBindBuffer(GL20.GL_ELEMENT_ARRAY_BUFFER, buffer.ID);
		}

		private void BindIndexClientArray(IndexBuffer buffer)
		{
			Platform.GL.glBindBuffer(GL20.GL_ELEMENT_ARRAY_BUFFER, 0);
		}

		public void BindShader(Shader shader)
		{
			if (shader == null)
				throw new ArgumentNullException("shader");
			if (!shader.IsReadyForUse)
				throw new InvalidOperationException("Shader hasn't been compiled and linked.");

			Platform.GL.glUseProgram(shader.ProgramID);
			_boundShader = shader;

			if (_isShaderVertexAttribsSet)
				ClearSetShaderVertexAttributes();

			_boundShader.OnBind();
		}

		public void UnbindShader()
		{
			Platform.GL.glUseProgram(0);

			if (_boundShader != null)
				_boundShader.OnUnbind();
			_boundShader = null;

			if (_isShaderVertexAttribsSet)
				ClearSetShaderVertexAttributes();
		}

		private void SetShaderVertexAttributes()
		{
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");
			if (_boundShader == null)
				throw new InvalidOperationException("No bound shader.");
			if (_boundVertexBuffer.NumAttributes < _boundShader.NumAttributes)
				throw new InvalidOperationException("The bound vertex buffer does not have enough attributes to use the bound shader.");

			for (int i = 0; i < _boundShader.NumAttributes; ++i)
			{
				int bufferAttribIndex;
				int offset;
				int size;

				if (_boundShader.IsMappedToVBOStandardAttribute(i))
				{
					// "automatic attribute index discovery" by mapping via standard types
					var standardType = _boundShader.GetMappedVBOStandardAttribute(i);
					bufferAttribIndex = _boundVertexBuffer.GetIndexOfStandardAttribute(standardType);
					if (bufferAttribIndex == -1)
						throw new InvalidOperationException("Standard attribute type not present in bound vertex buffer.");
				}
				else
					bufferAttribIndex = _boundShader.GetMappedVBOAttributeIndexFor(i);

				// offset value will be in terms of floats (not bytes)
				offset = _boundVertexBuffer.GetAttributeOffset(bufferAttribIndex);
				size = _boundVertexBuffer.GetAttributeSize(bufferAttribIndex);

				Platform.GL.glEnableVertexAttribArray(i);
				if (_boundVertexBuffer.IsClientSide)
				{
					// pass reference to the first vertex and the first float within that vertex that is for this attribute
					unsafe
					{
						fixed (float *p = _boundVertexBuffer.Data)
						{
							float *src = p + offset;
							Platform.GL.glVertexAttribPointer(i, size, GL20.GL_FLOAT, false, _boundVertexBuffer.ElementWidthInBytes, new IntPtr((long)src));
						}
					}

				}
				else
					// pass offset in bytes (starting from zero) that corresponds with the start of this attribute
					Platform.GL.glVertexAttribPointer(i, size, GL20.GL_FLOAT, false, _boundVertexBuffer.ElementWidthInBytes, (IntPtr)(offset * sizeof(float)));

				_enabledVertexAttribIndices.Push(i);
			}

			_isShaderVertexAttribsSet = true;
		}

		private void ClearSetShaderVertexAttributes()
		{
			while (_enabledVertexAttribIndices.Count > 0)
			{
				int index = _enabledVertexAttribIndices.Pop();
				Platform.GL.glDisableVertexAttribArray(index);
			}

			_isShaderVertexAttribsSet = false;
		}

		private bool IsReadyToRender
		{
			get
			{
				if (_boundShader != null && _boundVertexBuffer != null && _isShaderVertexAttribsSet)
					return true;
				else
					return false;
			}
		}

		#endregion

		#region Rendering

		public void RenderTriangles(IndexBuffer buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (!buffer.IsClientSide)
				throw new InvalidOperationException("GPU index buffers must be bound first.");
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");
			if (_boundIndexBuffer != null)
				throw new InvalidOperationException("GPU index buffer currently bound.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			int numVertices = buffer.NumElements;
			if (numVertices % 3 != 0)
				throw new InvalidOperationException("Number of elements in index buffer do not perfectly make up a set of triangles.");

			Platform.GL.glDrawElements(GL20.GL_TRIANGLES, numVertices, GL20.GL_UNSIGNED_SHORT, buffer.Data);
		}

		public void RenderTriangles()
		{
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			if (_boundIndexBuffer != null)
			{
				// using bound index buffer
				int numVertices = _boundIndexBuffer.NumElements;
				if (numVertices % 3 != 0)
					throw new InvalidOperationException("Number of elements in bound index buffer do not perfectly make up a set of triangles.");

				if (_boundIndexBuffer.IsClientSide)
					Platform.GL.glDrawElements(GL20.GL_TRIANGLES, numVertices, GL20.GL_UNSIGNED_SHORT, _boundIndexBuffer.Data);
				else
					Platform.GL.glDrawElements(GL20.GL_TRIANGLES, numVertices, GL20.GL_UNSIGNED_SHORT, (IntPtr)0);
			}
			else
			{
				// no index buffer, just render the whole vertex buffer
				int numVertices = _boundVertexBuffer.NumElements;
				if (numVertices % 3 != 0)
					throw new InvalidOperationException("Number of vertices in bound vertex buffer do not perfectly make up a set of triangles.");

				Platform.GL.glDrawArrays(GL20.GL_TRIANGLES, 0, numVertices);
			}
		}

		public void RenderTriangles(int startVertex, int numTriangles)
		{
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			int numVertices = numTriangles * 3;

			if (_boundIndexBuffer != null)
			{
				// using bound index buffer
				if ((_boundIndexBuffer.NumElements - startVertex) < numVertices)
					throw new InvalidOperationException("Bound index buffer does not contain enough elements.");

				if (_boundIndexBuffer.IsClientSide)
				{
					unsafe
					{
						fixed (ushort *p = _boundIndexBuffer.Data)
						{
							ushort *src = p + startVertex;
							Platform.GL.glDrawElements(GL20.GL_TRIANGLES, numVertices, GL20.GL_UNSIGNED_SHORT, new IntPtr((long)src));
						}
					}
				}
				else
				{
					// this offset needs to be in terms of bytes
					int offset = startVertex * _boundIndexBuffer.ElementWidthInBytes;
					Platform.GL.glDrawElements(GL20.GL_TRIANGLES, numVertices, GL20.GL_UNSIGNED_SHORT, (IntPtr)offset);
				}
			}
			else
			{
				// no index buffer, just render the whole vertex buffer
				if ((_boundVertexBuffer.NumElements - startVertex) < numVertices)
					throw new InvalidOperationException("Bound vertex buffer does not contain enough vertices.");

				Platform.GL.glDrawArrays(GL20.GL_TRIANGLES, startVertex, numVertices);
			}
		}

		public void RenderLines(IndexBuffer buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (!buffer.IsClientSide)
				throw new InvalidOperationException("GPU index buffers must be bound first.");
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");
			if (_boundIndexBuffer != null)
				throw new InvalidOperationException("GPU index buffer currently bound.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			int numVertices = buffer.NumElements;
			if (numVertices % 2 != 0)
				throw new InvalidOperationException("Number of elements in index buffer do not perfectly make up a set of lines.");

			Platform.GL.glDrawElements(GL20.GL_LINES, numVertices, GL20.GL_UNSIGNED_SHORT, buffer.Data);
		}

		public void RenderLines()
		{
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			if (_boundIndexBuffer != null)
			{
				// using bound index buffer
				int numVertices = _boundIndexBuffer.NumElements;
				if (numVertices % 2 != 0)
					throw new InvalidOperationException("Number of elements in bound index buffer do not perfectly make up a set of lines.");

				if (_boundIndexBuffer.IsClientSide)
					Platform.GL.glDrawElements(GL20.GL_LINES, numVertices, GL20.GL_UNSIGNED_SHORT, _boundIndexBuffer.Data);
				else
					Platform.GL.glDrawElements(GL20.GL_LINES, numVertices, GL20.GL_UNSIGNED_SHORT, (IntPtr)0);
			}
			else
			{
				// no index buffer, just render the whole vertex buffer
				int numVertices = _boundVertexBuffer.NumElements;
				if (numVertices % 2 != 0)
					throw new InvalidOperationException("Number of vertices in bound vertex buffer do not perfectly make up a set of lines.");

				Platform.GL.glDrawArrays(GL20.GL_LINES, 0, numVertices);
			}
		}

		public void RenderLines(int startVertex, int numLines)
		{
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			int numVertices = numLines * 2;

			if (_boundIndexBuffer != null)
			{
				// using bound index buffer
				if ((_boundIndexBuffer.NumElements - startVertex) < numVertices)
					throw new InvalidOperationException("Bound index buffer does not contain enough elements.");

				if (_boundIndexBuffer.IsClientSide)
				{
					unsafe
					{
						fixed (ushort *p = _boundIndexBuffer.Data)
						{
							ushort *src = p + startVertex;
							Platform.GL.glDrawElements(GL20.GL_LINES, numVertices, GL20.GL_UNSIGNED_SHORT, new IntPtr((long)src));
						}
					}
				}
				else
				{
					// this offset needs to be in terms of bytes
					int offset = startVertex * _boundIndexBuffer.ElementWidthInBytes;
					Platform.GL.glDrawElements(GL20.GL_LINES, numVertices, GL20.GL_UNSIGNED_SHORT, (IntPtr)offset);
				}
			}
			else
			{
				// no index buffer, just render the whole vertex buffer
				if ((_boundVertexBuffer.NumElements - startVertex) < numVertices)
					throw new InvalidOperationException("Bound vertex buffer does not contain enough vertices.");

				Platform.GL.glDrawArrays(GL20.GL_LINES, startVertex, numVertices);
			}
		}

		public void RenderPoints(IndexBuffer buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (!buffer.IsClientSide)
				throw new InvalidOperationException("GPU index buffers must be bound first.");
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");
			if (_boundIndexBuffer != null)
				throw new InvalidOperationException("GPU index buffer currently bound.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			int numVertices = buffer.NumElements;

			Platform.GL.glDrawElements(GL20.GL_POINTS, numVertices, GL20.GL_UNSIGNED_SHORT, buffer.Data);
		}

		public void RenderPoints()
		{
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			if (_boundIndexBuffer != null)
			{
				// using bound index buffer
				int numVertices = _boundIndexBuffer.NumElements;

				if (_boundIndexBuffer.IsClientSide)
					Platform.GL.glDrawElements(GL20.GL_POINTS, numVertices, GL20.GL_UNSIGNED_SHORT, _boundIndexBuffer.Data);
				else
					Platform.GL.glDrawElements(GL20.GL_POINTS, numVertices, GL20.GL_UNSIGNED_SHORT, (IntPtr)0);
			}
			else
			{
				// no index buffer, just render the whole vertex buffer
				int numVertices = _boundVertexBuffer.NumElements;

				Platform.GL.glDrawArrays(GL20.GL_POINTS, 0, numVertices);
			}
		}

		public void RenderPoints(int startVertex, int numPoints)
		{
			if (_boundVertexBuffer == null)
				throw new InvalidOperationException("No bound vertex buffer.");

			if (!_isShaderVertexAttribsSet)
				SetShaderVertexAttributes();

			if (_boundIndexBuffer != null)
			{
				// using bound index buffer
				if ((_boundIndexBuffer.NumElements - startVertex) < numPoints)
					throw new InvalidOperationException("Bound index buffer does not contain enough elements.");

				if (_boundIndexBuffer.IsClientSide)
				{
					unsafe
					{
						fixed (ushort *p = _boundIndexBuffer.Data)
						{
							ushort *src = p + startVertex;
							Platform.GL.glDrawElements(GL20.GL_POINTS, numPoints, GL20.GL_UNSIGNED_SHORT, new IntPtr((long)src));
						}
					}
				}
				else
				{
					// this offset needs to be in terms of bytes
					int offset = startVertex * _boundIndexBuffer.ElementWidthInBytes;
					Platform.GL.glDrawElements(GL20.GL_POINTS, numPoints, GL20.GL_UNSIGNED_SHORT, (IntPtr)offset);
				}
			}
			else
			{
				// no index buffer, just render the whole vertex buffer
				if ((_boundVertexBuffer.NumElements - startVertex) < numPoints)
					throw new InvalidOperationException("Bound vertex buffer does not contain enough vertices.");

				Platform.GL.glDrawArrays(GL20.GL_POINTS, startVertex, numPoints);
			}
		}

		#endregion

		#region OpenGL Resource Management

		public void RegisterManagedResource(GraphicsContextResource resource)
		{
			if (resource == null)
				throw new ArgumentNullException("resource");
			if (resource.IsReleased)
				throw new ArgumentException("Resource has been released already.");
			if (resource.GraphicsDevice == null)
				throw new ArgumentException("Resource is not tied to the graphics context (GraphicsDevice is null).");

			_managedResources.Add(resource);
		}

		public void UnregisterManagedResource(GraphicsContextResource resource)
		{
			if (resource == null)
				throw new ArgumentNullException("resource");

			_managedResources.Remove(resource);
		}

		public void UnregisterAllManagedResources()
		{
			_managedResources.Clear();
		}

		private bool _isGlResourcesReleased = false;

		private void ReleaseGlResources()
		{
			if (_isGlResourcesReleased)
				return;

			while (_managedResources.Count > 0)
			{
				var resource = _managedResources[0];
				resource.Dispose();
			}
			_managedResources.Clear();

			_isGlResourcesReleased = true;

			Platform.Logger.Info("Graphics", "Managed graphics context resources cleaned up.");
		}

		~GraphicsDevice()
		{
			ReleaseGlResources();
		}

		public void Dispose()
		{
			ReleaseGlResources();
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
