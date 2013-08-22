using System;
using System.IO;

namespace Blarg.GameFramework.Graphics
{
	public abstract class SpriteShader : StandardShader
	{
		protected string TextureHasAlphaOnlyUniformName;

		protected SpriteShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			Initialize();
		}

		protected SpriteShader(GraphicsDevice graphicsDevice, string vertexShaderSource, string fragmentShaderSource)
			: base(graphicsDevice, vertexShaderSource, fragmentShaderSource)
		{
			Initialize();
		}

		protected SpriteShader(GraphicsDevice graphicsDevice, TextReader vertexShaderSourceReader, TextReader fragmentShaderSourceReader)
			: base(graphicsDevice, vertexShaderSourceReader, fragmentShaderSourceReader)
		{
			Initialize();
		}

		private void Initialize()
		{
			TextureHasAlphaOnlyUniformName = "u_textureHasAlphaOnly";
		}

		public void SetTextureHasAlphaOnly(bool hasAlphaOnly)
		{
			if (!IsReadyForUse)
				throw new InvalidOperationException();
			SetUniform(TextureHasAlphaOnlyUniformName, Convert.ToInt32(hasAlphaOnly));
		}
	}
}
