using System;
using PortableGL;

namespace Blarg.GameFramework.Graphics
{
	public enum MinificationFilter
	{
		Nearest,
		Linear,
		NearestMipmapNearest,
		LinearMipmapNearest,
		NearestMipmapLinear,
		LinearMipmapLinear
	}

	public enum MagnificationFilter
	{
		Nearest,
		Linear
	}

	public enum WrapMode
	{
		ClampToEdge,
		Repeat
	}

	public class TextureParameters
	{
		public static readonly TextureParameters Default;
		public static readonly TextureParameters Pixelated;

		public MinificationFilter MinFilter;
		public MagnificationFilter MagFilter;
		public WrapMode WrapS;
		public WrapMode WrapT;

		static TextureParameters()
		{
			Default = new TextureParameters();
			Pixelated = new TextureParameters(MinificationFilter.Nearest, MagnificationFilter.Nearest);
		}

		public TextureParameters()
		{
			Init();
		}

		public TextureParameters(MinificationFilter minFilter, MagnificationFilter magFilter)
		{
			Init();
			MinFilter = minFilter;
			MagFilter = magFilter;
		}

		public void Apply(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			int minFilter = GL20.GL_NEAREST;
			int magFilter = GL20.GL_LINEAR;
			int wrapS = GL20.GL_REPEAT;
			int wrapT = GL20.GL_REPEAT;

			switch (MinFilter)
			{
				case MinificationFilter.Nearest:              minFilter = GL20.GL_NEAREST; break;
				case MinificationFilter.Linear:               minFilter = GL20.GL_LINEAR; break;
				case MinificationFilter.NearestMipmapNearest: minFilter = GL20.GL_NEAREST_MIPMAP_NEAREST; break;
				case MinificationFilter.LinearMipmapNearest:  minFilter = GL20.GL_LINEAR_MIPMAP_NEAREST; break;
				case MinificationFilter.NearestMipmapLinear:  minFilter = GL20.GL_NEAREST_MIPMAP_LINEAR; break;
				case MinificationFilter.LinearMipmapLinear:   minFilter = GL20.GL_LINEAR_MIPMAP_LINEAR; break;
			}

			switch (MagFilter)
			{
				case MagnificationFilter.Nearest: magFilter = GL20.GL_NEAREST; break;
				case MagnificationFilter.Linear:  magFilter = GL20.GL_LINEAR; break;
			}

			switch (WrapS)
			{
				case WrapMode.ClampToEdge: wrapS = GL20.GL_CLAMP_TO_EDGE; break;
				case WrapMode.Repeat:      wrapS = GL20.GL_REPEAT; break;
			}

			switch (WrapT)
			{
				case WrapMode.ClampToEdge: wrapT = GL20.GL_CLAMP_TO_EDGE; break;
				case WrapMode.Repeat:      wrapT = GL20.GL_REPEAT; break;
			}

			graphicsDevice.GL.glTexParameteri(GL20.GL_TEXTURE_2D, GL20.GL_TEXTURE_MIN_FILTER, (int)minFilter);
			graphicsDevice.GL.glTexParameteri(GL20.GL_TEXTURE_2D, GL20.GL_TEXTURE_MAG_FILTER, (int)magFilter);
			graphicsDevice.GL.glTexParameteri(GL20.GL_TEXTURE_2D, GL20.GL_TEXTURE_WRAP_S, (int)wrapS);
			graphicsDevice.GL.glTexParameteri(GL20.GL_TEXTURE_2D, GL20.GL_TEXTURE_WRAP_T, (int)wrapT);
		}

		private void Init()
		{
			MinFilter = MinificationFilter.Nearest;
			MagFilter = MagnificationFilter.Linear;
			WrapS = WrapMode.Repeat;
			WrapT = WrapMode.Repeat;
		}

		public TextureParameters Clone()
		{
			var clone = new TextureParameters();
			clone.MagFilter = MagFilter;
			clone.MinFilter = MinFilter;
			clone.WrapS = WrapS;
			clone.WrapT = WrapT;
			return clone;
		}
	}
}
