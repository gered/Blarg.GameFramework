using System;
using System.IO;

namespace Blarg.GameFramework.Graphics.CustomShaders
{
	public class CustomVertexSkinningShader : VertexSkinningShader
	{
		public CustomVertexSkinningShader(GraphicsDevice graphicsDevice, string vertexShaderSource, string fragmentShaderSource)
			: base(graphicsDevice, vertexShaderSource, fragmentShaderSource)
		{
			if (!HasUniform(ModelViewMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ModelViewMatrixUniformName));
			if (!HasUniform(ProjectionMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ProjectionMatrixUniformName));
			if (!HasUniform(JointPositionsUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", JointPositionsUniformName));
			if (!HasUniform(JointRotationsUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", JointRotationsUniformName));
		}

		public CustomVertexSkinningShader(GraphicsDevice graphicsDevice, TextReader vertexShaderSourceReader, TextReader fragmentShaderSourceReader)
			: base(graphicsDevice, vertexShaderSourceReader, fragmentShaderSourceReader)
		{
			if (!HasUniform(ModelViewMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ModelViewMatrixUniformName));
			if (!HasUniform(ProjectionMatrixUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", ProjectionMatrixUniformName));
			if (!HasUniform(JointPositionsUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", JointPositionsUniformName));
			if (!HasUniform(JointRotationsUniformName))
				throw new InvalidOperationException(String.Format("Missing \"{0}\" uniform.", JointRotationsUniformName));
		}
	}
}
