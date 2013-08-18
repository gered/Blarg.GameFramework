using System;
using System.IO;

namespace Blarg.GameFramework.Graphics.CustomShaders
{
	public class CustomSpriteShader : SpriteShader
	{
		public CustomSpriteShader(GraphicsDevice graphicsDevice, string vertexShaderSource, string fragmentShaderSource)
			: base(graphicsDevice, vertexShaderSource, fragmentShaderSource)
		{
			if (!HasUniform(ModelViewMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ModelViewMatrixUniformName));
			if (!HasUniform(ProjectionMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ProjectionMatrixUniformName));
			if (!HasUniform(TextureHasAlphaOnlyUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", TextureHasAlphaOnlyUniformName));
		}

		public CustomSpriteShader(GraphicsDevice graphicsDevice, TextReader vertexShaderSourceReader, TextReader fragmentShaderSourceReader)
			: base(graphicsDevice, vertexShaderSourceReader, fragmentShaderSourceReader)
		{
			if (!HasUniform(ModelViewMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ModelViewMatrixUniformName));
			if (!HasUniform(ProjectionMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ProjectionMatrixUniformName));
			if (!HasUniform(TextureHasAlphaOnlyUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", TextureHasAlphaOnlyUniformName));
		}
	}
}
