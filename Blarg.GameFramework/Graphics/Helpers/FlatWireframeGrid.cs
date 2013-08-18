using System;

namespace Blarg.GameFramework.Graphics.Helpers
{
	public class FlatWireframeGrid
	{
		VertexBuffer _horizontalPoints;
		VertexBuffer _verticalPoints;

		public int Width { get; private set; }
		public int Height { get; private set; }

		public GraphicsDevice GraphicsDevice { get; private set; }

		public FlatWireframeGrid(GraphicsDevice graphicsDevice, int width, int height)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");
			if (width < 1)
				throw new ArgumentOutOfRangeException("width");
			if (height < 1)
				throw new ArgumentOutOfRangeException("height");

			GraphicsDevice = graphicsDevice;
			Width = width;
			Height = height;

			_horizontalPoints = new VertexBuffer(GraphicsDevice, VertexAttributeDeclarations.ColorPosition3D, width * 2 + 2, BufferObjectUsage.Static);
			_verticalPoints = new VertexBuffer(GraphicsDevice, VertexAttributeDeclarations.ColorPosition3D, height * 2 + 2, BufferObjectUsage.Static);

			var gridColor = Color.White;

			for (int i = 0; i < height + 1; ++i)
			{
				_horizontalPoints.SetPosition3D((i * 2), -(width / 2.0f), 0.0f, i - (height / 2.0f));
				_horizontalPoints.SetColor((i * 2), ref gridColor);
				_horizontalPoints.SetPosition3D((i * 2) + 1, width / 2.0f, 0.0f, i - (height / 2.0f));
				_horizontalPoints.SetColor((i * 2) + 1, ref gridColor);
			}

			for (int i = 0; i < width + 1; ++i)
			{
				_verticalPoints.SetPosition3D((i * 2), i - (width / 2.0f), 0.0f, -(height / 2.0f));
				_verticalPoints.SetColor((i * 2), ref gridColor);
				_verticalPoints.SetPosition3D((i * 2) + 1, i - (width / 2.0f), 0.0f, height / 2.0f);
				_verticalPoints.SetColor((i * 2) + 1, ref gridColor);
			}
		}

		public void Render()
		{
			RenderState.Default.Apply();

			GraphicsDevice.BindVertexBuffer(_horizontalPoints);
			GraphicsDevice.RenderLines(0, _horizontalPoints.NumElements / 2);
			GraphicsDevice.UnbindVertexBuffer();

			GraphicsDevice.BindVertexBuffer(_verticalPoints);
			GraphicsDevice.RenderLines(0, _verticalPoints.NumElements / 2);
			GraphicsDevice.UnbindVertexBuffer();
		}
	}
}
