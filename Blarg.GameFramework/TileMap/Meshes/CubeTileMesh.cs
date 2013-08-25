using System;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Graphics.Helpers;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.TileMap.Meshes
{
	public class CubeTileMesh : TileMesh
	{
		BoundingBox _bounds;
		VertexBuffer _vertices;
		Vector3[] _collisionVertices;

		public readonly byte Faces;
		public readonly int TopFaceVertexOffset;
		public readonly int BottomFaceVertexOffset;
		public readonly int FrontFaceVertexOffset;
		public readonly int BackFaceVertexOffset;
		public readonly int LeftFaceVertexOffset;
		public readonly int RightFaceVertexOffset;

		public override BoundingBox Bounds
		{
			get { return _bounds; }
		}

		public override VertexBuffer Vertices
		{
			get { return _vertices; }
		}

		public override Vector3[] CollisionVertices
		{
			get { return _collisionVertices; }
		}

		public CubeTileMesh(
			TextureRegion topTexture,
			TextureRegion bottomTexture,
			TextureRegion frontTexture,
			TextureRegion backTexture,
			TextureRegion leftTexture,
			TextureRegion rightTexture,
			byte faces,
			byte opaqueSides,
			byte lightValue,
			bool alpha,
			float translucency,
			Color color
		)
			: base(opaqueSides, alpha, translucency, lightValue, color)
		{
			if (faces == 0)
				throw new ArgumentException("faces");

			Faces = faces;

			int numVertices = 0;

			if (HasFace(SIDE_TOP))
			{
				TopFaceVertexOffset = numVertices;
				numVertices += CUBE_VERTICES_PER_FACE;
			}
			else
				TopFaceVertexOffset = 0;

			if (HasFace(SIDE_BOTTOM))
			{
				BottomFaceVertexOffset = numVertices;
				numVertices += CUBE_VERTICES_PER_FACE;
			}
			else
				BottomFaceVertexOffset = 0;

			if (HasFace(SIDE_FRONT))
			{
				FrontFaceVertexOffset = numVertices;
				numVertices += CUBE_VERTICES_PER_FACE;
			}
			else
				FrontFaceVertexOffset = 0;

			if (HasFace(SIDE_BACK))
			{
				BackFaceVertexOffset = numVertices;
				numVertices += CUBE_VERTICES_PER_FACE;
			}
			else
				BackFaceVertexOffset = 0;

			if (HasFace(SIDE_LEFT))
			{
				LeftFaceVertexOffset = numVertices;
				numVertices += CUBE_VERTICES_PER_FACE;
			}
			else
				LeftFaceVertexOffset = 0;

			if (HasFace(SIDE_RIGHT))
			{
				RightFaceVertexOffset = numVertices;
				numVertices += CUBE_VERTICES_PER_FACE;
			}
			else
				RightFaceVertexOffset = 0;

			SetupFaceVertices(numVertices, topTexture, bottomTexture, frontTexture, backTexture, leftTexture, rightTexture);
			SetupCollisionVertices();
			_bounds = UNIT_BOUNDS;
		}

		public bool HasFace(byte side)
		{
			return Faces.IsBitSet(side);
		}

		private void SetupFaceVertices(
			int numVertices,
			TextureRegion topTexture,
			TextureRegion bottomTexture,
			TextureRegion frontTexture,
			TextureRegion backTexture,
			TextureRegion leftTexture,
			TextureRegion rightTexture
			)
		{
			int pos = 0;
			Vector3 a = new Vector3(-0.5f, -0.5f, -0.5f);
			Vector3 b = new Vector3(0.5f, 0.5f, 0.5f);

			_vertices = new VertexBuffer(VertexAttributeDeclarations.TextureColorNormalPosition3D, numVertices, BufferObjectUsage.Static);

			if (HasFace(SIDE_TOP))
			{
				pos = TopFaceVertexOffset;

				_vertices.SetPosition3D(pos, a.X, b.Y, b.Z);
				_vertices.SetColor(pos, Color);
				_vertices.SetNormal(pos, Vector3.Up);
				_vertices.SetTexCoord(pos, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 1.0f), topTexture));

				_vertices.SetPosition3D(pos + 1, b.X, b.Y, b.Z);
				_vertices.SetColor(pos + 1, Color);
				_vertices.SetNormal(pos + 1, Vector3.Up);
				_vertices.SetTexCoord(pos + 1, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), topTexture));

				_vertices.SetPosition3D(pos + 2, a.X, b.Y, a.Z);
				_vertices.SetColor(pos + 2, Color);
				_vertices.SetNormal(pos + 2, Vector3.Up);
				_vertices.SetTexCoord(pos + 2, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), topTexture));

				_vertices.SetPosition3D(pos + 3, b.X, b.Y, b.Z);
				_vertices.SetColor(pos + 3, Color);
				_vertices.SetNormal(pos + 3, Vector3.Up);
				_vertices.SetTexCoord(pos + 3, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), topTexture));

				_vertices.SetPosition3D(pos + 4, b.X, b.Y, a.Z);
				_vertices.SetColor(pos + 4, Color);
				_vertices.SetNormal(pos + 4, Vector3.Up);
				_vertices.SetTexCoord(pos + 4, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 0.0f), topTexture));

				_vertices.SetPosition3D(pos + 5, a.X, b.Y, a.Z);
				_vertices.SetColor(pos + 5, Color);
				_vertices.SetNormal(pos + 5, Vector3.Up);
				_vertices.SetTexCoord(pos + 5, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), topTexture));
			}

			if (HasFace(SIDE_BOTTOM))
			{
				pos = BottomFaceVertexOffset;

				_vertices.SetPosition3D(pos, b.X, a.Y, b.Z);
				_vertices.SetColor(pos, Color);
				_vertices.SetNormal(pos, Vector3.Down);
				_vertices.SetTexCoord(pos, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 1.0f), bottomTexture));

				_vertices.SetPosition3D(pos + 1, a.X, a.Y, b.Z);
				_vertices.SetColor(pos + 1, Color);
				_vertices.SetNormal(pos + 1, Vector3.Down);
				_vertices.SetTexCoord(pos + 1, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), bottomTexture));

				_vertices.SetPosition3D(pos + 2, b.X, a.Y, a.Z);
				_vertices.SetColor(pos + 2, Color);
				_vertices.SetNormal(pos + 2, Vector3.Down);
				_vertices.SetTexCoord(pos + 2, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), bottomTexture));

				_vertices.SetPosition3D(pos + 3, a.X, a.Y, b.Z);
				_vertices.SetColor(pos + 3, Color);
				_vertices.SetNormal(pos + 3, Vector3.Down);
				_vertices.SetTexCoord(pos + 3, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), bottomTexture));

				_vertices.SetPosition3D(pos + 4, a.X, a.Y, a.Z);
				_vertices.SetColor(pos + 4, Color);
				_vertices.SetNormal(pos + 4, Vector3.Down);
				_vertices.SetTexCoord(pos + 4, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 0.0f), bottomTexture));

				_vertices.SetPosition3D(pos + 5, b.X, a.Y, a.Z);
				_vertices.SetColor(pos + 5, Color);
				_vertices.SetNormal(pos + 5, Vector3.Down);
				_vertices.SetTexCoord(pos + 5, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), bottomTexture));
			}

			if (HasFace(SIDE_FRONT))
			{
				pos = FrontFaceVertexOffset;

				_vertices.SetPosition3D(pos, b.X, a.Y, a.Z);
				_vertices.SetColor(pos, Color);
				_vertices.SetNormal(pos, Vector3.Forward);
				_vertices.SetTexCoord(pos, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 1.0f), frontTexture));

				_vertices.SetPosition3D(pos + 1, a.X, a.Y, a.Z);
				_vertices.SetColor(pos + 1, Color);
				_vertices.SetNormal(pos + 1, Vector3.Forward);
				_vertices.SetTexCoord(pos + 1, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), frontTexture));

				_vertices.SetPosition3D(pos + 2, b.X, b.Y, a.Z);
				_vertices.SetColor(pos + 2, Color);
				_vertices.SetNormal(pos + 2, Vector3.Forward);
				_vertices.SetTexCoord(pos + 2, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), frontTexture));

				_vertices.SetPosition3D(pos + 3, a.X, a.Y, a.Z);
				_vertices.SetColor(pos + 3, Color);
				_vertices.SetNormal(pos + 3, Vector3.Forward);
				_vertices.SetTexCoord(pos + 3, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), frontTexture));

				_vertices.SetPosition3D(pos + 4, a.X, b.Y, a.Z);
				_vertices.SetColor(pos + 4, Color);
				_vertices.SetNormal(pos + 4, Vector3.Forward);
				_vertices.SetTexCoord(pos + 4, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 0.0f), frontTexture));

				_vertices.SetPosition3D(pos + 5, b.X, b.Y, a.Z);
				_vertices.SetColor(pos + 5, Color);
				_vertices.SetNormal(pos + 5, Vector3.Forward);
				_vertices.SetTexCoord(pos + 5, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), frontTexture));
			}

			if (HasFace(SIDE_BACK))
			{
				pos = BackFaceVertexOffset;

				_vertices.SetPosition3D(pos, a.X, a.Y, b.Z);
				_vertices.SetColor(pos, Color);
				_vertices.SetNormal(pos, Vector3.Backward);
				_vertices.SetTexCoord(pos, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 1.0f), backTexture));

				_vertices.SetPosition3D(pos + 1, b.X, a.Y, b.Z);
				_vertices.SetColor(pos + 1, Color);
				_vertices.SetNormal(pos + 1, Vector3.Backward);
				_vertices.SetTexCoord(pos + 1, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), backTexture));

				_vertices.SetPosition3D(pos + 2, a.X, b.Y, b.Z);
				_vertices.SetColor(pos + 2, Color);
				_vertices.SetNormal(pos + 2, Vector3.Backward);
				_vertices.SetTexCoord(pos + 2, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), backTexture));

				_vertices.SetPosition3D(pos + 3, b.X, a.Y, b.Z);
				_vertices.SetColor(pos + 3, Color);
				_vertices.SetNormal(pos + 3, Vector3.Backward);
				_vertices.SetTexCoord(pos + 3, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), backTexture));

				_vertices.SetPosition3D(pos + 4, b.X, b.Y, b.Z);
				_vertices.SetColor(pos + 4, Color);
				_vertices.SetNormal(pos + 4, Vector3.Backward);
				_vertices.SetTexCoord(pos + 4, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 0.0f), backTexture));

				_vertices.SetPosition3D(pos + 5, a.X, b.Y, b.Z);
				_vertices.SetColor(pos + 5, Color);
				_vertices.SetNormal(pos + 5, Vector3.Backward);
				_vertices.SetTexCoord(pos + 5, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), backTexture));
			}

			if (HasFace(SIDE_LEFT))
			{
				pos = LeftFaceVertexOffset;

				_vertices.SetPosition3D(pos, a.X, a.Y, a.Z);
				_vertices.SetColor(pos, Color);
				_vertices.SetNormal(pos, Vector3.Left);
				_vertices.SetTexCoord(pos, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 1.0f), leftTexture));

				_vertices.SetPosition3D(pos + 1, a.X, a.Y, b.Z);
				_vertices.SetColor(pos + 1, Color);
				_vertices.SetNormal(pos + 1, Vector3.Left);
				_vertices.SetTexCoord(pos + 1, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), leftTexture));

				_vertices.SetPosition3D(pos + 2, a.X, b.Y, a.Z);
				_vertices.SetColor(pos + 2, Color);
				_vertices.SetNormal(pos + 2, Vector3.Left);
				_vertices.SetTexCoord(pos + 2, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), leftTexture));

				_vertices.SetPosition3D(pos + 3, a.X, a.Y, b.Z);
				_vertices.SetColor(pos + 3, Color);
				_vertices.SetNormal(pos + 3, Vector3.Left);
				_vertices.SetTexCoord(pos + 3, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), leftTexture));

				_vertices.SetPosition3D(pos + 4, a.X, b.Y, b.Z);
				_vertices.SetColor(pos + 4, Color);
				_vertices.SetNormal(pos + 4, Vector3.Left);
				_vertices.SetTexCoord(pos + 4, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 0.0f), leftTexture));

				_vertices.SetPosition3D(pos + 5, a.X, b.Y, a.Z);
				_vertices.SetColor(pos + 5, Color);
				_vertices.SetNormal(pos + 5, Vector3.Left);
				_vertices.SetTexCoord(pos + 5, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), leftTexture));
			}

			if (HasFace(SIDE_RIGHT))
			{
				pos = RightFaceVertexOffset;

				_vertices.SetPosition3D(pos, b.X, a.Y, b.Z);
				_vertices.SetColor(pos, Color);
				_vertices.SetNormal(pos, Vector3.Right);
				_vertices.SetTexCoord(pos, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 1.0f), rightTexture));

				_vertices.SetPosition3D(pos + 1, b.X, a.Y, a.Z);
				_vertices.SetColor(pos + 1, Color);
				_vertices.SetNormal(pos + 1, Vector3.Right);
				_vertices.SetTexCoord(pos + 1, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), rightTexture));

				_vertices.SetPosition3D(pos + 2, b.X, b.Y, b.Z);
				_vertices.SetColor(pos + 2, Color);
				_vertices.SetNormal(pos + 2, Vector3.Right);
				_vertices.SetTexCoord(pos + 2, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), rightTexture));

				_vertices.SetPosition3D(pos + 3, b.X, a.Y, a.Z);
				_vertices.SetColor(pos + 3, Color);
				_vertices.SetNormal(pos + 3, Vector3.Right);
				_vertices.SetTexCoord(pos + 3, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 1.0f), rightTexture));

				_vertices.SetPosition3D(pos + 4, b.X, b.Y, a.Z);
				_vertices.SetColor(pos + 4, Color);
				_vertices.SetNormal(pos + 4, Vector3.Right);
				_vertices.SetTexCoord(pos + 4, GraphicsHelpers.ScaleTexCoord(new Vector2(1.0f, 0.0f), rightTexture));

				_vertices.SetPosition3D(pos + 5, b.X, b.Y, b.Z);
				_vertices.SetColor(pos + 5, Color);
				_vertices.SetNormal(pos + 5, Vector3.Right);
				_vertices.SetTexCoord(pos + 5, GraphicsHelpers.ScaleTexCoord(new Vector2(0.0f, 0.0f), rightTexture));
			}

		}

		private void SetupCollisionVertices() {
			_collisionVertices = new Vector3[_vertices.NumElements];
			for (int i = 0; i < _vertices.NumElements; ++i)
				_collisionVertices[i] = _vertices.GetPosition3D(i);
		}

		public override void Dispose()
		{
		}
	}
}

