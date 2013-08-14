using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Point3 : IEquatable<Point3>
	{
		public int X;
		public int Y;
		public int Z;

		public static readonly Point3 Zero = new Point3(0, 0, 0);

		public Point3(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Point3(Point3 other)
		: this(ref other)
		{
		}

		public Point3(ref Point3 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		public void Set(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Set(Point3 other)
		{
			Set(ref other);
		}

		public void Set(ref Point3 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		public static Point3 Add(Point3 left, Point3 right)
		{
			Point3 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Point3 left, ref Point3 right, out Point3 result)
		{
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			result.Z = left.Z + right.Z;
		}

		public static Point3 Subtract(Point3 left, Point3 right)
		{
			Point3 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Point3 left, ref Point3 right, out Point3 result)
		{
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			result.Z = left.Z - right.Z;
		}

		public static Point3 Multiply(Point3 left, Point3 right)
		{
			Point3 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static void Multiply(ref Point3 left, ref Point3 right, out Point3 result)
		{
			result.X = left.X * right.X;
			result.Y = left.Y * right.Y;
			result.Z = left.Z * right.Z;
		}

		public static Point3 Multiply(Point3 left, float right)
		{
			Point3 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static void Multiply(ref Point3 left, float right, out Point3 result)
		{
			result.X = (int)((float)left.X * right);
			result.Y = (int)((float)left.Y * right);
			result.Z = (int)((float)left.Z * right);
		}

		public static Point3 Divide(Point3 left, Point3 right)
		{
			Point3 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static void Divide(ref Point3 left, ref Point3 right, out Point3 result)
		{
			result.X = left.X / right.X;
			result.Y = left.Y / right.Y;
			result.Z = left.Z / right.Z;
		}

		public static Point3 Divide(Point3 left, float right)
		{
			Point3 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static void Divide(ref Point3 left, float right, out Point3 result)
		{
			result.X = (int)((float)left.X / right);
			result.Y = (int)((float)left.Y / right);
			result.Z = (int)((float)left.Z / right);
		}

		public static Point3 operator +(Point3 left, Point3 right)
		{
			Point3 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static Point3 operator -(Point3 left, Point3 right)
		{
			Point3 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static Point3 operator *(Point3 left, Point3 right)
		{
			Point3 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Point3 operator *(Point3 left, float right)
		{
			Point3 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static Point3 operator *(float left, Point3 right)
		{
			Point3 result;
			Multiply(ref right, left, out result);
			return result;
		}

		public static Point3 operator /(Point3 left, Point3 right)
		{
			Point3 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static Point3 operator /(Point3 left, float right)
		{
			Point3 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static bool operator ==(Point3 left, Point3 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Point3 left, Point3 right)
		{
			return !left.Equals(right);
		}

		public static Point3 operator -(Point3 left)
		{
			return new Point3(-left.X, -left.Y, -left.Z);
		}

		public override bool Equals(object obj)
		{
			if (obj is Point3)
				return this.Equals((Point3)obj);
			else
				return false;
		}

		public bool Equals(Point3 other)
		{
			return (X == other.X && Y == other.Y && Z == other.Z);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{X:{0} Y:{1} Z:{2}}}", X, Y, Z);
		}
	}
}
