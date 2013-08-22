using System;
using System.IO;

namespace Blarg.GameFramework.Graphics
{
	public abstract class VertexSkinningShader : StandardShader
	{
		protected string JointPositionsUniformName;
		protected string JointRotationsUniformName;

		protected VertexSkinningShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			Initialize();
		}

		protected VertexSkinningShader(GraphicsDevice graphicsDevice, string vertexShaderSource, string fragmentShaderSource)
			: base(graphicsDevice, vertexShaderSource, fragmentShaderSource)
		{
			Initialize();
		}

		protected VertexSkinningShader(GraphicsDevice graphicsDevice, TextReader vertexShaderSourceReader, TextReader fragmentShaderSourceReader)
			: base(graphicsDevice, vertexShaderSourceReader, fragmentShaderSourceReader)
		{
			Initialize();
		}

		private void Initialize()
		{
			JointPositionsUniformName = "u_jointPositions";
			JointRotationsUniformName = "u_jointRotations";
		}

		public void SetJointPositions(Vector3[] positions)
		{
			if (!IsReadyForUse)
				throw new InvalidOperationException();
			SetUniform(JointPositionsUniformName, positions);
		}

		public void SetJointRotations(Quaternion[] rotations)
		{
			if (!IsReadyForUse)
				throw new InvalidOperationException();
			SetUniform(JointRotationsUniformName, rotations);
		}
	}
}
