using System;

namespace Blarg.GameFramework
{
	public class Transformation
	{
		public Matrix4x4 Transform = Matrix4x4.Identity;

		public void Rotate(float radians, float axisX, float axisY, float axisZ)
		{
			var axis = new Vector3(axisX, axisY, axisZ);
			Matrix4x4 rotation;
			Matrix4x4.CreateRotation(radians, ref axis, out rotation);
			Transform *= rotation;
		}

		public void Rotate(float radians, ref Vector3 axis)
		{
			Matrix4x4 rotation;
			Matrix4x4.CreateRotation(radians, ref axis, out rotation);
			Transform *= rotation;
		}

		public void Scale(float scaleFactor)
		{
			Matrix4x4 scale;
			Matrix4x4.CreateScale(scaleFactor, scaleFactor, scaleFactor, out scale);
			Transform *= scale;
		}

		public void Scale(float x, float y, float z)
		{
			Matrix4x4 scale;
			Matrix4x4.CreateScale(x, y, z, out scale);
			Transform *= scale;
		}

		public void Scale(ref Vector3 v)
		{
			Matrix4x4 scale;
			Matrix4x4.CreateScale(v.X, v.Y, v.Z, out scale);
			Transform *= scale;
		}

		public void Translate(float x, float y, float z)
		{
			Matrix4x4 translation;
			Matrix4x4.CreateTranslation(x, y, z, out translation);
			Transform *= translation;
		}

		public void Translate(ref Vector3 v)
		{
			Matrix4x4 translation;
			Matrix4x4.CreateTranslation(v.X, v.Y, v.Z, out translation);
			Transform *= translation;
		}

		public void Reset()
		{
			Transform = Matrix4x4.Identity;
		}
	}
}
