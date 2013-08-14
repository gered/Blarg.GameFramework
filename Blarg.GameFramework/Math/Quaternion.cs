using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Quaternion : IEquatable<Quaternion>
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public float Length
		{
			get
			{
				return (float)Math.Sqrt(
					(W * W) + 
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
					(W * W) + 
						(X * X) + 
						(Y * Y) + 
						(Z * Z);
			}
		}

		public Vector3 Vector
		{
			get { return new Vector3(X, Y, Z); }
		}

		public Quaternion(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Quaternion(Quaternion q)
		: this(ref q)
		{
		}

		public Quaternion(ref Quaternion q)
		{
			X = q.X;
			Y = q.Y;
			Z = q.Z;
			W = q.W;
		}

		public void Set(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public void Set(Quaternion q)
		{
			Set(ref q);
		}

		public void Set(ref Quaternion q)
		{
			X = q.X;
			Y = q.Y;
			Z = q.Z;
			W = q.W;
		}

		public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

		public Matrix4x4 ToMatrix4x4()
		{
			Matrix4x4 result;
			ToMatrix4x4(out result);
			return result;
		}

		public void ToMatrix4x4(out Matrix4x4 result)
		{
			result.M11 = 1.0f - (2.0f * ((Y * Y) + (Z * Z)));
			result.M21 = 2.0f * ((X * Y) + (Z * W));
			result.M31 = 2.0f * ((Z * X) - (Y * W));
			result.M41 = 0.0f;

			result.M12 = 2.0f * ((X * Y) - (Z * W));
			result.M22 = 1.0f - (2.0f * ((Z * Z) + (X * X)));
			result.M32 = 2.0f * ((Y * Z) + (X * W));
			result.M42 = 0.0f;

			result.M13 = 2.0f * ((Z * X) + (Y * W));
			result.M23 = 2.0f * ((Y * Z) - (X * W));
			result.M33 = 1.0f - (2.0f * ((Y * Y) + (X * X)));
			result.M43 = 0.0f;

			result.M14 = 0.0f;
			result.M24 = 0.0f;
			result.M34 = 0.0f;
			result.M44 = 1.0f;
		}

		public Matrix3x3 ToMatrix3x3()
		{
			Matrix3x3 result;
			ToMatrix3x3(out result);
			return result;
		}

		public void ToMatrix3x3(out Matrix3x3 result)
		{
			result.M11 = 1.0f - (2.0f * ((Y * Y) + (Z * Z)));
			result.M21 = 2.0f * ((X * Y) + (Z * W));
			result.M31 = 2.0f * ((Z * X) - (Y * W));

			result.M12 = 2.0f * ((X * Y) - (Z * W));
			result.M22 = 1.0f - (2.0f * ((Z * Z) + (X * X)));
			result.M32 = 2.0f * ((Y * Z) + (X * W));

			result.M13 = 2.0f * ((Z * X) + (Y * W));
			result.M23 = 2.0f * ((Y * Z) - (X * W));
			result.M33 = 1.0f - (2.0f * ((Y * Y) + (X * X)));
		}

		public static Quaternion Conjugate(Quaternion q)
		{
			Quaternion result;
			Conjugate(ref q, out result);
			return result;
		}

		public static void Conjugate(ref Quaternion q, out Quaternion result)
		{
			result.X = -q.X;
			result.Y = -q.Y;
			result.Z = -q.Z;
			result.W = q.W;
		}

		public static Quaternion CreateFromAxisAngle(float angle, Vector3 axis)
		{
			Quaternion result;
			CreateFromAxisAngle(angle, ref axis, out result);
			return result;
		}

		public static void CreateFromAxisAngle(float angle, ref Vector3 axis, out Quaternion result)
		{
			float c = (float)Math.Cos(angle / 2.0f);
			float s = (float)Math.Sin(angle / 2.0f);

			result.W = c;
			result.X = axis.X * s;
			result.Y = axis.Y * s;
			result.Z = axis.Z * s;

			result = Normalize(result);
		}

		public static Quaternion CreateFromEulerAngles(float x, float y, float z)
		{
			Quaternion result;
			CreateFromEulerAngles(x, y, z, out result);
			return result;
		}

		public static void CreateFromEulerAngles(float x, float y, float z, out Quaternion result)
		{
			var qx = new Quaternion((float)Math.Sin(x / 2.0f), 0.0f, 0.0f, (float)Math.Cos(x / 2.0f));
			var qy = new Quaternion(0.0f, (float)Math.Sin(y / 2.0f), 0.0f, (float)Math.Cos(y / 2.0f));
			var qz = new Quaternion(0.0f, 0.0f, (float)Math.Sin(z / 2.0f), (float)Math.Cos(z / 2.0f));

			result = Normalize(qz * qy * qx);
		}

		public static Quaternion Cross(Quaternion a, Quaternion b)
		{
			Quaternion result;
			Cross(ref a, ref b, out result);
			return result;
		}

		public static void Cross(ref Quaternion a, ref Quaternion b, out Quaternion result)
		{
			result = a * b;
		}

		public static float Dot(Quaternion a, Quaternion b)
		{
			return Dot(ref a, ref b);
		}

		public static float Dot(ref Quaternion a, ref Quaternion b)
		{
			return (
				(a.W * b.W) + 
				(a.X * b.X) + 
				(a.Y * b.Y) + 
				(a.Z * b.Z)
				);
		}

		public static void ExtractAxisAngle(Quaternion q, out float angle, out Vector3 axis)
		{
			ExtractAxisAngle(ref q, out angle, out axis);
		}

		public static void ExtractAxisAngle(ref Quaternion q, out float angle, out Vector3 axis)
		{
			angle = 2.0f * (float)Math.Acos(q.W);
			float n = (float)Math.Sqrt(1.0f - (q.W * q.W));
			if (n > 0.0001f)
				axis = q.Vector / n;
			else
				axis = Vector3.XAxis;
		}

		public static Quaternion Inverse(Quaternion q)
		{
			Quaternion result;
			Inverse(ref q, out result);
			return result;
		}

		public static void Inverse(ref Quaternion q, out Quaternion result)
		{
			float inverseSquaredLength = 1.0f / q.LengthSquared;
			result.X = -q.X * inverseSquaredLength;
			result.Y = -q.Y * inverseSquaredLength;
			result.Z = -q.Z * inverseSquaredLength;
			result.W = q.W * inverseSquaredLength;
		}

		public static Quaternion Lerp(Quaternion a, Quaternion b, float interpolation)
		{
			Quaternion result;
			Lerp(ref a, ref b, interpolation, out result);
			return result;
		}

		public static void Lerp(ref Quaternion a, ref Quaternion b, float interpolation, out Quaternion result)
		{
			result = (a * (1.0f - interpolation)) + (b * interpolation);
		}

		public static Quaternion Normalize(Quaternion q)
		{
			Quaternion result;
			Normalize(ref q, out result);
			return result;
		}

		public static void Normalize(ref Quaternion q, out Quaternion result)
		{
			float inverseLength = 1.0f / q.Length;
			result.X = q.X * inverseLength;
			result.Y = q.Y * inverseLength;
			result.Z = q.Z * inverseLength;
			result.W = q.W * inverseLength;
		}

		public static Quaternion Slerp(Quaternion a, Quaternion b, float interpolation)
		{
			Quaternion result;
			Slerp(ref a, ref b, interpolation, out result);
			return result;
		}

		public static void Slerp(ref Quaternion a, ref Quaternion b, float interpolation, out Quaternion result)
		{
			if (a.LengthSquared == 0.0f)
			{
				if (b.LengthSquared == 0.0f)
				{
					result = Identity;
					return;
				}
				else
				{
					result = b;
					return;
				}
			}
			else if (b.LengthSquared == 0.0f)
			{
				result = a;
				return;
			}

			Quaternion q1 = a;
			Quaternion q2 = b;

			float cosHalfAngle = q1.W * q2.W + Vector3.Dot(q1.Vector, q2.Vector);

			if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
			{
				result = q1;
				return;
			}
			else if (cosHalfAngle < 0.0f)
			{
				q2.X = -q2.X;
				q2.Y = -q2.Y;
				q2.Z = -q2.Z;
				q2.W = -q2.W;
				cosHalfAngle = -cosHalfAngle;
			}

			float blendA;
			float blendB;
			if (cosHalfAngle < 0.99f)
			{
				float halfAngle = (float)Math.Acos(cosHalfAngle);
				float sinHalfAngle = (float)Math.Sin(halfAngle);
				float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
				blendA = (float)Math.Sin(halfAngle * (1.0f - interpolation)) * oneOverSinHalfAngle;
				blendB = (float)Math.Sin(halfAngle * interpolation) * oneOverSinHalfAngle;
			}
			else
			{
				blendA = 1.0f - interpolation;
				blendB = interpolation;
			}

			var v = q1.Vector * blendA + q2.Vector * blendB;
			float w = q1.W * blendA + q2.W * blendB;
			var temp = new Quaternion(v.X, v.Y, v.Z, w);
			if (temp.LengthSquared > 0.0f)
				result = Normalize(temp);
			else
				result = Identity;
		}

		public static Vector3 Transform(Quaternion q, Vector3 v)
		{
			Vector3 result;
			Transform(ref q, ref v, out result);
			return result;
		}

		public static void Transform(ref Quaternion q, ref Vector3 v, out Vector3 result)
		{

			Vector3 temp;
			temp.X = ((v.X * ((1.0f - (q.Y * (q.Y + q.Y))) - (q.Z * (q.Z + q.Z)))) + (v.Y * ((q.X * (q.Y + q.Y)) - (q.W * (q.Z + q.Z))))) + (v.Z * ((q.X * (q.Z + q.Z)) + (q.W * (q.Y + q.Y))));
			temp.Y = ((v.X * ((q.X * (q.Y + q.Y)) + (q.W * (q.Z + q.Z)))) + (v.Y * ((1.0f - (q.X * (q.X + q.X))) - (q.Z * (q.Z + q.Z))))) + (v.Z * ((q.Y * (q.Z + q.Z)) - (q.W * (q.X + q.X))));
			temp.Z = ((v.X * ((q.X * (q.Z + q.Z)) - (q.W * (q.Y + q.Y)))) + (v.Y * ((q.Y * (q.Z + q.Z)) + (q.W * (q.X + q.X))))) + (v.Z * ((1.0f - (q.X * (q.X + q.X))) - (q.Y * (q.Y + q.Y))));
			result = temp;
		}

		public static Quaternion Add(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			result.Z = left.Z + right.Z;
			result.W = left.W + right.W;
		}

		public static Quaternion Subtract(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			result.Z = left.Z - right.Z;
			result.W = left.W - right.W;
		}

		public static Quaternion Multiply(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static void Multiply(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			float X = (left.W * right.X) + (left.X * right.W) + (left.Y * right.Z) - (left.Z * right.Y);
			float Y = (left.W * right.Y) + (left.Y * right.W) + (left.Z * right.X) - (left.X * right.Z);
			float Z = (left.W * right.Z) + (left.Z * right.W) + (left.X * right.Y) - (left.Y * right.X);
			float W = (left.W * right.W) - (left.X * right.X) - (left.Y * right.Y) - (left.Z * right.Z);

			result.X = X;
			result.Y = Y;
			result.Z = Z;
			result.W = W;
		}

		public static Quaternion Multiply(Quaternion left, float right)
		{
			Quaternion result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static void Multiply(ref Quaternion left, float right, out Quaternion result)
		{
			result.X = left.X * right;
			result.Y = left.Y * right;
			result.Z = left.Z * right;
			result.W = left.W * right;
		}

		public static Quaternion Divide(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static void Divide(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			result.X = left.X / right.X;
			result.Y = left.Y / right.Y;
			result.Z = left.Z / right.Z;
			result.W = left.W / right.W;
		}

		public static Quaternion Divide(Quaternion left, float right)
		{
			Quaternion result;
			Divide(ref left, right, out result);
			return result;
		}

		public static void Divide(ref Quaternion left, float right, out Quaternion result)
		{
			result.X = left.X / right;
			result.Y = left.Y / right;
			result.Z = left.Z / right;
			result.W = left.W / right;
		}

		public static Quaternion operator +(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static Quaternion operator -(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static Quaternion operator *(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Quaternion operator *(Quaternion left, float right)
		{
			Quaternion result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static Quaternion operator /(Quaternion left, Quaternion right)
		{
			Quaternion result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static Quaternion operator /(Quaternion left, float right)
		{
			Quaternion result;
			Divide(ref left, right, out result);
			return result;
		}

		public static bool operator ==(Quaternion left, Quaternion right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Quaternion left, Quaternion right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Quaternion)
				return this.Equals((Quaternion)obj);
			else
				return false;
		}

		public bool Equals(Quaternion other)
		{
			return (
				X == other.X &&
				Y == other.Y &&
				Z == other.Z &&
				W == other.W
				);
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
