using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix4x4 : IEquatable<Matrix4x4>
	{
		public float M11;
		public float M21;
		public float M31;
		public float M41;
		public float M12;
		public float M22;
		public float M32;
		public float M42;
		public float M13;
		public float M23;
		public float M33;
		public float M43;
		public float M14;
		public float M24;
		public float M34;
		public float M44;

		const int _11 = 0;
		const int _12 = 4;
		const int _13 = 8;
		const int _14 = 12;
		const int _21 = 1;
		const int _22 = 5;
		const int _23 = 9;
		const int _24 = 13;
		const int _31 = 2;
		const int _32 = 6;
		const int _33 = 10;
		const int _34 = 14;
		const int _41 = 3;
		const int _42 = 7;
		const int _43 = 11;
		const int _44 = 15;

		public float Determinant
		{
			get
			{
				return
					(M11 * M22 - M21 * M12) * 
					(M33 * M44 - M43 * M34) - 
					(M11 * M32 - M31 * M12) * 
					(M23 * M44 - M43 * M24) + 
					(M11 * M42 - M41 * M12) * 
					(M23 * M34 - M33 * M24) + 
					(M21 * M32 - M31 * M22) * 
					(M13 * M44 - M43 * M14) - 
					(M21 * M42 - M41 * M22) * 
					(M13 * M34 - M33 * M14) + 
					(M31 * M42 - M41 * M32) * 
					(M13 * M24 - M23 * M14);
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

		public Vector3 Translation
		{
			get { return new Vector3(M14, M24, M34); }
		}

		public Matrix4x4(
			float m11, float m12, float m13, float m14,
			float m21, float m22, float m23, float m24,
			float m31, float m32, float m33, float m34,
			float m41, float m42, float m43, float m44
			)
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M14 = m14;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M24 = m24;
			M31 = m31;
			M32 = m32;
			M33 = m33;
			M34 = m34;
			M41 = m41;
			M42 = m42;
			M43 = m43;
			M44 = m44;
		}

		public Matrix4x4(float[] m)
		{
			M11 = m[_11];
			M12 = m[_12];
			M13 = m[_13];
			M14 = m[_14];
			M21 = m[_21];
			M22 = m[_22];
			M23 = m[_23];
			M24 = m[_24];
			M31 = m[_31];
			M32 = m[_32];
			M33 = m[_33];
			M34 = m[_34];
			M41 = m[_41];
			M42 = m[_42];
			M43 = m[_43];
			M44 = m[_44];
		}

		public void Set(
			float m11, float m12, float m13, float m14,
			float m21, float m22, float m23, float m24,
			float m31, float m32, float m33, float m34,
			float m41, float m42, float m43, float m44
			)
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M14 = m14;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M24 = m24;
			M31 = m31;
			M32 = m32;
			M33 = m33;
			M34 = m34;
			M41 = m41;
			M42 = m42;
			M43 = m43;
			M44 = m44;
		}

		public void Set(float[] m)
		{
			M11 = m[_11];
			M12 = m[_12];
			M13 = m[_13];
			M14 = m[_14];
			M21 = m[_21];
			M22 = m[_22];
			M23 = m[_23];
			M24 = m[_24];
			M31 = m[_31];
			M32 = m[_32];
			M33 = m[_33];
			M34 = m[_34];
			M41 = m[_41];
			M42 = m[_42];
			M43 = m[_43];
			M44 = m[_44];
		}

		public static readonly Matrix4x4 Identity = new Matrix4x4(
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, 1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, 1.0f, 0.0f,
			0.0f, 0.0f, 0.0f, 1.0f
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

		public Matrix3x3 ToMatrix3x3()
		{
			Matrix3x3 result;
			ToMatrix3x3(out result);
			return result;
		}

		public void ToMatrix3x3(out Matrix3x3 result)
		{
			result.M11 = M11;
			result.M12 = M12;
			result.M13 = M13;

			result.M21 = M21;
			result.M22 = M22;
			result.M23 = M23;

			result.M31 = M31;
			result.M32 = M32;
			result.M33 = M33;
		}

		public void ToArray(float[] result)
		{
			if (result == null || result.Length < 16)
				throw new ArgumentException();

			result[_11] = M11;
			result[_12] = M12;
			result[_13] = M13;
			result[_14] = M14;
			result[_21] = M21;
			result[_22] = M22;
			result[_23] = M23;
			result[_24] = M24;
			result[_31] = M31;
			result[_32] = M32;
			result[_33] = M33;
			result[_34] = M34;
			result[_41] = M41;
			result[_42] = M42;
			result[_43] = M43;
			result[_44] = M44;
		}

		public float[] ToArray()
		{
			var result = new float[16];
			ToArray(result);
			return result;
		}

		public static Matrix4x4 CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUp, Vector3 cameraForward)
		{
			Matrix4x4 result;
			CreateBillboard(ref objectPosition, ref cameraPosition, ref cameraUp, ref cameraForward, out result);
			return result;
		}

		public static void CreateBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraUp, ref Vector3 cameraForward, out Matrix4x4 result)
		{
			Vector3 forward = objectPosition - cameraPosition;
			float forwardLengthSquared = forward.LengthSquared;
			if (forwardLengthSquared < 0.0001f)
				forward = -cameraForward;
			else
				forward = forward * MathHelpers.FastInverseSqrt(forwardLengthSquared);

			Vector3 left = Vector3.Normalize(Vector3.Cross(cameraUp, forward));
			Vector3 up = Vector3.Cross(forward, left);

			result.M11 = left.X;
			result.M21 = left.Y;
			result.M31 = left.Z;
			result.M41 = 0.0f;

			result.M12 = up.X;
			result.M22 = up.Y;
			result.M32 = up.Z;
			result.M42 = 0.0f;

			result.M13 = forward.X;
			result.M23 = forward.Y;
			result.M33 = forward.Z;
			result.M43 = 0.0f;

			result.M14 = objectPosition.X;
			result.M24 = objectPosition.Y;
			result.M34 = objectPosition.Z;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateCylindricalBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraForward, Vector3 axis)
		{
			Matrix4x4 result;
			CreateCylindricalBillboard(ref objectPosition, ref cameraPosition, ref cameraForward, ref axis, out result);
			return result;
		}

		public static void CreateCylindricalBillboard(ref Vector3 objectPosition, ref Vector3 cameraPosition, ref Vector3 cameraForward, ref Vector3 axis, out Matrix4x4 result)
		{
			Vector3 temp = objectPosition - cameraPosition;
			float lengthSquared = temp.LengthSquared;
			if (lengthSquared < 0.0001f)
				temp = -cameraForward;
			else
				temp = temp * MathHelpers.FastInverseSqrt(lengthSquared);

			Vector3 up = axis;
			Vector3 forward;
			Vector3 left;

			float dot = Vector3.Dot(axis, temp);
			if (Math.Abs(dot) > 0.9982547f)
			{
				dot = Vector3.Dot(axis, Vector3.Forward);
				if (Math.Abs(dot) > 0.9982547f)
					forward = Vector3.Right;
				else
					forward = Vector3.Forward;

				left = Vector3.Normalize(Vector3.Cross(axis, forward));
				forward = Vector3.Normalize(Vector3.Cross(left, axis));
			}
			else
			{
				left = Vector3.Normalize(Vector3.Cross(axis, temp));
				forward = Vector3.Normalize(Vector3.Cross(left, up));
			}

			result.M11 = left.X;
			result.M21 = left.Y;
			result.M31 = left.Z;
			result.M41 = 0.0f;

			result.M12 = up.X;
			result.M22 = up.Y;
			result.M32 = up.Z;
			result.M42 = 0.0f;

			result.M13 = forward.X;
			result.M23 = forward.Y;
			result.M33 = forward.Z;
			result.M43 = 0.0f;

			result.M14 = objectPosition.X;
			result.M24 = objectPosition.Y;
			result.M34 = objectPosition.Z;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateScreenAlignedBillboard(Vector3 objectPosition, Vector3 cameraUp, Vector3 cameraForward)
		{
			Matrix4x4 result;
			CreateScreenAlignedBillboard(ref objectPosition, ref cameraUp, ref cameraForward, out result);
			return result;
		}

		public static void CreateScreenAlignedBillboard(ref Vector3 objectPosition, ref Vector3 cameraUp, ref Vector3 cameraForward, out Matrix4x4 result)
		{
			Vector3 left = Vector3.Normalize(Vector3.Cross(cameraUp, cameraForward));
			Vector3 up = Vector3.Cross(cameraForward, left);

			result.M11 = left.X;
			result.M21 = left.Y;
			result.M31 = left.Z;
			result.M41 = 0.0f;

			result.M12 = up.X;
			result.M22 = up.Y;
			result.M32 = up.Z;
			result.M42 = 0.0f;

			result.M13 = cameraForward.X;
			result.M23 = cameraForward.Y;
			result.M33 = cameraForward.Z;
			result.M43 = 0.0f;

			result.M14 = objectPosition.X;
			result.M24 = objectPosition.Y;
			result.M34 = objectPosition.Z;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateScreenAndAxisAlignedBillboard(Vector3 objectPosition, Vector3 cameraForward, Vector3 axis)
		{
			Matrix4x4 result;
			CreateScreenAndAxisAlignedBillboard(ref objectPosition, ref cameraForward, ref axis, out result);
			return result;
		}

		public static void CreateScreenAndAxisAlignedBillboard(ref Vector3 objectPosition, ref Vector3 cameraForward, ref Vector3 axis, out Matrix4x4 result)
		{
			Vector3 up = axis;
			Vector3 forward;
			Vector3 left;

			float dot = Vector3.Dot(axis, cameraForward);
			if (Math.Abs(dot) > 0.9982547f)
			{
				dot = Vector3.Dot(axis, Vector3.Forward);
				if (Math.Abs(dot) > 0.9982547f)
					forward = Vector3.Right;
				else
					forward = Vector3.Forward;

				left = Vector3.Normalize(Vector3.Cross(axis, forward));
				forward = Vector3.Normalize(Vector3.Cross(left, axis));
			}
			else
			{
				left = Vector3.Normalize(Vector3.Cross(axis, cameraForward));
				forward = Vector3.Normalize(Vector3.Cross(left, up));
			}

			result.M11 = left.X;
			result.M21 = left.Y;
			result.M31 = left.Z;
			result.M41 = 0.0f;

			result.M12 = up.X;
			result.M22 = up.Y;
			result.M32 = up.Z;
			result.M42 = 0.0f;

			result.M13 = forward.X;
			result.M23 = forward.Y;
			result.M33 = forward.Z;
			result.M43 = 0.0f;

			result.M14 = objectPosition.X;
			result.M24 = objectPosition.Y;
			result.M34 = objectPosition.Z;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateFromEulerAngles(float x, float y, float z)
		{
			Matrix4x4 result;
			CreateFromEulerAngles(x, y, z, out result);
			return result;
		}

		public static void CreateFromEulerAngles(float x, float y, float z, out Matrix4x4 result)
		{
			Matrix4x4 rotateZ;
			Matrix4x4 rotateY;
			Matrix4x4 rotateX;
			Matrix4x4.CreateRotationZ(z, out rotateZ);
			Matrix4x4.CreateRotationY(y, out rotateY);
			Matrix4x4.CreateRotationX(x, out rotateX);

			result = rotateZ * rotateY * rotateX;
		}

		public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUp)
		{
			Matrix4x4 result;
			CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUp, out result);
			return result;
		}

		public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUp, out Matrix4x4 result)
		{
			// build basic lookat matrix without translation component included
			Vector3 forward = Vector3.Normalize(cameraTarget - cameraPosition);
			Vector3 left = Vector3.Normalize(Vector3.Cross(forward, cameraUp));
			Vector3 up = Vector3.Cross(left, forward);

			result.M11 = left.X;
			result.M21 = up.X;
			result.M31 = -forward.X;
			result.M41 = 0.0f;

			result.M12 = left.Y;
			result.M22 = up.Y;
			result.M32 = -forward.Y;
			result.M42 = 0.0f;

			result.M13 = left.Z;
			result.M23 = up.Z;
			result.M33 = -forward.Z;
			result.M43 = 0.0f;

			result.M14 = 0.0f;
			result.M24 = 0.0f;
			result.M34 = 0.0f;
			result.M44 = 1.0f;

			// multiply the translation into the lookat matrix 
			// this matrix multiplication is simplified so that we're only multiplying components that can actually affect the result
			// out = Matrix4x4::CreateTranslation(-cameraPosition.x, -cameraPosition.y, -cameraPosition.z) * out;
			result.M14 = result.M11 * -cameraPosition.X + result.M12 * -cameraPosition.Y + result.M13 * -cameraPosition.Z + result.M14;
			result.M24 = result.M21 * -cameraPosition.X + result.M22 * -cameraPosition.Y + result.M23 * -cameraPosition.Z + result.M24;
			result.M34 = result.M31 * -cameraPosition.X + result.M32 * -cameraPosition.Y + result.M33 * -cameraPosition.Z + result.M34;
			result.M44 = result.M41 * -cameraPosition.X + result.M42 * -cameraPosition.Y + result.M43 * -cameraPosition.Z + result.M44;
		}

		public static Matrix4x4 CreateOrthographic(float left, float right, float bottom, float top, float near, float far)
		{
			Matrix4x4 result;
			CreateOrthographic(left, right, bottom, top, near, far, out result);
			return result;
		}

		public static void CreateOrthographic(float left, float right, float bottom, float top, float near, float far, out Matrix4x4 result)
		{
			result.M11 = 2.0f / (right - left);
			result.M12 = 0.0f;
			result.M13 = 0.0f;
			result.M14 = -((right + left) / (right - left));

			result.M21 = 0.0f;
			result.M22 = 2.0f / (top - bottom);
			result.M23 = 0.0f;
			result.M24 = -((top + bottom) / (top - bottom));

			result.M31 = 0.0f;
			result.M32 = 0.0f;
			result.M33 = -2.0f / (far - near);
			result.M34 = -((far + near) / (far - near));

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = 0.0f;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreatePerspective(float left, float right, float bottom, float top, float near, float far)
		{
			Matrix4x4 result;
			CreatePerspective(left, right, bottom, top, near, far, out result);
			return result;
		}

		public static void CreatePerspective(float left, float right, float bottom, float top, float near, float far, out Matrix4x4 result)
		{
			result.M11 = (2.0f * near) / (right - left);
			result.M12 = 0.0f;
			result.M13 = (right + left) / (right - left);
			result.M14 = 0.0f;

			result.M21 = 0.0f;
			result.M22 = (2.0f * near) / (top - bottom);
			result.M23 = (top + bottom) / (top - bottom);
			result.M24 = 0.0f;

			result.M31 = 0.0f;
			result.M32 = 0.0f;
			result.M33 = -((far + near)) / (far - near);
			result.M34 = -((2.0f * far * near)) / (far - near);

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = -1.0f;
			result.M44 = 0.0f;
		}

		public static Matrix4x4 CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float near, float far)
		{
			Matrix4x4 result;
			CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, near, far, out result);
			return result;
		}

		public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float near, float far, out Matrix4x4 result)
		{
			float f = 1.0f / (float)Math.Tan(fieldOfView / 2.0f);

			result.M11 = f / aspectRatio;
			result.M12 = 0.0f;
			result.M13 = 0.0f;
			result.M14 = 0.0f;

			result.M21 = 0.0f;
			result.M22 = f;
			result.M23 = 0.0f;
			result.M24 = 0.0f;

			result.M31 = 0.0f;
			result.M32 = 0.0f;
			result.M33 = (far + near) / (near - far);
			result.M34 = (2.0f * far * near) / (near - far);

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = -1.0f;
			result.M44 = 0.0f;
		}

		public static Matrix4x4 CreateRotation(float angle, Vector3 axis)
		{
			Matrix4x4 result;
			CreateRotation(angle, ref axis, out result);
			return result;
		}

		public static void CreateRotation(float angle, ref Vector3 axis, out Matrix4x4 result)
		{
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			result.M11 = (axis.X * axis.X) * (1.0f - c) + c;
			result.M12 = (axis.X * axis.Y) * (1.0f - c) - (axis.Z * s);
			result.M13 = (axis.X * axis.Z) * (1.0f - c) + (axis.Y * s);
			result.M14 = 0.0f;

			result.M21 = (axis.Y * axis.X) * (1.0f - c) + (axis.Z * s);
			result.M22 = (axis.Y * axis.Y) * (1.0f - c) + c;
			result.M23 = (axis.Y * axis.Z) * (1.0f - c) - (axis.X * s);
			result.M24 = 0.0f;

			result.M31 = (axis.Z * axis.X) * (1.0f - c) - (axis.Y * s);
			result.M32 = (axis.Z * axis.Y) * (1.0f - c) + (axis.X * s);
			result.M33 = (axis.Z * axis.Z) * (1.0f - c) + c;
			result.M34 = 0.0f;

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = 0.0f;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateRotationX(float angle)
		{
			Matrix4x4 result;
			CreateRotationX(angle, out result);
			return result;
		}

		public static void CreateRotationX(float angle, out Matrix4x4 result)
		{
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			result.M11 = 1.0f;
			result.M12 = 0.0f;
			result.M13 = 0.0f;
			result.M14 = 0.0f;

			result.M21 = 0.0f;
			result.M22 = c;
			result.M23 = -s;
			result.M24 = 0.0f;

			result.M31 = 0.0f;
			result.M32 = s;
			result.M33 = c;
			result.M34 = 0.0f;

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = 0.0f;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateRotationY(float angle)
		{
			Matrix4x4 result;
			CreateRotationY(angle, out result);
			return result;
		}

		public static void CreateRotationY(float angle, out Matrix4x4 result)
		{
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			result.M11 = c;
			result.M12 = 0.0f;
			result.M13 = s;
			result.M14 = 0.0f;

			result.M21 = 0.0f;
			result.M22 = 1.0f;
			result.M23 = 0.0f;
			result.M24 = 0.0f;

			result.M31 = -s;
			result.M32 = 0.0f;
			result.M33 = c;
			result.M34 = 0.0f;

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = 0.0f;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateRotationZ(float angle)
		{
			Matrix4x4 result;
			CreateRotationZ(angle, out result);
			return result;
		}

		public static void CreateRotationZ(float angle, out Matrix4x4 result)
		{
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			result.M11 = c;
			result.M12 = -s;
			result.M13 = 0.0f;
			result.M14 = 0.0f;

			result.M21 = s;
			result.M22 = c;
			result.M23 = 0.0f;
			result.M24 = 0.0f;

			result.M31 = 0.0f;
			result.M32 = 0.0f;
			result.M33 = 1.0f;
			result.M34 = 0.0f;

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = 0.0f;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateScale(float x, float y, float z)
		{
			Matrix4x4 result;
			CreateScale(x, y, z, out result);
			return result;
		}

		public static void CreateScale(float x, float y, float z, out Matrix4x4 result)
		{
			result.M11 = x;
			result.M12 = 0.0f;
			result.M13 = 0.0f;
			result.M14 = 0.0f;

			result.M21 = 0.0f;
			result.M22 = y;
			result.M23 = 0.0f;
			result.M24 = 0.0f;

			result.M31 = 0.0f;
			result.M32 = 0.0f;
			result.M33 = z;
			result.M34 = 0.0f;

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = 0.0f;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateTranslation(float x, float y, float z)
		{
			Matrix4x4 result;
			CreateTranslation(x, y, z, out result);
			return result;
		}

		public static void CreateTranslation(float x, float y, float z, out Matrix4x4 result)
		{
			result.M11 = 1.0f;
			result.M12 = 0.0f;
			result.M13 = 0.0f;
			result.M14 = x;

			result.M21 = 0.0f;
			result.M22 = 1.0f;
			result.M23 = 0.0f;
			result.M24 = y;

			result.M31 = 0.0f;
			result.M32 = 0.0f;
			result.M33 = 1.0f;
			result.M34 = z;

			result.M41 = 0.0f;
			result.M42 = 0.0f;
			result.M43 = 0.0f;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
		{
			Matrix4x4 result;
			CreateWorld(ref position, ref forward, ref up, out result);
			return result;
		}

		public static void CreateWorld(ref Vector3 position, ref Vector3 forward, ref Vector3 up, out Matrix4x4 result)
		{
			Vector3 f = Vector3.Normalize(-forward);
			Vector3 l = Vector3.Normalize(Vector3.Cross(up, f));
			Vector3 u = Vector3.Cross(f, l);

			result.M11 = l.X;
			result.M21 = l.Y;
			result.M31 = l.Z;
			result.M41 = 0.0f;

			result.M12 = u.X;
			result.M22 = u.Y;
			result.M32 = u.Z;
			result.M42 = 0.0f;

			result.M13 = f.X;
			result.M23 = f.Y;
			result.M33 = f.Z;
			result.M43 = 0.0f;

			result.M14 = position.X;
			result.M24 = position.Y;
			result.M34 = position.Z;
			result.M44 = 1.0f;
		}

		public static Matrix4x4 Inverse(Matrix4x4 m)
		{
			Matrix4x4 result;
			Inverse(ref m, out result);
			return result;
		}

		public static void Inverse(ref Matrix4x4 m, out Matrix4x4 result)
		{
			float d = m.Determinant;
			if (MathHelpers.IsCloseEnough(d, 0.0f))
				result = Matrix4x4.Identity;
			else
			{
				d = 1.0f / d;

				Matrix4x4 temp;

				temp.M11 = d * (m.M22 * (m.M33 * m.M44 - m.M43 * m.M34) + m.M32 * (m.M43 * m.M24 - m.M23 * m.M44) + m.M42 * (m.M23 * m.M34 - m.M33 * m.M24));
				temp.M21 = d * (m.M23 * (m.M31 * m.M44 - m.M41 * m.M34) + m.M33 * (m.M41 * m.M24 - m.M21 * m.M44) + m.M43 * (m.M21 * m.M34 - m.M31 * m.M24));
				temp.M31 = d * (m.M24 * (m.M31 * m.M42 - m.M41 * m.M32) + m.M34 * (m.M41 * m.M22 - m.M21 * m.M42) + m.M44 * (m.M21 * m.M32 - m.M31 * m.M22));
				temp.M41 = d * (m.M21 * (m.M42 * m.M33 - m.M32 * m.M43) + m.M31 * (m.M22 * m.M43 - m.M42 * m.M23) + m.M41 * (m.M32 * m.M23 - m.M22 * m.M33));

				temp.M12 = d * (m.M32 * (m.M13 * m.M44 - m.M43 * m.M14) + m.M42 * (m.M33 * m.M14 - m.M13 * m.M34) + m.M12 * (m.M43 * m.M34 - m.M33 * m.M44));
				temp.M22 = d * (m.M33 * (m.M11 * m.M44 - m.M41 * m.M14) + m.M43 * (m.M31 * m.M14 - m.M11 * m.M34) + m.M13 * (m.M41 * m.M34 - m.M31 * m.M44));
				temp.M32 = d * (m.M34 * (m.M11 * m.M42 - m.M41 * m.M12) + m.M44 * (m.M31 * m.M12 - m.M11 * m.M32) + m.M14 * (m.M41 * m.M32 - m.M31 * m.M42));
				temp.M42 = d * (m.M31 * (m.M42 * m.M13 - m.M12 * m.M43) + m.M41 * (m.M12 * m.M33 - m.M32 * m.M13) + m.M11 * (m.M32 * m.M43 - m.M42 * m.M33));

				temp.M13 = d * (m.M42 * (m.M13 * m.M24 - m.M23 * m.M14) + m.M12 * (m.M23 * m.M44 - m.M43 * m.M24) + m.M22 * (m.M43 * m.M14 - m.M13 * m.M44));
				temp.M23 = d * (m.M43 * (m.M11 * m.M24 - m.M21 * m.M14) + m.M13 * (m.M21 * m.M44 - m.M41 * m.M24) + m.M23 * (m.M41 * m.M14 - m.M11 * m.M44));
				temp.M33 = d * (m.M44 * (m.M11 * m.M22 - m.M21 * m.M12) + m.M14 * (m.M21 * m.M42 - m.M41 * m.M22) + m.M24 * (m.M41 * m.M12 - m.M11 * m.M42));
				temp.M43 = d * (m.M41 * (m.M22 * m.M13 - m.M12 * m.M23) + m.M11 * (m.M42 * m.M23 - m.M22 * m.M43) + m.M21 * (m.M12 * m.M43 - m.M42 * m.M13));

				temp.M14 = d * (m.M12 * (m.M33 * m.M24 - m.M23 * m.M34) + m.M22 * (m.M13 * m.M34 - m.M33 * m.M14) + m.M32 * (m.M23 * m.M14 - m.M13 * m.M24));
				temp.M24 = d * (m.M13 * (m.M31 * m.M24 - m.M21 * m.M34) + m.M23 * (m.M11 * m.M34 - m.M31 * m.M14) + m.M33 * (m.M21 * m.M14 - m.M11 * m.M24));
				temp.M34 = d * (m.M14 * (m.M31 * m.M22 - m.M21 * m.M32) + m.M24 * (m.M11 * m.M32 - m.M31 * m.M12) + m.M34 * (m.M21 * m.M12 - m.M11 * m.M22));
				temp.M44 = d * (m.M11 * (m.M22 * m.M33 - m.M32 * m.M23) + m.M21 * (m.M32 * m.M13 - m.M12 * m.M33) + m.M31 * (m.M12 * m.M23 - m.M22 * m.M13));

				result = temp;
			}
		}

		public static Matrix4x4 Transpose(Matrix4x4 m)
		{
			Matrix4x4 result;
			Transpose(ref m, out result);
			return result;
		}

		public static void Transpose(ref Matrix4x4 m, out Matrix4x4 result)
		{
			Matrix4x4 temp;

			temp.M11 = m.M11;
			temp.M12 = m.M21;
			temp.M13 = m.M31;
			temp.M14 = m.M41;

			temp.M21 = m.M12;
			temp.M22 = m.M22;
			temp.M23 = m.M32;
			temp.M24 = m.M42;

			temp.M31 = m.M13;
			temp.M32 = m.M23;
			temp.M33 = m.M33;
			temp.M34 = m.M43;

			temp.M41 = m.M14;
			temp.M42 = m.M24;
			temp.M43 = m.M34;
			temp.M44 = m.M44;

			result = temp;
		}

		public static Vector3 Transform(Matrix4x4 m, Vector3 v)
		{
			Vector3 result;
			Transform(ref m, ref v, out result);
			return result;
		}

		public static void Transform(ref Matrix4x4 m, ref Vector3 v, out Vector3 result)
		{
			Vector3 temp;
			temp.X = v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13 + m.M14;
			temp.Y = v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23 + m.M24;
			temp.Z = v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33 + m.M34;
			result = temp;
		}

		public static Vector4 Transform(Matrix4x4 m, Vector4 v)
		{
			Vector4 result;
			Transform(ref m, ref v, out result);
			return result;
		}

		public static void Transform(ref Matrix4x4 m, ref Vector4 v, out Vector4 result)
		{
			Vector4 temp;
			temp.X = v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13 + v.W * m.M14;
			temp.Y = v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23 + v.W * m.M24;
			temp.Z = v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33 + v.W * m.M34;
			temp.W = v.X * m.M41 + v.Y * m.M42 + v.Z * m.M43 + v.W * m.M44;
			result = temp;
		}

		public static Vector3 TransformNormal(Matrix4x4 m, Vector3 v)
		{
			Vector3 result;
			TransformNormal(ref m, ref v, out result);
			return result;
		}

		public static void TransformNormal(ref Matrix4x4 m, ref Vector3 v, out Vector3 result)
		{
			Vector3 temp;
			temp.X = v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13;
			temp.Y = v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23;
			temp.Z = v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33;
			result = temp;
		}

		public static Matrix4x4 Add(Matrix4x4 left, Matrix4x4 right)
		{
			Matrix4x4 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static void Add(ref Matrix4x4 left, ref Matrix4x4 right, out Matrix4x4 result)
		{
			result.M11 = left.M11 + right.M11; 
			result.M12 = left.M12 + right.M12;
			result.M13 = left.M13 + right.M13;
			result.M14 = left.M14 + right.M14;
			result.M21 = left.M21 + right.M21;
			result.M22 = left.M22 + right.M22;
			result.M23 = left.M23 + right.M23;
			result.M24 = left.M24 + right.M24;
			result.M31 = left.M31 + right.M31;
			result.M32 = left.M32 + right.M32;
			result.M33 = left.M33 + right.M33;
			result.M34 = left.M34 + right.M34;
			result.M41 = left.M41 + right.M41;
			result.M42 = left.M42 + right.M42;
			result.M43 = left.M43 + right.M43;
			result.M44 = left.M44 + right.M44;
		}

		public static Matrix4x4 Subtract(Matrix4x4 left, Matrix4x4 right)
		{
			Matrix4x4 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static void Subtract(ref Matrix4x4 left, ref Matrix4x4 right, out Matrix4x4 result)
		{
			result.M11 = left.M11 - right.M11; 
			result.M12 = left.M12 - right.M12;
			result.M13 = left.M13 - right.M13;
			result.M14 = left.M14 - right.M14;
			result.M21 = left.M21 - right.M21;
			result.M22 = left.M22 - right.M22;
			result.M23 = left.M23 - right.M23;
			result.M24 = left.M24 - right.M24;
			result.M31 = left.M31 - right.M31;
			result.M32 = left.M32 - right.M32;
			result.M33 = left.M33 - right.M33;
			result.M34 = left.M34 - right.M34;
			result.M41 = left.M41 - right.M41;
			result.M42 = left.M42 - right.M42;
			result.M43 = left.M43 - right.M43;
			result.M44 = left.M44 - right.M44;
		}

		public static Matrix4x4 Multiply(Matrix4x4 left, Matrix4x4 right)
		{
			Matrix4x4 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static void Multiply(ref Matrix4x4 left, ref Matrix4x4 right, out Matrix4x4 result)
		{
			Matrix4x4 m;

			m.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
			m.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
			m.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
			m.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

			m.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
			m.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
			m.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
			m.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

			m.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
			m.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
			m.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
			m.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

			m.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
			m.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
			m.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
			m.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;

			result = m;
		}

		public static Matrix4x4 Multiply(Matrix4x4 left, float right)
		{
			Matrix4x4 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static void Multiply(ref Matrix4x4 left, float right, out Matrix4x4 result)
		{
			result.M11 = left.M11 * right;
			result.M12 = left.M12 * right;
			result.M13 = left.M13 * right;
			result.M14 = left.M14 * right;
			result.M21 = left.M21 * right;
			result.M22 = left.M22 * right;
			result.M23 = left.M23 * right;
			result.M24 = left.M24 * right;
			result.M31 = left.M31 * right;
			result.M32 = left.M32 * right;
			result.M33 = left.M33 * right;
			result.M34 = left.M34 * right;
			result.M41 = left.M41 * right;
			result.M42 = left.M42 * right;
			result.M43 = left.M43 * right;
			result.M44 = left.M44 * right;
		}

		public static Matrix4x4 Divide(Matrix4x4 left, Matrix4x4 right)
		{
			Matrix4x4 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static void Divide(ref Matrix4x4 left, ref Matrix4x4 right, out Matrix4x4 result)
		{
			result.M11 = left.M11 / right.M11; 
			result.M12 = left.M12 / right.M12;
			result.M13 = left.M13 / right.M13;
			result.M14 = left.M14 / right.M14;
			result.M21 = left.M21 / right.M21;
			result.M22 = left.M22 / right.M22;
			result.M23 = left.M23 / right.M23;
			result.M24 = left.M24 / right.M24;
			result.M31 = left.M31 / right.M31;
			result.M32 = left.M32 / right.M32;
			result.M33 = left.M33 / right.M33;
			result.M34 = left.M34 / right.M34;
			result.M41 = left.M41 / right.M41;
			result.M42 = left.M42 / right.M42;
			result.M43 = left.M43 / right.M43;
			result.M44 = left.M44 / right.M44;
		}

		public static Matrix4x4 Divide(Matrix4x4 left, float right)
		{
			Matrix4x4 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static void Divide(ref Matrix4x4 left, float right, out Matrix4x4 result)
		{
			result.M11 = left.M11 / right;
			result.M12 = left.M12 / right;
			result.M13 = left.M13 / right;
			result.M14 = left.M14 / right;
			result.M21 = left.M21 / right;
			result.M22 = left.M22 / right;
			result.M23 = left.M23 / right;
			result.M24 = left.M24 / right;
			result.M31 = left.M31 / right;
			result.M32 = left.M32 / right;
			result.M33 = left.M33 / right;
			result.M34 = left.M34 / right;
			result.M41 = left.M41 / right;
			result.M42 = left.M42 / right;
			result.M43 = left.M43 / right;
			result.M44 = left.M44 / right;
		}

		public static Matrix4x4 operator +(Matrix4x4 left, Matrix4x4 right)
		{
			Matrix4x4 result;
			Add(ref left, ref right, out result);
			return result;
		}

		public static Matrix4x4 operator -(Matrix4x4 left, Matrix4x4 right)
		{
			Matrix4x4 result;
			Subtract(ref left, ref right, out result);
			return result;
		}

		public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
		{
			Matrix4x4 result;
			Multiply(ref left, ref right, out result);
			return result;
		}

		public static Matrix4x4 operator *(Matrix4x4 left, float right)
		{
			Matrix4x4 result;
			Multiply(ref left, right, out result);
			return result;
		}

		public static Matrix4x4 operator /(Matrix4x4 left, Matrix4x4 right)
		{
			Matrix4x4 result;
			Divide(ref left, ref right, out result);
			return result;
		}

		public static Matrix4x4 operator /(Matrix4x4 left, float right)
		{
			Matrix4x4 result;
			Divide(ref left, right, out result);
			return result;
		}

		public static bool operator ==(Matrix4x4 left, Matrix4x4 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Matrix4x4 left, Matrix4x4 right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Matrix4x4)
				return this.Equals((Matrix4x4)obj);
			else
				return false;
		}

		public bool Equals(Matrix4x4 other)
		{
			return (
				M11 == other.M11 &&
				M12 == other.M12 &&
				M13 == other.M13 &&
				M14 == other.M14 &&
				M21 == other.M21 &&
				M22 == other.M22 &&
				M23 == other.M23 &&
				M24 == other.M24 &&
				M31 == other.M31 &&
				M32 == other.M32 &&
				M33 == other.M33 &&
				M34 == other.M34 &&
				M41 == other.M41 &&
				M42 == other.M42 &&
				M43 == other.M43 &&
				M44 == other.M44
				);
		}

		public override int GetHashCode()
		{
			return (
				M11.GetHashCode() ^ 
				M12.GetHashCode() ^ 
				M13.GetHashCode() ^ 
				M14.GetHashCode() ^ 
				M21.GetHashCode() ^ 
				M22.GetHashCode() ^ 
				M23.GetHashCode() ^ 
				M24.GetHashCode() ^ 
				M31.GetHashCode() ^ 
				M32.GetHashCode() ^ 
				M33.GetHashCode() ^ 
				M34.GetHashCode() ^ 
				M41.GetHashCode() ^ 
				M42.GetHashCode() ^ 
				M43.GetHashCode() ^ 
				M44.GetHashCode()
				);
		}

		public override string ToString()
		{
			return String.Format(
				"{{{0}, {1}, {2}, {3}\n  {4}, {5}, {6}, {7}\n  {8}, {9}, {10}, {11}\n  {12}, {13}, {14}, {15}}}",
				M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44
				);
		}
	}
}
