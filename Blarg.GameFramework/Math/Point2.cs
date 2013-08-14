using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Point2 : IEquatable<Point2>
	{
		public int X;
		public int Y;

		public static readonly Point2 Zero = new Point2(0, 0);

		public Point2(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point2(Point2 other)
		: this(ref other)
		{
		}

		public Point2(ref Point2 other)
		{
			X = other.X;
			Y = other.Y;
		}

		public void Set(int x, int y)
		{
			X = x;
			Y = y;
		}

		public void Set(Point2 other)
		{
			Set(ref other);
		}

		public void Set(ref Point2 other)
		{
			X = other.X;
			Y = other.Y;
		}

		public static Point2 Add(Point2 left, Point2 right)
		{
			Point2 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Point2 left, ref Point2 right, out Point2 result)
		{
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
		}

		public static Point2 Subtract(Point2 left, Point2 right)
		{
			Point2 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Point2 left, ref Point2 right, out Point2 result)
		{
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
		}

		public static Point2 Multiply(Point2 left, Point2 right)
		{
			Point2 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static void Multiply(ref Point2 left, ref Point2 right, out Point2 result)
		{
			result.X = left.X * right.X;
			result.Y = left.Y * right.Y;
		}

		public static Point2 Multiply(Point2 left, float right)
		{
			Point2 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static void Multiply(ref Point2 left, float right, out Point2 result)
		{
			result.X = (int)((float)left.X * right);
			result.Y = (int)((float)left.Y * right);
		}

		public static Point2 Divide(Point2 left, Point2 right)
		{
			Point2 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static void Divide(ref Point2 left, ref Point2 right, out Point2 result)
		{
			result.X = left.X / right.X;
			result.Y = left.Y / right.Y;
		}

		public static Point2 Divide(Point2 left, float right)
		{
			Point2 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static void Divide(ref Point2 left, float right, out Point2 result)
		{
			result.X = (int)((float)left.X / right);
			result.Y = (int)((float)left.Y / right);
		}

		public static Point2 operator +(Point2 left, Point2 right)
		{
			Point2 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static Point2 operator -(Point2 left, Point2 right)
		{
			Point2 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static Point2 operator *(Point2 left, Point2 right)
		{
			Point2 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Point2 operator *(Point2 left, float right)
		{
			Point2 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static Point2 operator *(float left, Point2 right)
		{
			Point2 result;
			Multiply(ref right, left, out result);
			return result;
		}

		public static Point2 operator /(Point2 left, Point2 right)
		{
			Point2 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static Point2 operator /(Point2 left, float right)
		{
			Point2 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static bool operator ==(Point2 left, Point2 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Point2 left, Point2 right)
		{
			return !left.Equals(right);
		}

		public static Point2 operator -(Point2 left)
		{
			return new Point2(-left.X, -left.Y);
		}

		public override bool Equals(object obj)
		{
			if (obj is Point2)
				return this.Equals((Point2)obj);
			else
				return false;
		}

		public bool Equals(Point2 other)
		{
			return (X == other.X && Y == other.Y);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{X:{0} Y:{1}}}", X, Y);
		}
	}
}
