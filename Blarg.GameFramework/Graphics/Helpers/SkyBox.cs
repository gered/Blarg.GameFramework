using System;

namespace Blarg.GameFramework.Graphics.Helpers
{
	public class SkyBox
	{
		Texture _top;
		Texture _bottom;
		Texture _left;
		Texture _right;
		Texture _front;
		Texture _back;
		VertexBuffer _vertices;

		RenderState _renderState;
		BlendState _blendState;

		public SkyBox(Texture top,
		              Texture bottom,
		              Texture left,
		              Texture right,
		              Texture front,
		              Texture back)
		{
			if (top == null || bottom == null || left == null || right == null || front == null || back == null)
				throw new ArgumentNullException();

			_top = top;
			_bottom = bottom;
			_left = left;
			_right = right;
			_front = front;
			_back = back;
			SetupRenderSettings();
			GenerateVertices();
		}

		public SkyBox(Texture top,
		              Texture side,
		              Texture bottom)
		{
			if (top == null || side == null || bottom == null)
				throw new ArgumentNullException();

			_top = top;
			_bottom = bottom;
			_left = side;
			_right = side;
			_front = side;
			_back = side;
			SetupRenderSettings();
			GenerateVertices();
		}

		private void SetupRenderSettings()
		{
			_renderState = new RenderState();
			_renderState.DepthTesting = false;
			_renderState.DepthWriting = false;

			_blendState = new BlendState();
		}

		private void GenerateVertices()
		{
			const float n = 10.0f;

			_vertices = new VertexBuffer(Framework.GraphicsDevice, 
			                             VertexAttributeDeclarations.TexturePosition3D, 
			                             36, 
			                             BufferObjectUsage.Static);

			// front
			_vertices.SetTexCoord(0, 1.0f, 1.0f);
			_vertices.SetTexCoord(1, 0.0f, 1.0f);
			_vertices.SetTexCoord(2, 1.0f, 0.0f);
			_vertices.SetTexCoord(3, 0.0f, 1.0f);
			_vertices.SetTexCoord(4, 0.0f, 0.0f);
			_vertices.SetTexCoord(5, 1.0f, 0.0f);
			_vertices.SetPosition3D(0, -n, -n, -n);
			_vertices.SetPosition3D(1,  n, -n, -n);
			_vertices.SetPosition3D(2, -n,  n, -n);
			_vertices.SetPosition3D(3,  n, -n, -n);
			_vertices.SetPosition3D(4,  n,  n, -n);
			_vertices.SetPosition3D(5, -n,  n, -n);

			// left
			_vertices.SetTexCoord(6,  1.0f, 1.0f);
			_vertices.SetTexCoord(7,  0.0f, 1.0f);
			_vertices.SetTexCoord(8,  1.0f, 0.0f);
			_vertices.SetTexCoord(9,  0.0f, 1.0f);
			_vertices.SetTexCoord(10, 0.0f, 0.0f);
			_vertices.SetTexCoord(11, 1.0f, 0.0f);
			_vertices.SetPosition3D(6,  -n, -n,  n);
			_vertices.SetPosition3D(7,  -n, -n, -n);
			_vertices.SetPosition3D(8,  -n,  n,  n);
			_vertices.SetPosition3D(9,  -n, -n, -n);
			_vertices.SetPosition3D(10, -n,  n, -n);
			_vertices.SetPosition3D(11, -n,  n,  n);

			// back
			_vertices.SetTexCoord(12, 1.0f, 1.0f);
			_vertices.SetTexCoord(13, 0.0f, 1.0f);
			_vertices.SetTexCoord(14, 1.0f, 0.0f);
			_vertices.SetTexCoord(15, 0.0f, 1.0f);
			_vertices.SetTexCoord(16, 0.0f, 0.0f);
			_vertices.SetTexCoord(17, 1.0f, 0.0f);
			_vertices.SetPosition3D(12,  n, -n,  n);
			_vertices.SetPosition3D(13, -n, -n,  n);
			_vertices.SetPosition3D(14,  n,  n,  n);
			_vertices.SetPosition3D(15, -n, -n,  n);
			_vertices.SetPosition3D(16, -n,  n,  n);
			_vertices.SetPosition3D(17,  n,  n,  n);

			// right
			_vertices.SetTexCoord(18, 1.0f, 1.0f);
			_vertices.SetTexCoord(19, 0.0f, 1.0f);
			_vertices.SetTexCoord(20, 1.0f, 0.0f);
			_vertices.SetTexCoord(21, 0.0f, 1.0f);
			_vertices.SetTexCoord(22, 0.0f, 0.0f);
			_vertices.SetTexCoord(23, 1.0f, 0.0f);
			_vertices.SetPosition3D(18,  n, -n, -n);
			_vertices.SetPosition3D(19,  n, -n,  n);
			_vertices.SetPosition3D(20,  n,  n, -n);
			_vertices.SetPosition3D(21,  n, -n,  n);
			_vertices.SetPosition3D(22,  n,  n,  n);
			_vertices.SetPosition3D(23,  n,  n, -n);

			// top
			_vertices.SetTexCoord(24, 0.0f, 1.0f);
			_vertices.SetTexCoord(25, 0.0f, 0.0f);
			_vertices.SetTexCoord(26, 1.0f, 1.0f);
			_vertices.SetTexCoord(27, 0.0f, 0.0f);
			_vertices.SetTexCoord(28, 1.0f, 0.0f);
			_vertices.SetTexCoord(29, 1.0f, 1.0f);
			_vertices.SetPosition3D(24,  n,  n,  n);
			_vertices.SetPosition3D(25, -n,  n,  n);
			_vertices.SetPosition3D(26,  n,  n, -n);
			_vertices.SetPosition3D(27, -n,  n,  n);
			_vertices.SetPosition3D(28, -n,  n, -n);
			_vertices.SetPosition3D(29,  n,  n, -n);

			// bottom
			_vertices.SetTexCoord(30, 0.0f, 1.0f);
			_vertices.SetTexCoord(31, 0.0f, 0.0f);
			_vertices.SetTexCoord(32, 1.0f, 1.0f);
			_vertices.SetTexCoord(33, 0.0f, 0.0f);
			_vertices.SetTexCoord(34, 1.0f, 0.0f);
			_vertices.SetTexCoord(35, 1.0f, 1.0f);
			_vertices.SetPosition3D(30, -n, -n,  n);
			_vertices.SetPosition3D(31,  n, -n,  n);
			_vertices.SetPosition3D(32, -n, -n, -n);
			_vertices.SetPosition3D(33,  n, -n,  n);
			_vertices.SetPosition3D(34,  n, -n, -n);
			_vertices.SetPosition3D(35, -n, -n, -n);
		}

