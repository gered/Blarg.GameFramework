using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundingBox : IEquatable<BoundingBox>
	{
		public Vector3 Min;
		public Vector3 Max;

		public float Width
		{
			get { return Math.Abs(Max.X - Min.X); }
		}

		public float Height
		{
			get { return Math.Abs(Max.Y - Min.Y); }
		}

		public float Depth
		{
			get { return Math.Abs(Max.Z - Min.Z); }
		}

		public BoundingBox(Vector3 min, Vector3 max)
			: this(ref min, ref max)
		{
		}

		public BoundingBox(ref Vector3 min, ref Vector3 max)
		{
			Min = min;
			Max = max;
		}

		public BoundingBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
		{
			Min.X = minX;
			Min.Y = minY;
			Min.Z = minZ;
			Max.X = maxX;
			Max.Y = maxY;
			Max.Z = maxZ;
		}

		public BoundingBox(Vector3 center, float halfWidth)
			: this(ref center, halfWidth)
		{
		}

		public BoundingBox(ref Vector3 center, float halfWidth)
		{
			Min.X = center.X - halfWidth;
			Min.Y = center.Y - halfWidth;
			Min.Z = center.Z - halfWidth;
			Max.X = center.X + halfWidth;
			Max.Y = center.Y + halfWidth;
			Max.Z = center.Z + halfWidth;
		}

		public BoundingBox(Vector3[] vertices)
		{
			float minX = 0.0f;
			float minY = 0.0f;
			float minZ = 0.0f;
			float maxX = 0.0f;
			float maxY = 0.0f;
			float maxZ = 0.0f;

			for (int i = 0; i < vertices.Length; ++i)
			{
				minX = Math.Min(vertices[i].X, minX);
				minY = Math.Min(vertices[i].Y, minY);
				minZ = Math.Min(vertices[i].Z, minZ);
				maxX = Math.Max(vertices[i].X, maxX);
				maxY = Math.Max(vertices[i].Y, maxY);
				maxZ = Math.Max(vertices[i].Z, maxZ);
			}

			Min.X = minX;
			Min.Y = minY;
			Min.Z = minZ;
			Max.X = maxX;
			Max.Y = maxY;
			Max.Z = maxZ;
		}

		public static float GetSquaredDistanceFromPointToBox(Vector3 point, BoundingBox box)
		{
			return GetSquaredDistanceFromPointToBox(ref point, ref box);
		}

		public static float GetSquaredDistanceFromPointToBox(ref Vector3 point, ref BoundingBox box)
		{
			float distanceSq = 0.0f;
			float v;

			v = point.X;
			if (v < box.Min.X)
				distanceSq += (box.Min.X - v) * (box.Min.X - v);
			if (v > box.Max.X)
				distanceSq += (v - box.Max.X) * (v - box.Max.X);

			v = point.Y;
			if (v < box.Min.Y)
				distanceSq += (box.Min.Y - v) * (box.Min.Y - v);
			if (v > box.Max.Y)
				distanceSq += (v - box.Max.Y) * (v - box.Max.Y);

			v = point.Z;
			if (v < box.Min.Z)
				distanceSq += (box.Min.Z - v) * (box.Min.Z - v);
			if (v > box.Max.Z)
				distanceSq += (v - box.Max.Z) * (v - box.Max.Z);

			return distanceSq;
		}

		public static float GetDistanceFromPointToBox(Vector3 point, BoundingBox box)
		{
			return GetDistanceFromPointToBox(ref point, ref box);
		}

		public static float GetDistanceFromPointToBox(ref Vector3 point, ref BoundingBox box)
		{
			return (float)Math.Sqrt(GetSquaredDistanceFromPointToBox(ref point, ref box));
		}

		public static bool operator ==(BoundingBox left, BoundingBox right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(BoundingBox left, BoundingBox right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is BoundingBox)
				return this.Equals((BoundingBox)obj);
			else
				return false;
		}

		public bool Equals(BoundingBox other)
		{
			return (Min == other.Min && Max == other.Max);
		}

		public override int GetHashCode()
		{
			return Min.GetHashCode() ^ Max.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{{0}, {1}}}", Min, Max);
		}
	}
}
