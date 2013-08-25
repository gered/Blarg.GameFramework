using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3 : IEquatable<Vector3>
	{
		public float X;
		public float Y;
		public float Z;

		public static readonly Vector3 Zero = new Vector3(0.0f, 0.0f, 0.0f);
		public static readonly Vector3 XAxis = new Vector3(1.0f, 0.0f, 0.0f);
		public static readonly Vector3 YAxis = new Vector3(0.0f, 1.0f, 0.0f);
		public static readonly Vector3 ZAxis = new Vector3(0.0f, 0.0f, 1.0f);
		public static readonly Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
		public static readonly Vector3 Down = new Vector3(0.0f, -1.0f, 0.0f);
		public static readonly Vector3 Forward = new Vector3(0.0f, 0.0f, -1.0f);
		public static readonly Vector3 Backward = new Vector3(0.0f, 0.0f, 1.0f);
		public static readonly Vector3 Left = new Vector3(-1.0f, 0.0f, 0.0f);
		public static readonly Vector3 Right = new Vector3(1.0f, 0.0f, 0.0f);

		public float Length
		{
			get
			{
				return (float)Math.Sqrt(
					(X * X) + 
					(Y * Y) + 
					(Z * Z)
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
					(Z * Z);
			}
		}

		public float InverseLength
		{
			get { return MathHelpers.FastInverseSqrt(LengthSquared); }
		}

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(Vector3 other)
		: this(ref other)
		{
		}

		public Vector3(ref Vector3 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		public Vector3(Point3 point)
		: this(ref point)
		{
		}

		public Vector3(ref Point3 point)
		{
			X = (float)point.X;
			Y = (float)point.Y;
			Z = (float)point.Z;
		}

		public void Set(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Set(Vector3 other)
		{
			Set(ref other);
		}

		public void Set(ref Vector3 other)
		{
			X = other.X;
			Y = other.Y;
			Z = other.Z;
		}

		public void Set(Point3 point)
		{
			Set(ref point);
		}

		public void Set(ref Point3 point)
		{
			X = (float)point.X;
			Y = (float)point.Y;
			Z = (float)point.Z;
		}

		public static Vector3 Cross(Vector3 a, Vector3 b)
		{
			Vector3 result;
			Cross(ref a, ref b, out result);
			return result;
		}

		public static void Cross(ref Vector3 a, ref Vector3 b, out Vector3 result)
		{
			result.X = (a.Y * b.Z) - (a.Z * b.Y);
			result.Y = (a.Z * b.X) - (a.X * b.Z);
			result.Z = (a.X * b.Y) - (a.Y * b.X);
		}

		public static float Distance(Vector3 a, Vector3 b)
		{
			return Distance(ref a, ref b);
		}

		public static float Distance(ref Vector3 a, ref Vector3 b)
		{
			return (float)Math.Sqrt(
				((b.X - a.X) * (b.X - a.X)) + 
				((b.Y - a.Y) * (b.Y - a.Y)) + 
				((b.Z - a.Z) * (b.Z - a.Z))
				);
		}

		public static float DistanceSquared(Vector3 a, Vector3 b)
		{
			return DistanceSquared(ref a, ref b);
		}

		public static float DistanceSquared(ref Vector3 a, ref Vector3 b)
		{
			return
				((b.X - a.X) * (b.X - a.X)) + 
				((b.Y - a.Y) * (b.Y - a.Y)) + 
				((b.Z - a.Z) * (b.Z - a.Z));
		}

		public static float Dot(Vector3 a, Vector3 b)
		{
			return Dot(ref a, ref b);
		}

		public static float Dot(ref Vector3 a, ref Vector3 b)
		{
			return
				(a.X * b.X) + 
				(a.Y * b.Y) + 
				(a.Z * b.Z);
		}

		public static Vector3 Lerp(Vector3 a, Vector3 b, float interpolation)
		{
			Vector3 result;
			Lerp(ref a, ref b, interpolation, out result);
			return result;
		}

		public static void Lerp(ref Vector3 a, ref Vector3 b, float interpolation, out Vector3 result)
		{
			result.X = a.X + (b.X - a.X) * interpolation;
			result.Y = a.Y + (b.Y - a.Y) * interpolation;
			result.Z = a.Z + (b.Z - a.Z) * interpolation;
		}

		public static Vector3 Normalize(Vector3 v)
		{
			Vector3 result;
			Normalize(ref v, out result);
			return result;
		}

		public static void Normalize(ref Vector3 v, out Vector3 result)
		{
			float inverseLength = v.InverseLength;
			result.X = v.X * inverseLength;
			result.Y = v.Y * inverseLength;
			result.Z = v.Z * inverseLength;
		}

		public static Vector3 SetLength(Vector3 v, float length)
		{
			Vector3 result;
			SetLength(ref v, length, out result);
			return result;
		}

		public static void SetLength(ref Vector3 v, float length, out Vector3 result)
		{
			float temp = length / v.Length;
			result.X = v.X * temp;
			result.Y = v.Y * temp;
			result.Z = v.Z * temp;
		}

		public static Vector3 SurfaceNormal(Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 result;
			SurfaceNormal(ref a, ref b, ref c, out result);
			return result;
		}

		public static void SurfaceNormal(ref Vector3 a, ref Vector3 b, ref Vector3 c, out Vector3 result)
		{
			result = Normalize(Cross((b - a), (c - a)));
		}

		public static Vector3 Add(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			result.Z = left.Z + right.Z;
		}

		public static Vector3 Subtract(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			result.Z = left.Z - right.Z;
		}

		public static Vector3 Multiply(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static void Multiply(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result.X = left.X * right.X;
			result.Y = left.Y * right.Y;
			result.Z = left.Z * right.Z;
		}

		public static Vector3 Multiply(Vector3 left, float right)
		{
			Vector3 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static void Multiply(ref Vector3 left, float right, out Vector3 result)
		{
			result.X = left.X * right;
			result.Y = left.Y * right;
			result.Z = left.Z * right;
		}

		public static Vector3 Divide(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static void Divide(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result.X = left.X / right.X;
			result.Y = left.Y / right.Y;
			result.Z = left.Z / right.Z;
		}

		public static Vector3 Divide(Vector3 left, float right)
		{
			Vector3 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static void Divide(ref Vector3 left, float right, out Vector3 result)
		{
			result.X = left.X / right;
			result.Y = left.Y / right;
			result.Z = left.Z / right;
		}

		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static Vector3 operator *(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Vector3 operator *(Vector3 left, float right)
		{
			Vector3 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static Vector3 operator *(float left, Vector3 right)
		{
			Vector3 result;
			Multiply(ref right, left, out result);
			return result;
		}

		public static Vector3 operator /(Vector3 left, Vector3 right)
		{
			Vector3 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static Vector3 operator /(Vector3 left, float right)
		{
			Vector3 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static bool operator ==(Vector3 left, Vector3 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector3 left, Vector3 right)
		{
			return !left.Equals(right);
		}

		public static Vector3 operator -(Vector3 left)
		{
			return new Vector3(-left.X, -left.Y, -left.Z);
		}

		public override bool Equals(object obj)
		{
			if (obj is Vector3)
				return this.Equals((Vector3)obj);
			else
				return false;
		}

		public bool Equals(Vector3 other)
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
