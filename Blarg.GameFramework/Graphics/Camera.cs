using System;

namespace Blarg.GameFramework.Graphics
{
	public abstract class Camera
	{
		public readonly Frustum Frustum;
		public Matrix4x4 LookAt;
		public Matrix4x4 Projection;
		public Vector3 Position;
		public Vector3 Direction;
		public Vector3 Up;
		public float Near;
		public float Far;

		public int ViewportWidth { get; protected set; }
		public int ViewportHeight { get; protected set; }

		protected readonly ViewContext ViewContext;
		protected float NearWidth;
		protected float NearHeight;

		public Camera(ViewContext viewContext, float near = 1.0f, float far = 50.0f)
		{
			if (viewContext == null)
				throw new ArgumentNullException("viewContext");

			ViewContext = viewContext;
			Frustum = new Frustum(this);
			Position = Vector3.Zero;
			Direction = Vector3.Forward;
			Up = Vector3.Up;
			LookAt = Matrix4x4.Identity;

			Near = near;
			Far = far;

			Update();
		}

		public abstract void Update();

		public Ray Pick(int screenX, int screenY)
		{
			float nx = 2.0f * ((float)(screenX - (ViewContext.ViewportWidth / 2))) / ((float)ViewContext.ViewportWidth);
			float ny = 2.0f * -((float)(screenY - (ViewContext.ViewportHeight / 2))) / ((float)ViewContext.ViewportHeight);

			// pick ray calculation method copied from http://code.google.com/p/libgdx/
			Vector3 vz = Vector3.Normalize(Direction * -1.0f);
			Vector3 vx = Vector3.Normalize(Vector3.Cross(Vector3.Up, vz));
			Vector3 vy = Vector3.Normalize(Vector3.Cross(vz, vx));

			Vector3 near_center = Position - (vz * Near);

			Vector3 a = (vx * NearWidth) * nx;
			Vector3 b = (vy * NearHeight) * ny;
			Vector3 near_point = a + b + near_center;

			Vector3 dir = Vector3.Normalize(near_point - Position);

			return new Ray(Position, dir);
		}

		public Point2 Project(ref Vector3 objectPosition)
		{
			Matrix4x4 modelview = ViewContext.ModelViewMatrix;
			Matrix4x4 projection = ViewContext.ProjectionMatrix;

			return Project(ref objectPosition, ref modelview, ref projection);
		}

		public Point2 Project(ref Vector3 objectPosition, ref Matrix4x4 modelView, ref Matrix4x4 projection)
		{
			// transform object position by modelview matrix (vector transform, w = 1)
			float tempX = objectPosition.X * modelView.M11 + objectPosition.Y * modelView.M12 + objectPosition.Z * modelView.M13 + modelView.M14;
			float tempY = objectPosition.X * modelView.M21 + objectPosition.Y * modelView.M22 + objectPosition.Z * modelView.M23 + modelView.M24;
			float tempZ = objectPosition.X * modelView.M31 + objectPosition.Y * modelView.M32 + objectPosition.Z * modelView.M33 + modelView.M34;
			float tempW = objectPosition.X * modelView.M41 + objectPosition.Y * modelView.M42 + objectPosition.Z * modelView.M43 + modelView.M44;

			// transform the above by the projection matrix (optimized for bottom row of the projection matrix always being [0, 0, -1, 0])
			float transformedX = tempX * projection.M11 + tempY * projection.M12 + tempZ * projection.M13 + tempW * projection.M14;
			float transformedY = tempX * projection.M21 + tempY * projection.M22 + tempZ * projection.M23 + tempW * projection.M24;
			float transformedZ = tempX * projection.M31 + tempY * projection.M32 + tempZ * projection.M33 + tempW * projection.M34;
			float transformedW = -tempZ;

			// w normalizes between -1 and 1
			// TODO: shouldn't really handle this using an assert... however, I'd like to know when/if this happens
			System.Diagnostics.Debug.Assert(transformedW != 0.0f);
			transformedW = 1.0f / transformedW;

			// perspective division
			transformedX *= transformedW;
			transformedY *= transformedW;
			transformedZ *= transformedW;

			// map to 2D viewport coordinates (ignoring Z)
			Point2 result;
			result.X = (int)(((transformedX * 0.5f) + 0.5f) * (float)ViewContext.PixelScaler.ScaledWidth + (float)ViewContext.PixelScaler.ScaledViewport.Left);
			result.Y = (int)(((transformedY * 0.5f) + 0.5f) * (float)ViewContext.PixelScaler.ScaledHeight + (float)ViewContext.PixelScaler.ScaledViewport.Top);
			// float z = (1.0f + transformedZ) * 0.5f;   // would be between 0.0f and 1.0f

			// adjust Y coordinate so that 0 is at the top of the screen instead of the bottom
			result.Y = (int)ViewContext.PixelScaler.ScaledHeight - result.Y;

			return result; 
		}
	}
}
