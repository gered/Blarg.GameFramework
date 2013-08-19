using System;

namespace Blarg.GameFramework.Graphics
{
	public class GeometryDebugRenderer
	{
		private const int DefaultVerticesAmount = 4096;

		private VertexBuffer _vertices;
		private RenderState _renderState;
		private Color _color1;
		private Color _color2;
		private bool _hasBegunRendering;

		public GraphicsDevice GraphicsDevice { get; private set; }

		public GeometryDebugRenderer(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			GraphicsDevice = graphicsDevice;

			_vertices = new VertexBuffer(GraphicsDevice, VertexAttributeDeclarations.ColorPosition3D, DefaultVerticesAmount, BufferObjectUsage.Stream);

			_color1 = new Color(1.0f, 1.0f, 0.0f);
			_color2 = new Color(1.0f, 0.0f, 0.0f);

			_renderState = (RenderState)RenderState.Default.Clone();
			_renderState.LineWidth = 2.0f;

			_hasBegunRendering = false;
		}

		#region Begin/End

		public void Begin(bool depthTesting = true)
		{
			if (_hasBegunRendering)
				throw new InvalidOperationException();

			_vertices.MoveToStart();
			_renderState.DepthTesting = depthTesting;

			_hasBegunRendering = true;
		}

		public void End()
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			if (_vertices.CurrentPosition == 0)
			{
				// nothing to render!
				_hasBegunRendering = false;
				return;
			}

			int numPointsToRender = _vertices.CurrentPosition;
			int numLinesToRender = numPointsToRender / 2;

			var shader = GraphicsDevice.DebugShader;

			var modelView = GraphicsDevice.ViewContext.ModelViewMatrix;
			var projection = GraphicsDevice.ViewContext.ProjectionMatrix;

			GraphicsDevice.BindShader(shader);
			shader.SetModelViewMatrix(ref modelView);
			shader.SetProjectionMatrix(ref projection);

			_renderState.Apply();

			GraphicsDevice.BindVertexBuffer(_vertices);
			GraphicsDevice.RenderLines(0, numLinesToRender);
			GraphicsDevice.RenderPoints(0, numPointsToRender);
			GraphicsDevice.UnbindVertexBuffer();

			GraphicsDevice.UnbindShader();

			_hasBegunRendering = false;
		}

		#endregion

		#region Primitive/Shape Rendering

		public void Render(BoundingBox box)
		{
			Render(ref box, ref _color1);
		}

		public void Render(ref BoundingBox box)
		{
			Render(ref box, ref _color1);
		}

		public void Render(BoundingBox box, Color color)
		{
			Render(ref box, ref color);
		}

		public void Render(ref BoundingBox box, ref Color color)
		{
			const int NumVerticesForBox = 24;

			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			EnsureSpaceFor(NumVerticesForBox);

			int i = _vertices.CurrentPosition;

			// removed lines which are duplicated by more then one face
			// left and right faces don't need to be drawn at all (entirely duplicated lines)

			// top
			_vertices.SetPosition3D(i++, box.Min.X, box.Max.Y, box.Min.Z);
			_vertices.SetPosition3D(i++, box.Max.X, box.Max.Y, box.Min.Z);

			_vertices.SetPosition3D(i++, box.Max.X, box.Max.Y, box.Min.Z);
			_vertices.SetPosition3D(i++, box.Max.X, box.Max.Y, box.Max.Z);

			_vertices.SetPosition3D(i++, box.Max.X, box.Max.Y, box.Max.Z);
			_vertices.SetPosition3D(i++, box.Min.X, box.Max.Y, box.Max.Z);

			_vertices.SetPosition3D(i++, box.Min.X, box.Max.Y, box.Max.Z);
			_vertices.SetPosition3D(i++, box.Min.X, box.Max.Y, box.Min.Z);

			// back
			_vertices.SetPosition3D(i++, box.Min.X, box.Min.Y, box.Min.Z);
			_vertices.SetPosition3D(i++, box.Max.X, box.Min.Y, box.Min.Z);

			_vertices.SetPosition3D(i++, box.Max.X, box.Min.Y, box.Min.Z);
			_vertices.SetPosition3D(i++, box.Max.X, box.Max.Y, box.Min.Z);

			_vertices.SetPosition3D(i++, box.Min.X, box.Max.Y, box.Min.Z);
			_vertices.SetPosition3D(i++, box.Min.X, box.Min.Y, box.Min.Z);

			// front
			_vertices.SetPosition3D(i++, box.Min.X, box.Min.Y, box.Max.Z);
			_vertices.SetPosition3D(i++, box.Max.X, box.Min.Y, box.Max.Z);

			_vertices.SetPosition3D(i++, box.Max.X, box.Min.Y, box.Max.Z);
			_vertices.SetPosition3D(i++, box.Max.X, box.Max.Y, box.Max.Z);

			_vertices.SetPosition3D(i++, box.Min.X, box.Max.Y, box.Max.Z);
			_vertices.SetPosition3D(i++, box.Min.X, box.Min.Y, box.Max.Z);

			// bottom
			_vertices.SetPosition3D(i++, box.Max.X, box.Min.Y, box.Min.Z);
			_vertices.SetPosition3D(i++, box.Max.X, box.Min.Y, box.Max.Z);

			_vertices.SetPosition3D(i++, box.Min.X, box.Min.Y, box.Max.Z);
			_vertices.SetPosition3D(i++, box.Min.X, box.Min.Y, box.Min.Z);

			// fill in all the colours
			for (int j = _vertices.CurrentPosition; j < i; ++j)
				_vertices.SetColor(j, ref color);

			_vertices.Move(NumVerticesForBox);
		}

