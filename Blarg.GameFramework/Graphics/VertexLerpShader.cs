using System;
using System.IO;

namespace Blarg.GameFramework.Graphics
{
	public abstract class VertexLerpShader : StandardShader
	{
		protected string LerpUniformName { get; set; }

		protected VertexLerpShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			Initialize();
		}

		protected VertexLerpShader(GraphicsDevice graphicsDevice, string vertexShaderSource, string fragmentShaderSource)
			: base(graphicsDevice, vertexShaderSource, fragmentShaderSource)
		{
			Initialize();
		}

		protected VertexLerpShader(GraphicsDevice graphicsDevice, TextReader vertexShaderSourceReader, TextReader fragmentShaderSourceReader)
			: base(graphicsDevice, vertexShaderSourceReader, fragmentShaderSourceReader)
		{
			Initialize();
		}

		private void Initialize()
		{
			LerpUniformName = "u_lerp";
		}

		public void SetLerp(float t)
		{
			if (!IsReadyForUse)
				throw new InvalidOperationException();
			SetUniform(LerpUniformName, t);
		}
	}
}
