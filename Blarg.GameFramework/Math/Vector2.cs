using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2 : IEquatable<Vector2>
	{
		public float X;
		public float Y;

		public static readonly Vector2 Zero = new Vector2(0.0f, 0.0f);
		public static readonly Vector2 XAxis = new Vector2(1.0f, 0.0f);
		public static readonly Vector2 YAxis = new Vector2(0.0f, 1.0f);
		public static readonly Vector2 Up = new Vector2(0.0f, 1.0f);
		public static readonly Vector2 Down = new Vector2(0.0f, -1.0f);
		public static readonly Vector2 Left = new Vector2(-1.0f, 0.0f);
		public static readonly Vector2 Right = new Vector2(1.0f, 0.0f);

		public float Length
		{
			get
			{
				return (float)Math.Sqrt(
					(X * X) + 
					(Y * Y)
					);

			}
		}

		public float LengthSquared
		{
			get
			{
				return
					(X * X) + 
					(Y * Y);
			}
		}

		public float InverseLength
		{
			get { return MathHelpers.FastInverseSqrt(LengthSquared); }
		}

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public Vector2(Vector2 other)
		: this(ref other)
		{
		}

		public Vector2(ref Vector2 other)
		{
			X = other.X;
			Y = other.Y;
		}

		public Vector2(Point2 point)
		: this(ref point)
		{
		}

		public Vector2(ref Point2 point)
		{
			X = (float)point.X;
			Y = (float)point.Y;
		}

		public void Set(float x, float y)
		{
			X = x;
			Y = y;
		}

		public void Set(Vector2 other)
		{
			Set(ref other);
		}

		public void Set(ref Vector2 other)
		{
			X = other.X;
			Y = other.Y;
		}

		public void Set(Point2 point)
		{
			Set(ref point);
		}

		public void Set(ref Point2 point)
		{
			X = (float)point.X;
			Y = (float)point.Y;
		}

		public static float Distance(Vector2 a, Vector2 b)
		{
			return Distance(ref a, ref b);
		}

		public static float Distance(ref Vector2 a, ref Vector2 b)
		{
			return (float)Math.Sqrt(
				((b.X - a.X) * (b.X - a.X)) + 
				((b.Y - a.Y) * (b.Y - a.Y))
				);
		}

		public static float DistanceSquared(Vector2 a, Vector2 b)
		{
			return DistanceSquared(ref a, ref b);
		}

		public static float DistanceSquared(ref Vector2 a, ref Vector2 b)
		{
			return
				((b.X - a.X) * (b.X - a.X)) + 
				((b.Y - a.Y) * (b.Y - a.Y));
		}

		public static float Dot(Vector2 a, Vector2 b)
		{
			return Dot(ref a, ref b);
		}

		public static float Dot(ref Vector2 a, ref Vector2 b)
		{
			return
				(a.X * b.X) + 
				(a.Y * b.Y);
		}

		public static Vector2 Lerp(Vector2 a, Vector2 b, float interpolation)
		{
			Vector2 result;
			Lerp(ref a, ref b, interpolation, out result);
			return result;
		}

		public static void Lerp(ref Vector2 a, ref Vector2 b, float interpolation, out Vector2 result)
		{
			result.X = a.X + (b.X - a.X) * interpolation;
			result.Y = a.Y + (b.Y - a.Y) * interpolation;
		}

		public static Vector2 Normalize(Vector2 v)
		{
			Vector2 result;
			Normalize(ref v, out result);
			return result;
		}

		public static void Normalize(ref Vector2 v, out Vector2 result)
		{
			float inverseLength = v.InverseLength;
			result.X = v.X * inverseLength;
			result.Y = v.Y * inverseLength;
		}

		public static Vector2 SetLength(Vector2 v, float length)
		{
			Vector2 result;
			SetLength(ref v, length, out result);
			return result;
		}

		public static void SetLength(ref Vector2 v, float length, out Vector2 result)
		{
			float temp = length / v.Length;
			result.X = v.X * temp;
			result.Y = v.Y * temp;
		}

		public static Vector2 Add(Vector2 left, Vector2 right)
		{
			Vector2 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Vector2 left, ref Vector2 right, out Vector2 result)
		{
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
		}

		public static Vector2 Subtract(Vector2 left, Vector2 right)
		{
			Vector2 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Vector2 left, ref Vector2 right, out Vector2 result)
		{
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
		}

		public static Vector2 Multiply(Vector2 left, Vector2 right)
		{
			Vector2 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static void Multiply(ref Vector2 left, ref Vector2 right, out Vector2 result)
		{
			result.X = left.X * right.X;
			result.Y = left.Y * right.Y;
		}

		public static Vector2 Multiply(Vector2 left, float right)
		{
			Vector2 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static void Multiply(ref Vector2 left, float right, out Vector2 result)
		{
			result.X = left.X * right;
			result.Y = left.Y * right;
		}

		public static Vector2 Divide(Vector2 left, Vector2 right)
		{
			Vector2 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static void Divide(ref Vector2 left, ref Vector2 right, out Vector2 result)
		{
			result.X = left.X / right.X;
			result.Y = left.Y / right.Y;
		}

		public static Vector2 Divide(Vector2 left, float right)
		{
			Vector2 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static void Divide(ref Vector2 left, float right, out Vector2 result)
		{
			result.X = left.X / right;
			result.Y = left.Y / right;
		}

		public static Vector2 operator +(Vector2 left, Vector2 right)
		{
			Vector2 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static Vector2 operator -(Vector2 left, Vector2 right)
		{
			Vector2 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static Vector2 operator *(Vector2 left, Vector2 right)
		{
			Vector2 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Vector2 operator *(Vector2 left, float right)
		{
			Vector2 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static Vector2 operator *(float left, Vector2 right)
		{
			Vector2 result;
			Multiply(ref right, left, out result);
			return result;
		}

		public static Vector2 operator /(Vector2 left, Vector2 right)
		{
			Vector2 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static Vector2 operator /(Vector2 left, float right)
		{
			Vector2 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !left.Equals(right);
		}

		public static Vector2 operator -(Vector2 left)
		{
			return new Vector2(-left.X, -left.Y);
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector2)
				return this.Equals((Vector2)obj);
			else
				return false;
		}

		public bool Equals(Vector2 other)
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
