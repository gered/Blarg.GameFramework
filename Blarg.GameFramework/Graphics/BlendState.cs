using System;
using PortableGL;

namespace Blarg.GameFramework.Graphics
{
	public enum BlendFactor
	{
		Zero,
		One,
		SrcColor,
		InverseSrcColor,
		DstColor,
		InverseDstColor,
		SrcAlpha,
		InverseSrcAlpha,
		DstAlpha,
		InverseDstAlpha,
		SrcAlphaSaturation,
		ConstantColor,
		ConstantAlpha
	}

	public class BlendState
	{
		public static readonly BlendState Default;
		public static readonly BlendState Opaque;
		public static readonly BlendState AlphaBlend;

		public bool Blending;
		public BlendFactor SourceBlendFactor;
		public BlendFactor DestinationBlendFactor;

		static BlendState()
		{
			Default = new BlendState();
			Opaque = new BlendState();
			AlphaBlend = new BlendState(BlendFactor.SrcAlpha, BlendFactor.InverseSrcAlpha);
		}

		public BlendState()
		{
			Init();
		}

		public BlendState(BlendFactor sourceFactor, BlendFactor destinationFactor)
		{
			Init();
			Blending = true;
			SourceBlendFactor = sourceFactor;
			DestinationBlendFactor = destinationFactor;
		}

		public void Apply(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			if (Blending)
			{
				graphicsDevice.GL.glEnable(GL20.GL_BLEND);

				var source = GL20.GL_ONE;
				var dest = GL20.GL_ZERO;

				// OpenTK is missing enum values for these combos (maybe they're not valid in OpenGL ??)
				System.Diagnostics.Debug.Assert(SourceBlendFactor != BlendFactor.SrcColor);
				System.Diagnostics.Debug.Assert(SourceBlendFactor != BlendFactor.InverseSrcColor);
				System.Diagnostics.Debug.Assert(DestinationBlendFactor != BlendFactor.SrcAlphaSaturation);

				switch (SourceBlendFactor)
				{
					case BlendFactor.Zero:               source = GL20.GL_ZERO; break;
					case BlendFactor.One:                source = GL20.GL_ONE; break;
					case BlendFactor.DstColor:           source = GL20.GL_DST_COLOR; break;
					case BlendFactor.InverseDstColor:    source = GL20.GL_ONE_MINUS_DST_COLOR; break;
					case BlendFactor.SrcAlpha:           source = GL20.GL_SRC_ALPHA; break;
					case BlendFactor.InverseSrcAlpha:    source = GL20.GL_ONE_MINUS_SRC_ALPHA; break;
					case BlendFactor.DstAlpha:           source = GL20.GL_DST_ALPHA; break;
					case BlendFactor.InverseDstAlpha:    source = GL20.GL_ONE_MINUS_DST_ALPHA; break;
					case BlendFactor.ConstantAlpha:      source = GL20.GL_CONSTANT_ALPHA; break;
					case BlendFactor.ConstantColor:      source = GL20.GL_CONSTANT_COLOR; break;
					case BlendFactor.SrcAlphaSaturation: source = GL20.GL_SRC_ALPHA_SATURATE; break;
				}

				switch (DestinationBlendFactor)
				{
					case BlendFactor.Zero:            dest = GL20.GL_ZERO; break;
					case BlendFactor.One:             dest = GL20.GL_ONE; break;
					case BlendFactor.SrcColor:        dest = GL20.GL_SRC_COLOR; break;
					case BlendFactor.InverseSrcColor: dest = GL20.GL_ONE_MINUS_SRC_COLOR; break;
					case BlendFactor.DstColor:        dest = GL20.GL_DST_COLOR; break;
					case BlendFactor.InverseDstColor: dest = GL20.GL_ONE_MINUS_DST_COLOR; break;
					case BlendFactor.SrcAlpha:        dest = GL20.GL_SRC_ALPHA; break;
					case BlendFactor.InverseSrcAlpha: dest = GL20.GL_ONE_MINUS_SRC_ALPHA; break;
					case BlendFactor.DstAlpha:        dest = GL20.GL_DST_ALPHA; break;
					case BlendFactor.InverseDstAlpha: dest = GL20.GL_ONE_MINUS_DST_ALPHA; break;
					case BlendFactor.ConstantAlpha:   dest = GL20.GL_CONSTANT_ALPHA; break;
					case BlendFactor.ConstantColor:   dest = GL20.GL_CONSTANT_COLOR; break;
				}

				graphicsDevice.GL.glBlendFunc(source, dest);
			}
			else
				graphicsDevice.GL.glDisable(GL20.GL_BLEND);
		}

		private void Init()
		{
			Blending = false;
			SourceBlendFactor = BlendFactor.One;
			DestinationBlendFactor = BlendFactor.Zero;
		}

		public BlendState Clone()
		{
			var clone = new BlendState();
			clone.Blending = Blending;
			clone.DestinationBlendFactor = DestinationBlendFactor;
			clone.SourceBlendFactor = SourceBlendFactor;
			return clone;
		}
	}
}
