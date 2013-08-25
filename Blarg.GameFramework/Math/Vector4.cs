using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4 : IEquatable<Vector4>
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public static readonly Vector4 Zero = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

		public float Length
		{
			get
			{
				return (float)Math.Sqrt(
					(X * X) + 
					(Y * Y) + 
					(Z * Z) +
					(W * W)
					);

			}
		}

		public float LengthSquared
		{
			get
			{
				return
					(X * X) + 
					(Y * Y) + 
					(Z * Z) +
					(W * W);
			}
		}

		public float InverseLength
		{
			get { return MathHelpers.FastInverseSqrt(LengthSquared); }
		}

		public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4(Vector4 other)
		: this(ref other)
		{
		}

		public Vector4(ref Vector4 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
			W = other.W;
		}

		public void Set(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public void Set(Vector4 other)
		{
			Set(ref other);
		}

		public void Set(ref Vector4 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
			W = other.W;
		}

		public static float Distance(Vector4 a, Vector4 b)
		{
			return Distance(ref a, ref b);
		}

		public static float Distance(ref Vector4 a, ref Vector4 b)
		{
			return (float)Math.Sqrt(
				((b.X - a.X) * (b.X - a.X)) + 
				((b.Y - a.Y) * (b.Y - a.Y)) + 
				((b.Z - a.Z) * (b.Z - a.Z)) + 
				((b.W - a.W) * (b.W - a.W))
				);
		}

		public static float DistanceSquared(Vector4 a, Vector4 b)
		{
			return DistanceSquared(ref a, ref b);
		}

		public static float DistanceSquared(ref Vector4 a, ref Vector4 b)
		{
			return
				((b.X - a.X) * (b.X - a.X)) + 
				((b.Y - a.Y) * (b.Y - a.Y)) + 
				((b.Z - a.Z) * (b.Z - a.Z)) + 
				((b.W - a.W) * (b.W - a.W));
		}

		public static float Dot(Vector4 a, Vector4 b)
		{
			return Dot(ref a, ref b);
		}

		public static float Dot(ref Vector4 a, ref Vector4 b)
		{
			return
				(a.X * b.X) + 
				(a.Y * b.Y) + 
				(a.Z * b.Z) + 
				(a.W * b.W);
		}

		public static Vector4 Lerp(Vector4 a, Vector4 b, float interpolation)
		{
			Vector4 result;
			Lerp(ref a, ref b, interpolation, out result);
			return result;
		}

		public static void Lerp(ref Vector4 a, ref Vector4 b, float interpolation, out Vector4 result)
		{
			result.X = a.X + (b.X - a.X) * interpolation;
			result.Y = a.Y + (b.Y - a.Y) * interpolation;
			result.Z = a.Z + (b.Z - a.Z) * interpolation;
			result.W = a.W + (b.W - a.W) * interpolation;
		}

		public static Vector4 Normalize(Vector4 v)
		{
			Vector4 result;
			Normalize(ref v, out result);
			return result;
		}

		public static void Normalize(ref Vector4 v, out Vector4 result)
		{
			float inverseLength = v.InverseLength;
			result.X = v.X * inverseLength;
			result.Y = v.Y * inverseLength;
			result.Z = v.Z * inverseLength;
			result.W = v.W * inverseLength;
		}

		public static Vector4 SetLength(Vector4 v, float length)
		{
			Vector4 result;
			SetLength(ref v, length, out result);
			return result;
		}

		public static void SetLength(ref Vector4 v, float length, out Vector4 result)
		{
			float temp = length / v.Length;
			result.X = v.X * temp;
			result.Y = v.Y * temp;
			result.Z = v.Z * temp;
			result.W = v.W * temp;
		}

		public static Vector4 Add(Vector4 left, Vector4 right)
		{
			Vector4 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Vector4 left, ref Vector4 right, out Vector4 result)
		{
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			result.Z = left.Z + right.Z;
			result.W = left.W + right.W;
		}

		public static Vector4 Subtract(Vector4 left, Vector4 right)
		{
			Vector4 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Vector4 left, ref Vector4 right, out Vector4 result)
		{
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			result.Z = left.Z - right.Z;
			result.W = left.W - right.W;
		}

		public static Vector4 Multiply(Vector4 left, Vector4 right)
		{
			Vector4 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static void Multiply(ref Vector4 left, ref Vector4 right, out Vector4 result)
		{
			result.X = left.X * right.X;
			result.Y = left.Y * right.Y;
			result.Z = left.Z * right.Z;
			result.W = left.W * right.W;
		}

		public static Vector4 Multiply(Vector4 left, float right)
		{
			Vector4 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static void Multiply(ref Vector4 left, float right, out Vector4 result)
		{
			result.X = left.X * right;
			result.Y = left.Y * right;
			result.Z = left.Z * right;
			result.W = left.W * right;
		}

		public static Vector4 Divide(Vector4 left, Vector4 right)
		{
			Vector4 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static void Divide(ref Vector4 left, ref Vector4 right, out Vector4 result)
		{
			result.X = left.X / right.X;
			result.Y = left.Y / right.Y;
			result.Z = left.Z / right.Z;
			result.W = left.W / right.W;
		}

		public static Vector4 Divide(Vector4 left, float right)
		{
			Vector4 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static void Divide(ref Vector4 left, float right, out Vector4 result)
		{
			result.X = left.X / right;
			result.Y = left.Y / right;
			result.Z = left.Z / right;
			result.W = left.W / right;
		}

		public static Vector4 operator +(Vector4 left, Vector4 right)
		{
			Vector4 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static Vector4 operator -(Vector4 left, Vector4 right)
		{
			Vector4 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static Vector4 operator *(Vector4 left, Vector4 right)
		{
			Vector4 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Vector4 operator *(Vector4 left, float right)
		{
			Vector4 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static Vector4 operator *(float left, Vector4 right)
		{
			Vector4 result;
			Multiply(ref right, left, out result);
			return result;
		}

		public static Vector4 operator /(Vector4 left, Vector4 right)
		{
			Vector4 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static Vector4 operator /(Vector4 left, float right)
		{
			Vector4 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static bool operator ==(Vector4 left, Vector4 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector4 left, Vector4 right)
		{
			return !left.Equals(right);
		}

		public static Vector4 operator -(Vector4 left)
		{
			return new Vector4(-left.X, -left.Y, -left.Z, -left.W);
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector4)
				return this.Equals((Vector4)obj);
			else
				return false;
		}

		public bool Equals(Vector4 other)
		{
			return (X == other.X && Y == other.Y && Z == other.Z && W == other.W);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{X:{0} Y:{1} Z:{2} W:{3}}}", X, Y, Z, W);
		}
	}
}
