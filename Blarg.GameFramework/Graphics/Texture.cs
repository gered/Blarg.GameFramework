using System;
using PortableGL;

namespace Blarg.GameFramework.Graphics
{
	public enum TextureFormat
	{
		RGB,
		RGBA,
		Alpha,
		Depth
	}

	public class Texture : GraphicsContextResource
	{
		TextureParameters _textureParams;

		public int ID { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public TextureFormat Format { get; private set; }

		public bool IsInvalidated
		{
			get { return ID == -1; }
		}

		public Texture(GraphicsDevice graphicsDevice, Image image, TextureParameters textureParameters = null)
			: base(graphicsDevice)
		{
			// if this was specified, we should make our own separate copy of it as these 
			// parameters should stay the same when/if the texture is recreated
			if (textureParameters != null)
				_textureParams = (TextureParameters)textureParameters.Clone();

			CreateTexture(image);
		}

		public Texture(GraphicsDevice graphicsDevice, int width, int height, TextureFormat format, TextureParameters textureParameters = null)
			: base(graphicsDevice)
		{
			// if this was specified, we should make our own separate copy of it as these 
			// parameters should stay the same when/if the texture is recreated
			if (textureParameters != null)
				_textureParams = (TextureParameters)textureParameters.Clone();

			CreateTexture(width, height, format);
		}

		#region OpenGL Texture Creation

		private void CreateTexture(Image image)
		{
			if (image == null)
				throw new ArgumentNullException("image");

			if (!GraphicsDevice.IsNonPowerOfTwoTextureSupported)
			{
				if (!MathHelpers.IsPowerOf2(image.Width) || !MathHelpers.IsPowerOf2(image.Height))
					throw new InvalidOperationException();
			}
			if (image.BitsPerPixel == 8 && image.PixelFormat != ImageFormat.A)
				throw new InvalidOperationException();
			if (image.BitsPerPixel == 16)
				throw new InvalidOperationException();

			TextureFormat textureFormat;
			int internalFormat;
			int pixelFormat;
			int pixelType = GL20.GL_UNSIGNED_BYTE;

			if (image.PixelFormat == ImageFormat.A)
			{
				textureFormat = TextureFormat.Alpha;
				internalFormat = GL20.GL_ALPHA;
				pixelFormat = GL20.GL_ALPHA;
			}
			else
			{
				if (image.BitsPerPixel == 24)
				{
					textureFormat = TextureFormat.RGB;
					internalFormat = GL20.GL_RGB;
					pixelFormat = GL20.GL_RGB;
				}
				else if (image.BitsPerPixel == 32)
				{
					textureFormat = TextureFormat.RGBA;
					internalFormat = GL20.GL_RGBA;
					pixelFormat = GL20.GL_RGBA;
				}
				else
					throw new InvalidOperationException();
			}

			Width = image.Width;
			Height = image.Height;
			Format = textureFormat;

			ID = GraphicsDevice.GL.glGenTextures();

			GraphicsDevice.BindTexture(this, 0);

			if (_textureParams == null)
				_textureParams = GraphicsDevice.GetCopyOfTextureParameters();
			_textureParams.Apply(GraphicsDevice);

			GraphicsDevice.GL.glTexImage2D(GL20.GL_TEXTURE_2D, 0, internalFormat, Width, Height, 0, pixelFormat, pixelType, image.Pixels);

			Framework.Logger.Info(GraphicsContextResource.LOG_TAG, "Created texture from image. ID = {0}, bpp = {1}, size = {2}x{3}.", ID, image.BitsPerPixel, Width, Height);
		}

		private void CreateTexture(int width, int height, TextureFormat format, bool useExistingTextureParams = false)
		{
			if (!GraphicsDevice.IsNonPowerOfTwoTextureSupported)
			{
				if (!MathHelpers.IsPowerOf2(width) || !MathHelpers.IsPowerOf2(height))
					throw new InvalidOperationException("Texture dimensions must be a power of 2.");
			}

			int bpp = 0;
			int internalFormat;
			int pixelFormat;
			int pixelType;
			GetTextureSpecsFromFormat(format, out bpp, out internalFormat, out pixelFormat, out pixelType);

			if (bpp == 0)
				throw new InvalidOperationException();

			Width = width;
			Height = height;
			Format = format;

			ID = GraphicsDevice.GL.glGenTextures();

			GraphicsDevice.BindTexture(this, 0);

			if (!useExistingTextureParams || _textureParams == null)
				_textureParams = GraphicsDevice.GetCopyOfTextureParameters();
			_textureParams.Apply(GraphicsDevice);

			GraphicsDevice.GL.glTexImage2D(GL20.GL_TEXTURE_2D, 0, internalFormat, Width, Height, 0, pixelFormat, pixelType, IntPtr.Zero);

			if (Format == TextureFormat.Depth)
				Framework.Logger.Info(GraphicsContextResource.LOG_TAG, "Created uninitialized texture. ID = {0}, depth component only, size = {1}x{2}", ID, Width, Height);
			else
				Framework.Logger.Info(GraphicsContextResource.LOG_TAG, "Created uninitialized texture. ID = {0}, bpp = {1}, size = {2}x{3}", ID, bpp, Width, Height);
		}

		#endregion

		public void Update(Image image, int destX = 0, int destY = 0)
		{
			if (IsInvalidated)
				throw new InvalidOperationException();
			if (Format == TextureFormat.Depth)
				throw new InvalidOperationException();

			if (image == null)
				throw new ArgumentNullException("image");
			if (destX < 0 || destX >= Width)
				throw new InvalidOperationException();
			if (destY < 0 || destY >= Height)
				throw new InvalidOperationException();
			if (image.Width > Width)
				throw new InvalidOperationException();
			if (image.Height > Height)
				throw new InvalidOperationException();
			if ((destX + image.Width) > Width)
				throw new InvalidOperationException();
			if ((destY + image.Height) > Height)
				throw new InvalidOperationException();

			if (image.BitsPerPixel == 8 && image.PixelFormat != ImageFormat.A)
				throw new InvalidOperationException();
			if (image.BitsPerPixel == 16)
				throw new InvalidOperationException();

			int pixelFormat;
			int pixelType = GL20.GL_UNSIGNED_BYTE;

			if (image.PixelFormat == ImageFormat.A)
				pixelFormat = GL20.GL_ALPHA;
			else
			{
				if (image.BitsPerPixel == 24)
					pixelFormat = GL20.GL_RGB;
				else if (image.BitsPerPixel == 32)
					pixelFormat = GL20.GL_RGBA;
				else
					throw new InvalidOperationException();
			}

			GraphicsDevice.BindTexture(this, 0);
			GraphicsDevice.GL.glTexSubImage2D(GL20.GL_TEXTURE_2D, 0, destX, destY, image.Width, image.Height, pixelFormat, pixelType, image.Pixels);
		}

		private void GetTextureSpecsFromFormat(TextureFormat textureFormat, out int bpp, out int internalFormat, out int pixelFormat, out int type)
		{
			switch (textureFormat)
			{
				case TextureFormat.Alpha:
					bpp = 8;
					internalFormat = GL20.GL_ALPHA;
					pixelFormat = GL20.GL_ALPHA;
					type = GL20.GL_UNSIGNED_BYTE;
					break;

				case TextureFormat.RGB:
					bpp = 24;
					internalFormat = GL20.GL_RGB;
					pixelFormat = GL20.GL_RGB;
					type = GL20.GL_UNSIGNED_BYTE;
					break;

				case TextureFormat.RGBA:
					bpp = 32;
					internalFormat = GL20.GL_RGBA;
					pixelFormat = GL20.GL_RGBA;
					type = GL20.GL_UNSIGNED_BYTE;
					break;

				case TextureFormat.Depth:
					bpp = 0;        // doesn't really matter for this one... ?
					internalFormat = GL20.GL_DEPTH_COMPONENT;
					pixelFormat = GL20.GL_DEPTH_COMPONENT;

					// TODO: check that these are correct ...
					if (Framework.Application.PlatformType == PlatformType.Mobile)
						type = GL20.GL_UNSIGNED_SHORT;
					else
						type = GL20.GL_FLOAT;
					break;

				default:
					bpp = 0;

					// junk -- just to appease the compiler
					internalFormat = GL20.GL_RGBA;
					pixelFormat = GL20.GL_RGBA;
					type = GL20.GL_UNSIGNED_BYTE;
					break;
			}
		}
		#region GraphicsContextResource

		public override void OnNewContext()
		{
			// TODO: recreate empty texture with same width/height/format
			//       (it is up to the application code to refill proper image data,
			//       or the ContentManager if loaded that way)
			CreateTexture(Width, Height, Format, true);
		}

		public override void OnLostContext()
		{
			ID = -1;
		}

		protected override bool ReleaseResource()
		{
			// this needs to happen before the OpenGL context is destroyed
			// which is not guaranteed if we rely 100% on the GC to clean 
			// everything up. best solution is to ensure all Texture
			// objects are not being referenced before the window is
			// closed and do a GC.Collect()

			if (!IsInvalidated)
			{
				if (GraphicsDevice != null)
					GraphicsDevice.UnbindTexture(this);

				GraphicsDevice.GL.glDeleteTextures(ID);

				Framework.Logger.Info(GraphicsContextResource.LOG_TAG, "Deleted Texture ID = {0}.", ID);

				ID = -1;
			}

			return true;
		}

		#endregion
	}
}
