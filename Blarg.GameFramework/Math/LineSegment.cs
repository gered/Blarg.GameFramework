using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct LineSegment : IEquatable<LineSegment>
	{
		public Vector3 A;
		public Vector3 B;

		public LineSegment(Vector3 a, Vector3 b)
		: this(ref a, ref b)
		{
		}

		public LineSegment(ref Vector3 a, ref Vector3 b)
		{
			A = a;
			B = b;
		}

		public LineSegment(float ax, float ay, float az, float bx, float by, float bz)
		{
			A.X = ax;
			A.Y = ay;
			A.Z = az;
			B.X = bx;
			B.Y = by;
			B.Z = bz;
		}

		public LineSegment(Ray ray, float length)
		: this(ref ray, length)
		{
		}

		public LineSegment(ref Ray ray, float length)
		{
			A = ray.Position;
			B = ray.GetPositionAt(length);
		}

		public static bool operator ==(LineSegment left, LineSegment right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(LineSegment left, LineSegment right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is LineSegment)
				return this.Equals((LineSegment)obj);
			else
				return false;
		}

		public bool Equals(LineSegment other)
		{
			return (A == other.A && B == other.B);
		}

		public override int GetHashCode()
		{
			return A.GetHashCode() ^ B.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{{0}-{1}}}", A, B);
		}
	}
}