		public void Render(Point3 boxMin, Point3 boxMax)
		{
			Render(ref boxMin, ref boxMax, ref _color1);
		}

		public void Render(ref Point3 boxMin, ref Point3 boxMax)
		{
			Render(ref boxMin, ref boxMax, ref _color1);
		}

		public void Render(Point3 boxMin, Point3 boxMax, Color color)
		{
			Render(ref boxMin, ref boxMax, ref color);
		}

		public void Render(ref Point3 boxMin, ref Point3 boxMax, ref Color color)
		{
			var box = new BoundingBox();
			box.Min.Set(ref boxMin);
			box.Max.Set(ref boxMax);
			Render(ref box, ref color);
		}

		public void Render(BoundingSphere sphere)
		{
			Render(ref sphere, ref _color1);
		}

		public void Render(ref BoundingSphere sphere)
		{
			Render(ref sphere, ref _color1);
		}

		public void Render(BoundingSphere sphere, Color color)
		{
			Render(ref sphere, ref color);
		}

		public void Render(ref BoundingSphere sphere, ref Color color)
		{
			const int NumVerticesForSphere = 615;

			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			EnsureSpaceFor(NumVerticesForSphere);

			int p = _vertices.CurrentPosition;

			float ax, ay, az;
			float bx, by, bz;
			float cx = 0.0f, cy = 0.0f, cz = 0.0f;
			float dx = 0.0f, dy = 0.0f, dz = 0.0f;
			float theta1, theta2, theta3;

			int n = 12;
			for (int j = 0; j < n / 2; ++j)
			{
				theta1 = j * MathConstants.Pi * 2 / n - MathConstants.Pi / 2;
				theta2 = (j + 1) * MathConstants.Pi * 2 / n - MathConstants.Pi / 2;

				for (int i = 0; i <= n; ++i)
				{
					theta3 = i * MathConstants.Pi * 2 / n;
					ax = sphere.Center.X + sphere.Radius * (float)Math.Cos(theta2) * (float)Math.Cos(theta3);
					ay = sphere.Center.Y + sphere.Radius * (float)Math.Sin(theta2);
					az = sphere.Center.Z + sphere.Radius * (float)Math.Cos(theta2) * (float)Math.Sin(theta3);

					bx = sphere.Center.X + sphere.Radius * (float)Math.Cos(theta1) * (float)Math.Cos(theta3);
					by = sphere.Center.Y + sphere.Radius * (float)Math.Sin(theta1);
					bz = sphere.Center.Z + sphere.Radius * (float)Math.Cos(theta1) * (float)Math.Sin(theta3);

					if (j > 0 || i > 0)
					{
						_vertices.SetPosition3D(p++, ax, ay, az);
						_vertices.SetPosition3D(p++, bx, by, bz);

						_vertices.SetPosition3D(p++, bx, by, bz);
						_vertices.SetPosition3D(p++, dx, dy, dz);

						_vertices.SetPosition3D(p++, dx, dy, dz);
						_vertices.SetPosition3D(p++, cx, cy, cz);

						_vertices.SetPosition3D(p++, cx, cy, cz);
						_vertices.SetPosition3D(p++, ax, ay, az);
					}

					cx = ax;
					cy = ay;
					cz = az;
					dx = bx;
					dy = by;
					dz = bz;
				}
			}

			// fill in all the colours
			for (int i = _vertices.CurrentPosition; i < p; ++i)
				_vertices.SetColor(i, ref color);

			_vertices.Move(NumVerticesForSphere);
		}