		public void Render()
		{
			var graphicsDevice = Framework.GraphicsDevice;

			_renderState.Apply(graphicsDevice);
			_blendState.Apply(graphicsDevice);

			var shader = graphicsDevice.SimpleTextureShader;
			graphicsDevice.BindShader(shader);
			graphicsDevice.BindVertexBuffer(_vertices);

			var modelViewMatrix = graphicsDevice.ViewContext.ModelViewMatrix;
			// position at the origin, but keep current rotation
			modelViewMatrix.M14 = 0.0f;
			modelViewMatrix.M24 = 0.0f;
			modelViewMatrix.M34 = 0.0f;
			shader.SetModelViewMatrix(ref modelViewMatrix);

			shader.SetProjectionMatrix(graphicsDevice.ViewContext.ProjectionMatrix);

			// front
			graphicsDevice.BindTexture(_front);
			graphicsDevice.RenderTriangles(0, 2);

			// left
			graphicsDevice.BindTexture(_left);
			graphicsDevice.RenderTriangles(6, 2);

			// back
			graphicsDevice.BindTexture(_back);
			graphicsDevice.RenderTriangles(12, 2);

			// right
			graphicsDevice.BindTexture(_right);
			graphicsDevice.RenderTriangles(18, 2);

			// top
			graphicsDevice.BindTexture(_top);
			graphicsDevice.RenderTriangles(24, 2);

			// bottom
			graphicsDevice.BindTexture(_bottom);
			graphicsDevice.RenderTriangles(30, 2);

			graphicsDevice.UnbindShader();
			graphicsDevice.UnbindVertexBuffer();
		}
	}
}

