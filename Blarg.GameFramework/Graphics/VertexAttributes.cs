using System;

namespace Blarg.GameFramework.Graphics
{
	public enum VertexStandardAttributes
	{
		// high byte = standard type bitmask
		// low byte = number of floats needed
		Position2D = 0x0102,
		Position3D = 0x0203,
		Normal = 0x0403,
		Color = 0x0804,
		TexCoord = 0x1002
	}

	public enum VertexAttributes
	{
		// standard types
		// high byte = standard type bitmask
		// low byte = number of floats needed
		Position2D = VertexStandardAttributes.Position2D,
		Position3D = VertexStandardAttributes.Position3D,
		Normal = VertexStandardAttributes.Normal,
		Color = VertexStandardAttributes.Color,
		TexCoord = VertexStandardAttributes.TexCoord,

		// generic types, value is equal to number of floats needed
		Float1 = 1,
		Float2 = 2,
		Float3 = 3,
		Float4 = 4,
		Vector2 = 2,
		Vector3 = 3,
		Vector4 = 4,
		Matrix3x3 = 9,
		Matrix4x4 = 16
	}

	public static class VertexAttributeDeclarations
	{
		public static readonly VertexAttributes[] ColorPosition3D = new VertexAttributes[]
		{
			VertexAttributes.Color,
			VertexAttributes.Position3D
		};

		public static readonly VertexAttributes[] TexturePosition3D = new VertexAttributes[]
		{
			VertexAttributes.TexCoord,
			VertexAttributes.Position3D
		};

		public static readonly VertexAttributes[] ColorNormalPosition3D = new VertexAttributes[]
		{
			VertexAttributes.Color,
			VertexAttributes.Normal,
			VertexAttributes.Position3D
		};

		public static readonly VertexAttributes[] TextureNormalPosition3D = new VertexAttributes[]
		{
			VertexAttributes.TexCoord,
			VertexAttributes.Normal,
			VertexAttributes.Position3D
		};

		public static readonly VertexAttributes[] TextureColorPosition2D = new VertexAttributes[]
		{
			VertexAttributes.TexCoord,
			VertexAttributes.Color,
			VertexAttributes.Position2D
		};

		public static readonly VertexAttributes[] TextureColorPosition3D = new VertexAttributes[]
		{
			VertexAttributes.TexCoord,
			VertexAttributes.Color,
			VertexAttributes.Position3D
		};
	}
}
