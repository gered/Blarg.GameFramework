using System;
using System.IO;

namespace Blarg.GameFramework.Graphics.CustomShaders
{
	public class CustomStandardShader : StandardShader
	{
		public CustomStandardShader(GraphicsDevice graphicsDevice, string vertexShaderSource, string fragmentShaderSource)
			: base(graphicsDevice, vertexShaderSource, fragmentShaderSource)
		{
			if (!HasUniform(ModelViewMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ModelViewMatrixUniformName));
			if (!HasUniform(ProjectionMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ProjectionMatrixUniformName));
		}

		public CustomStandardShader(GraphicsDevice graphicsDevice, TextReader vertexShaderSourceReader, TextReader fragmentShaderSourceReader)
			: base(graphicsDevice, vertexShaderSourceReader, fragmentShaderSourceReader)
		{
			if (!HasUniform(ModelViewMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ModelViewMatrixUniformName));
			if (!HasUniform(ProjectionMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ProjectionMatrixUniformName));
		}
	}
}