		public void Render(Ray ray, float length)
		{
			Render(ref ray, length, ref _color1, ref _color2);
		}

		public void Render(ref Ray ray, float length)
		{
			Render(ref ray, length, ref _color1, ref _color2);
		}

		public void Render(Ray ray, float length, Color originColor, Color endColor)
		{
			Render(ref ray, length, ref originColor, ref endColor);
		}

		public void Render(ref Ray ray, float length, ref Color originColor, ref Color endColor)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			EnsureSpaceFor(2);

			Vector3 endPoint = ray.GetPositionAt(length);

			_vertices.SetCurrentPosition3D(ref ray.Position);
			_vertices.SetCurrentColor(ref originColor);
			_vertices.MoveNext();

			_vertices.SetCurrentPosition3D(ref endPoint);
			_vertices.SetCurrentColor(ref endColor);
			_vertices.MoveNext();
		}

		public void Render(LineSegment line)
		{
			Render(ref line, ref _color1);
		}

		public void Render(ref LineSegment line)
		{
			Render(ref line, ref _color1);
		}

		public void Render(LineSegment line, Color color)
		{
			Render(ref line, ref color);
		}

		public void Render(ref LineSegment line, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			EnsureSpaceFor(2);

			_vertices.SetCurrentPosition3D(ref line.A);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();

			_vertices.SetCurrentPosition3D(ref line.B);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
		}

		public void Render(Vector3 a, Vector3 b)
		{
			Render(ref a, ref b, ref _color1);
		}

		public void Render(ref Vector3 a, ref Vector3 b)
		{
			Render(ref a, ref b, ref _color1);
		}

		public void Render(Vector3 a, Vector3 b, Color color)
		{
			Render(ref a, ref b, ref color);
		}

		public void Render(ref Vector3 a, ref Vector3 b, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			EnsureSpaceFor(2);

			_vertices.SetCurrentPosition3D(ref a);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();

			_vertices.SetCurrentPosition3D(ref b);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
		}

		public void Render(Vector3 a, Vector3 b, Vector3 c)
		{
			Render(ref a, ref b, ref c, ref _color1);
		}

		public void Render(ref Vector3 a, ref Vector3 b, ref Vector3 c)
		{
			Render(ref a, ref b, ref c, ref _color1);
		}

		public void Render(Vector3 a, Vector3 b, Vector3 c, Color color)
		{
			Render(ref a, ref b, ref c, ref color);
		}

		public void Render(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			EnsureSpaceFor(6);

			// A -> B
			_vertices.SetCurrentPosition3D(ref a);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
			_vertices.SetCurrentPosition3D(ref b);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();

			// A -> C
			_vertices.SetCurrentPosition3D(ref a);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
			_vertices.SetCurrentPosition3D(ref c);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();

			// B -> C
			_vertices.SetCurrentPosition3D(ref b);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
			_vertices.SetCurrentPosition3D(ref c);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
		}

		public void Render(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			Render(ref a, ref b, ref c, ref d, ref _color1);
		}

		public void Render(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d)
		{
			Render(ref a, ref b, ref c, ref d, ref _color1);
		}

		public void Render(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color)
		{
			Render(ref a, ref b, ref c, ref d, ref color);
		}

		public void Render(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, ref Color color)
		{
			if (!_hasBegunRendering)
				throw new InvalidOperationException();

			EnsureSpaceFor(8);

			// A -> B
			_vertices.SetCurrentPosition3D(ref a);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
			_vertices.SetCurrentPosition3D(ref b);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();

			// A -> C
			_vertices.SetCurrentPosition3D(ref a);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
			_vertices.SetCurrentPosition3D(ref c);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();

			// C -> D
			_vertices.SetCurrentPosition3D(ref c);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
			_vertices.SetCurrentPosition3D(ref d);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();

			// B -> D
			_vertices.SetCurrentPosition3D(ref b);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
			_vertices.SetCurrentPosition3D(ref d);
			_vertices.SetCurrentColor(ref color);
			_vertices.MoveNext();
		}

		#endregion

		private void EnsureSpaceFor(int numVertices)
		{
			if (_vertices.RemainingElements >= numVertices)
				return;

			int numVerticesNeeded = numVertices - _vertices.RemainingElements;
			_vertices.Extend(numVerticesNeeded);
		}
	}
}
