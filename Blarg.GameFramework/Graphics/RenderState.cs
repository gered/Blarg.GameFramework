using System;
using PortableGL;

namespace Blarg.GameFramework.Graphics
{
	public enum CullMode
	{
		Back,
		Front,
		FrontAndBack
	}

	public enum DepthFunc
	{
		Never,
		Less,
		Equal,
		LessOrEqual,
		Greater,
		NotEqual,
		GreaterOrEqual,
		Always
	}

	public class RenderState
	{
		public static readonly RenderState Default;
		public static readonly RenderState NoDepthTesting;
		public static readonly RenderState NoCulling;

		static RenderState()
		{
			Default = new RenderState();

			NoDepthTesting = new RenderState();
			NoDepthTesting.DepthTesting = false;

			NoCulling = new RenderState();
			NoCulling.FaceCulling = false;
		}

		public bool DepthTesting { get; set; }
		public DepthFunc DepthFunc { get; set; }
		public bool FaceCulling { get; set; }
		public CullMode FaceCullingMode { get; set; }
		public float LineWidth { get; set; }

		public RenderState()
		{
			Init();
		}

		public void Apply()
		{
			if (DepthTesting)
			{
				Platform.GL.glEnable(GL20.GL_DEPTH_TEST);

				int depthFunc = GL20.GL_LESS;
				switch (DepthFunc)
				{
					case DepthFunc.Never:          depthFunc = GL20.GL_NEVER; break;
					case DepthFunc.Less:           depthFunc = GL20.GL_LESS; break;
					case DepthFunc.Equal:          depthFunc = GL20.GL_EQUAL; break;
					case DepthFunc.LessOrEqual:    depthFunc = GL20.GL_LEQUAL; break;
					case DepthFunc.Greater:        depthFunc = GL20.GL_GREATER; break;;
					case DepthFunc.NotEqual:       depthFunc = GL20.GL_NOTEQUAL; break;
					case DepthFunc.GreaterOrEqual: depthFunc = GL20.GL_GEQUAL; break;
					case DepthFunc.Always:         depthFunc = GL20.GL_ALWAYS; break;
				}
				Platform.GL.glDepthFunc(depthFunc);
			}
			else
				Platform.GL.glDisable(GL20.GL_DEPTH_TEST);

			if (FaceCulling)
			{
				Platform.GL.glEnable(GL20.GL_CULL_FACE);
				if (FaceCullingMode == CullMode.FrontAndBack)
					Platform.GL.glCullFace(GL20.GL_FRONT_AND_BACK);
				else if (FaceCullingMode == CullMode.Front)
					Platform.GL.glCullFace(GL20.GL_FRONT);
				else
					Platform.GL.glCullFace(GL20.GL_BACK);
			}
			else
				Platform.GL.glDisable(GL20.GL_CULL_FACE);

			Platform.GL.glLineWidth(LineWidth);
		}

		private void Init()
		{
			DepthTesting = true;
			DepthFunc = DepthFunc.Less;
			FaceCulling = true;
			FaceCullingMode = CullMode.Back;
			LineWidth = 1.0f;
		}

		public RenderState Clone()
		{
			var clone = new RenderState();
			clone.DepthFunc = DepthFunc;
			clone.DepthTesting = DepthTesting;
			clone.FaceCulling = FaceCulling;
			clone.FaceCullingMode = FaceCullingMode;
			clone.LineWidth = LineWidth;
			return clone;
		}

	}
}
