using System;
using PortableGL;

namespace Blarg.GameFramework.Graphics
{
	public enum RenderbufferFormat
	{
		RGB,
		RGBA,
		Depth,
		Stencil
	}

	public class Renderbuffer : GraphicsContextResource
	{

		public int ID { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public RenderbufferFormat Format { get; private set; }

		public bool IsInvalidated
		{
			get { return ID == -1; }
		}

		public Renderbuffer(GraphicsDevice graphicsDevice, int width, int height, RenderbufferFormat format)
			: base(graphicsDevice)
		{
			ID = -1;
			Create(width, height, format);
		}

		private void Create(int width, int height, RenderbufferFormat format)
		{
			if (width < 1)
				throw new ArgumentOutOfRangeException("width");
			if (height < 1)
				throw new ArgumentOutOfRangeException("height");
			if (!IsInvalidated)
				throw new InvalidOperationException();

			int glFormat = 0;
			if (Framework.PlatformType == PlatformType.Mobile)
			{
				switch (format)
				{
					case RenderbufferFormat.RGB:     glFormat = GL20.GL_RGB565; break;
					case RenderbufferFormat.RGBA:    glFormat = GL20.GL_RGBA4; break;
					case RenderbufferFormat.Depth:   glFormat = GL20.GL_DEPTH_COMPONENT16; break;
					case RenderbufferFormat.Stencil: glFormat = GL20.GL_STENCIL_INDEX8; break;
				}
			}
			else
			{
				switch (format)
				{
					case RenderbufferFormat.RGB:     glFormat = GL20.GL_RGB; break;
					case RenderbufferFormat.RGBA:    glFormat = GL20.GL_RGBA; break;
					case RenderbufferFormat.Depth:   glFormat = GL20.GL_DEPTH_COMPONENT; break;
					case RenderbufferFormat.Stencil: glFormat = GL20.GL_STENCIL_INDEX; break;
				}
			}
			if (glFormat == 0)
				throw new InvalidOperationException();

			ID = GraphicsDevice.GL.glGenRenderbuffers();

			Width = width;
			Height = height;
			Format = format;

			GraphicsDevice.BindRenderbuffer(this);
			GraphicsDevice.GL.glRenderbufferStorage(GL20.GL_RENDERBUFFER, glFormat, Width, Height);
			GraphicsDevice.UnbindRenderbuffer(this);

			Framework.Logger.Info(GraphicsContextResource.LOG_TAG, "Created renderbuffer. ID = {0}, format = {1}, size = {2}x{3}.", ID, Format.ToString(), Width, Height);
		}

		#region GraphicsContextResource

		public override void OnNewContext()
		{
			Create(Width, Height, Format);
		}

		public override void OnLostContext()
		{
			ID = -1;
		}

		protected override bool ReleaseResource()
		{
			if (!IsInvalidated)
			{
				GraphicsDevice.UnbindRenderbuffer(this);

				GraphicsDevice.GL.glDeleteRenderbuffers(ID);

				Framework.Logger.Info(GraphicsContextResource.LOG_TAG, "Deleted Renderbuffer ID = {0}.", ID);

				ID = -1;
			}

			return true;
		}

		#endregion
	}
}
