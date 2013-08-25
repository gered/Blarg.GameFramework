using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix3x3 : IEquatable<Matrix3x3>
	{
		public float M11;
		public float M21;
		public float M31;
		public float M12;
		public float M22;
		public float M32;
		public float M13;
		public float M23;
		public float M33;

		const int _11 = 0;
		const int _12 = 3;
		const int _13 = 6;
		const int _21 = 1;
		const int _22 = 4;
		const int _23 = 7;
		const int _31 = 2;
		const int _32 = 5;
		const int _33 = 8;

		public float Determinant
		{
			get
			{
				return
					M11 * M22 * M33 + 
					M12 * M23 * M31 + 
					M13 * M21 * M32 - 
					M11 * M23 * M32 - 
					M12 * M21 * M33 - 
					M13 * M22 * M31;
			}
		}

		public Vector3 Forward
		{
			get { return new Vector3(M13, M23, M33); }
		}

		public Vector3 Backward
		{
			get { return new Vector3(-M13, -M23, -M33); }
		}

		public Vector3 Left
		{
			get { return new Vector3(M11, M21, M31); }
		}

		public Vector3 Right
		{
			get { return new Vector3(-M11, -M21, -M31); }
		}

		public Vector3 Up
		{
			get { return new Vector3(M12, M22, M32); }
		}

		public Vector3 Down
		{
			get { return new Vector3(-M12, -M22, -M32); }
		}

		public Matrix3x3(
			float m11, float m12, float m13,
			float m21, float m22, float m23,
			float m31, float m32, float m33
			)
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M31 = m31;
			M32 = m32;
			M33 = m33;
		}

		public Matrix3x3(float[] m)
		{
			M11 = m[_11];
			M12 = m[_12];
			M13 = m[_13];
			M21 = m[_21];
			M22 = m[_22];
			M23 = m[_23];
			M31 = m[_31];
			M32 = m[_32];
			M33 = m[_33];
		}

		public void Set(
			float m11, float m12, float m13,
			float m21, float m22, float m23,
			float m31, float m32, float m33
			)
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M31 = m31;
			M32 = m32;
			M33 = m33;
		}

		public void Set(float[] m)
		{
			M11 = m[_11];
			M12 = m[_12];
			M13 = m[_13];
			M21 = m[_21];
			M22 = m[_22];
			M23 = m[_23];
			M31 = m[_31];
			M32 = m[_32];
			M33 = m[_33];
		}

		public static readonly Matrix3x3 Identity = new Matrix3x3(
			1.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 1.0f
			);

		public Quaternion ToQuaternion()
		{
			Quaternion result;
			ToQuaternion(out result);
			return result;
		}

		public void ToQuaternion(out Quaternion result)
		{
			Quaternion temp;

			float n = M11 + M22 + M33;
			if (n > 0.0f)
			{
				float a = (float)Math.Sqrt(n + 1.0f);
				temp.W = a / 2.0f;
				a = 0.5f / a;
				temp.X = (M32 - M23) * a;
				temp.Y = (M13 - M31) * a;
				temp.Z = (M21 - M12) * a;
			}
			else if ((M11 >= M22) && (M11 >= M33))
			{
				float a = (float)Math.Sqrt(1.0f + M11 - M22 - M33);
				float b = 0.5f / a;

				temp.X = 0.5f * a;
				temp.Y = (M21 + M12) * b;
				temp.Z = (M31 + M13) * b;
				temp.W = (M32 - M23) * b;
			}
			else if (M22 > M33)
			{
				float a = (float)Math.Sqrt(1.0f + M22 - M11 - M33);
				float b = 0.5f / a;

				temp.X = (M12 + M21) * b;
				temp.Y = 0.5f * a;
				temp.Z = (M23 + M32) * b;
				temp.W = (M13 - M31) * b;
			}
			else
			{
				float a = (float)Math.Sqrt(1.0f + M33 - M11 - M22);
				float b = 0.5f / a;

				temp.X = (M13 + M31) * b;
				temp.Y = (M23 + M32) * b;
				temp.Z = 0.5f * a;
				temp.W = (M21 - M12) * b;
			}

			Quaternion.Normalize(ref temp, out result);
		}

		public Matrix4x4 ToMatrix4x4()
		{
			Matrix4x4 result;
			ToMatrix4x4(out result);
			return result;
		}

		public void ToMatrix4x4(out Matrix4x4 result)
		{
			result.M11 = M11;
			result.M12 = M12;
			result.M13 = M13;
			result.M14 = 0.0f;

			result.M21 = M21;
			result.M22 = M22;
			result.M23 = M23;
			result.M24 = 0.0f;

			result.M31 = M31;
			result.M32 = M32;
			result.M33 = M33;
			result.M34 = 0.0f;

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = 0.0f;
			result.M44 = 1.0f;
		}

		public void ToArray(float[] result)
		{
			if (result == null || result.Length < 9)
				throw new ArgumentException();

			result[0] = M11;
			result[3] = M12;
			result[6] = M13;
			result[1] = M21;
			result[4] = M22;
			result[7] = M23;
			result[2] = M31;
			result[5] = M32;
			result[8] = M33;
		}

		public float[] ToArray()
		{
			var result = new float[9];
			ToArray(result);
			return result;
		}

		public static Matrix3x3 CreateFromEulerAngles(float x, float y, float z)
		{
			Matrix3x3 result;
			CreateFromEulerAngles(x, y, z, out result);
			return result;
		}

		public static void CreateFromEulerAngles(float x, float y, float z, out Matrix3x3 result)
		{
			Matrix3x3 rotateZ;
			Matrix3x3 rotateY;
			Matrix3x3 rotateX;
			Matrix3x3.CreateRotationZ(z, out rotateZ);
			Matrix3x3.CreateRotationY(y, out rotateY);
			Matrix3x3.CreateRotationX(x, out rotateX);

			result = rotateZ * rotateY * rotateX;
		}

		public static Matrix3x3 CreateRotation(float angle, Vector3 axis)
		{
			Matrix3x3 result;
			CreateRotation(angle, ref axis, out result);
			return result;
		}

		public static void CreateRotation(float angle, ref Vector3 axis, out Matrix3x3 result)
		{
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			result.M11 = (axis.X * axis.X) * (1.0f - c) + c;
			result.M12 = (axis.X * axis.Y) * (1.0f - c) - (axis.Z * s);
			result.M13 = (axis.X * axis.Z) * (1.0f - c) + (axis.Y * s);

			result.M21 = (axis.Y * axis.X) * (1.0f - c) + (axis.Z * s);
			result.M22 = (axis.Y * axis.Y) * (1.0f - c) + c;
			result.M23 = (axis.Y * axis.Z) * (1.0f - c) - (axis.X * s);

			result.M31 = (axis.Z * axis.X) * (1.0f - c) - (axis.Y * s);
			result.M32 = (axis.Z * axis.Y) * (1.0f - c) + (axis.X * s);
			result.M33 = (axis.Z * axis.Z) * (1.0f - c) + c;
		}

		public static Matrix3x3 CreateRotationX(float angle)
		{
			Matrix3x3 result;
			CreateRotationX(angle, out result);
			return result;
		}

		public static void CreateRotationX(float angle, out Matrix3x3 result)
		{
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			result.M11 = 1.0f;
			result.M12 = 0.0f;
			result.M13 = 0.0f;

			result.M21 = 0.0f;
			result.M22 = c;
			result.M23 = -s;

			result.M31 = 0.0f;
			result.M32 = s;
			result.M33 = c;
		}

		public static Matrix3x3 CreateRotationY(float angle)
		{
			Matrix3x3 result;
			CreateRotationY(angle, out result);
			return result;
		}

		public static void CreateRotationY(float angle, out Matrix3x3 result)
		{
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			result.M11 = c;
			result.M12 = 0.0f;
			result.M13 = s;

			result.M21 = 0.0f;
			result.M22 = 1.0f;
			result.M23 = 0.0f;

			result.M31 = -s;
			result.M32 = 0.0f;
			result.M33 = c;
		}

		public static Matrix3x3 CreateRotationZ(float angle)
		{
			Matrix3x3 result;
			CreateRotationZ(angle, out result);
			return result;
		}

		public static void CreateRotationZ(float angle, out Matrix3x3 result)
		{
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			result.M11 = c;
			result.M12 = -s;
			result.M13 = 0.0f;

			result.M21 = s;
			result.M22 = c;
			result.M23 = 0.0f;

			result.M31 = 0.0f;
			result.M32 = 0.0f;
			result.M33 = 1.0f;
		}

		public static Matrix3x3 Inverse(Matrix3x3 m)
		{
			Matrix3x3 result;
			Inverse(ref m, out result);
			return result;
		}

		public static void Inverse(ref Matrix3x3 m, out Matrix3x3 result)
		{
			float d = m.Determinant;
			if (MathHelpers.IsCloseEnough(d, 0.0f))
				result = Matrix3x3.Identity;
			else
			{
				Matrix3x3 temp;

				d = 1.0f / d;

				temp.M11 = d * (m.M22 * m.M33 - m.M32 * m.M23);
				temp.M21 = d * (m.M31 * m.M23 - m.M21 * m.M33);
				temp.M31 = d * (m.M21 * m.M32 - m.M31 * m.M22);

				temp.M12 = d * (m.M32 * m.M13 - m.M12 * m.M33);
				temp.M22 = d * (m.M11 * m.M33 - m.M31 * m.M13);
				temp.M32 = d * (m.M31 * m.M12 - m.M11 * m.M32);

				temp.M13 = d * (m.M12 * m.M23 - m.M22 * m.M13);
				temp.M23 = d * (m.M21 * m.M13 - m.M11 * m.M23);
				temp.M33 = d * (m.M11 * m.M22 - m.M21 * m.M12);

				result = temp;
			}
		}

		public static Matrix3x3 Transpose(Matrix3x3 m)
		{
			Matrix3x3 result;
			Transpose(ref m, out result);
			return result;
		}

		public static void Transpose(ref Matrix3x3 m, out Matrix3x3 result)
		{
			Matrix3x3 temp;

			temp.M11 = m.M11;
			temp.M12 = m.M21;
			temp.M13 = m.M31;

			temp.M21 = m.M12;
			temp.M22 = m.M22;
			temp.M23 = m.M32;

			temp.M31 = m.M13;
			temp.M32 = m.M23;
			temp.M33 = m.M33;

			result = temp;
		}

		public static Vector3 Transform(Matrix3x3 m, Vector3 v)
		{
			Vector3 result;
			Transform(ref m, ref v, out result);
			return result;
		}

		public static void Transform(ref Matrix3x3 m, ref Vector3 v, out Vector3 result)
		{
			Vector3 temp;

			temp.X = v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13;
			temp.Y = v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23;
			temp.Z = v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33;

			result = temp;
		}

		public static Matrix3x3 Add(Matrix3x3 left, Matrix3x3 right)
		{
			Matrix3x3 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Matrix3x3 left, ref Matrix3x3 right, out Matrix3x3 result)
		{
			result.M11 = left.M11 + right.M11; 
			result.M12 = left.M12 + right.M12;
			result.M13 = left.M13 + right.M13;
			result.M21 = left.M21 + right.M21;
			result.M22 = left.M22 + right.M22;
			result.M23 = left.M23 + right.M23;
			result.M31 = left.M31 + right.M31;
			result.M32 = left.M32 + right.M32;
			result.M33 = left.M33 + right.M33;
		}

		public static Matrix3x3 Subtract(Matrix3x3 left, Matrix3x3 right)
		{
			Matrix3x3 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Matrix3x3 left, ref Matrix3x3 right, out Matrix3x3 result)
		{
			result.M11 = left.M11 - right.M11;
			result.M12 = left.M12 - right.M12;
			result.M13 = left.M13 - right.M13;
			result.M21 = left.M21 - right.M21;
			result.M22 = left.M22 - right.M22;
			result.M23 = left.M23 - right.M23;
			result.M31 = left.M31 - right.M31;
			result.M32 = left.M32 - right.M32;
			result.M33 = left.M33 - right.M33;
		}

		public static Matrix3x3 Multiply(Matrix3x3 left, Matrix3x3 right)
		{
			Matrix3x3 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static void Multiply(ref Matrix3x3 left, ref Matrix3x3 right, out Matrix3x3 result)
		{
			Matrix3x3 m;

			m.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31;
			m.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32;
			m.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33;

			m.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31;
			m.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32;
			m.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33;

			m.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31;
			m.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32;
			m.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33;

			result = m;
		}

		public static Matrix3x3 Multiply(Matrix3x3 left, float right)
		{
			Matrix3x3 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static void Multiply(ref Matrix3x3 left, float right, out Matrix3x3 result)
		{
			result.M11 = left.M11 * right;
			result.M12 = left.M12 * right;
			result.M13 = left.M13 * right;
			result.M21 = left.M21 * right;
			result.M22 = left.M22 * right;
			result.M23 = left.M23 * right;
			result.M31 = left.M31 * right;
			result.M32 = left.M32 * right;
			result.M33 = left.M33 * right;
		}

		public static Matrix3x3 Divide(Matrix3x3 left, Matrix3x3 right)
		{
			Matrix3x3 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static void Divide(ref Matrix3x3 left, ref Matrix3x3 right, out Matrix3x3 result)
		{
			result.M11 = left.M11 / right.M11; 
			result.M12 = left.M12 / right.M12;
			result.M13 = left.M13 / right.M13;
			result.M21 = left.M21 / right.M21;
			result.M22 = left.M22 / right.M22;
			result.M23 = left.M23 / right.M23;
			result.M31 = left.M31 / right.M31;
			result.M32 = left.M32 / right.M32;
			result.M33 = left.M33 / right.M33;
		}

		public static Matrix3x3 Divide(Matrix3x3 left, float right)
		{
			Matrix3x3 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static void Divide(ref Matrix3x3 left, float right, out Matrix3x3 result)
		{
			result.M11 = left.M11 / right;
			result.M12 = left.M12 / right;
			result.M13 = left.M13 / right;
			result.M21 = left.M21 / right;
			result.M22 = left.M22 / right;
			result.M23 = left.M23 / right;
			result.M31 = left.M31 / right;
			result.M32 = left.M32 / right;
			result.M33 = left.M33 / right;
		}

		public static Matrix3x3 operator +(Matrix3x3 left, Matrix3x3 right)
		{
			Matrix3x3 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static Matrix3x3 operator -(Matrix3x3 left, Matrix3x3 right)
		{
			Matrix3x3 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static Matrix3x3 operator *(Matrix3x3 left, Matrix3x3 right)
		{
			Matrix3x3 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Matrix3x3 operator *(Matrix3x3 left, float right)
		{
			Matrix3x3 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static Matrix3x3 operator /(Matrix3x3 left, Matrix3x3 right)
		{
			Matrix3x3 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static Matrix3x3 operator /(Matrix3x3 left, float right)
		{
			Matrix3x3 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static bool operator ==(Matrix3x3 left, Matrix3x3 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Matrix3x3 left, Matrix3x3 right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Matrix3x3)
				return this.Equals((Matrix3x3)obj);
			else
				return false;
		}

		public bool Equals(Matrix3x3 other)
		{
			return (
				M11 == other.M11 &&
				M12 == other.M12 &&
				M13 == other.M13 &&
				M21 == other.M21 &&
				M22 == other.M22 &&
				M23 == other.M23 &&
				M31 == other.M31 &&
				M32 == other.M32 &&
				M33 == other.M33
				);
		}

		public override int GetHashCode()
		{
			return (
				M11.GetHashCode() ^ 
				M12.GetHashCode() ^ 
				M13.GetHashCode() ^ 
				M21.GetHashCode() ^ 
				M22.GetHashCode() ^ 
				M23.GetHashCode() ^ 
				M31.GetHashCode() ^ 
				M32.GetHashCode() ^ 
				M33.GetHashCode()
				);
		}

		public override string ToString()
		{
			return String.Format(
				"{{{0}, {1}, {2}\n  {3}, {4}, {5}\n  {6}, {7}, {8}}}",
				M11, M12, M13, M21, M22, M23, M31, M32, M33
				);
		}
	}
}
