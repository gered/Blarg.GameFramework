using System;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.TileMap.Meshes
{
	public abstract class TileMesh : IDisposable
	{
		public static readonly Vector3 OFFSET = new Vector3(0.5f, 0.5f, 0.5f);
		public static readonly Vector3 UNIT_SIZE = new Vector3(1.0f, 1.0f, 1.0f);
		public static readonly BoundingBox UNIT_BOUNDS = new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f));

		public const byte SIDE_TOP = 1;
		public const byte SIDE_BOTTOM = 2;
		public const byte SIDE_FRONT = 4;
		public const byte SIDE_BACK = 8;
		public const byte SIDE_LEFT = 16;
		public const byte SIDE_RIGHT = 32;
		public const byte SIDE_ALL = (SIDE_TOP | SIDE_BOTTOM | SIDE_FRONT | SIDE_BACK | SIDE_LEFT | SIDE_RIGHT);

		public const int CUBE_VERTICES_PER_FACE = 6;

		public readonly byte OpaqueSides;
		public readonly bool Alpha;
		public readonly float Translucency;
		public readonly byte LightValue;
		public readonly Color Color;

		public abstract BoundingBox Bounds { get; }
		public abstract VertexBuffer Vertices { get; }
		public abstract Vector3[] CollisionVertices { get; }

		public bool IsCompletelyOpaque
		{
			get { return OpaqueSides == SIDE_ALL; }
		}

		public bool IsLightSource
		{
			get { return LightValue > 0; }
		}

		public TileMesh(byte opaqueSides, bool alpha, float translucency, byte lightValue, Color color)
		{
			OpaqueSides = opaqueSides;
			Alpha = alpha;
			Translucency = translucency;
			LightValue = lightValue;
			Color = color;
		}

		public bool IsOpaque(byte side)
		{
			return OpaqueSides.IsBitSet(side);
		}

		public abstract void Dispose();
	}
}

