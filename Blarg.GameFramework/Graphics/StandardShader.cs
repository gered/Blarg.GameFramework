using System;
using System.IO;

namespace Blarg.GameFramework.Graphics
{
	public abstract class StandardShader : Shader
	{
		protected string ModelViewMatrixUniformName;
		protected string ProjectionMatrixUniformName;

		protected StandardShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			Initialize();
		}

		protected StandardShader(GraphicsDevice graphicsDevice, string vertexShaderSource, string fragmentShaderSource)
			: base(graphicsDevice, vertexShaderSource, fragmentShaderSource)
		{
			Initialize();
		}

		protected StandardShader(GraphicsDevice graphicsDevice, TextReader vertexShaderSourceReader, TextReader fragmentShaderSourceReader)
			: base(graphicsDevice, vertexShaderSourceReader, fragmentShaderSourceReader)
		{
			Initialize();
		}

		private void Initialize()
		{
			ModelViewMatrixUniformName = "u_modelViewMatrix";
			ProjectionMatrixUniformName = "u_projectionMatrix";
		}

		public void SetModelViewMatrix(Matrix4x4 m)
		{
			SetModelViewMatrix(ref m);
		}

		public void SetModelViewMatrix(ref Matrix4x4 m)
		{
			if (!IsReadyForUse)
				throw new InvalidOperationException();
			SetUniform(ModelViewMatrixUniformName, ref m);
		}

		public void SetProjectionMatrix(Matrix4x4 m)
		{
			SetProjectionMatrix(ref m);
		}

		public void SetProjectionMatrix(ref Matrix4x4 m)
		{
			if (!IsReadyForUse)
				throw new InvalidOperationException();
			SetUniform(ProjectionMatrixUniformName, ref m);
		}
	}
}
