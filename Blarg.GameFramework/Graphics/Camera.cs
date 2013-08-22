using System;

namespace Blarg.GameFramework.Graphics
{
	public class Camera
	{
		ViewContext _viewContext;
		float _nearHeight;
		float _nearWidth;

		public readonly Frustum Frustum;
		public Matrix4x4 LookAt;
		public Matrix4x4 Projection;
		public Vector3 Forward;
		public Vector3 Up;

		public Vector3 Orientation;
		public Vector3 Position;

		public int ViewportWidth { get; private set; }
		public int ViewportHeight { get; private set; }
		public float AspectRatio { get; private set; }
		public float Near { get; private set; }
		public float Far { get; private set; }
		public float FieldOfViewAngle { get; private set; }

		public Camera(ViewContext viewContext, float near = 1.0f, float far = 50.0f, float fieldOfView = MathConstants.Radians60)
		{
			if (viewContext == null)
				throw new ArgumentNullException("viewContext");

			_viewContext = viewContext;
			Frustum = new Frustum(_viewContext);
			Position = Vector3.Zero;
			Orientation = Vector3.Zero;
			Forward = Vector3.Zero;
			Up = Vector3.Up;
			LookAt = Matrix4x4.Identity;

			FieldOfViewAngle = fieldOfView;
			Near = near;
			Far = far;

			CalculateDefaultProjection(
				_viewContext.ViewportLeft, 
				_viewContext.ViewportTop, 
				_viewContext.ViewportRight, 
				_viewContext.ViewportBottom
				);
		}

		public virtual void OnUpdate(float delta)
		{
		}

		public virtual void OnRender(float delta)
		{
			Vector3 movement = Vector3.Zero;
			UpdateLookAtMatrix(ref movement);
			_viewContext.ModelViewMatrix = LookAt;
			Frustum.Calculate();
		}

		public virtual void OnResize(ref Rect size)
		{
			CalculateDefaultProjection(size.Left, size.Top, size.Right, size.Bottom);
			_viewContext.ProjectionMatrix = Projection;
		}

		public virtual void UpdateProjectionMatrix()
		{
			CalculateDefaultProjection(
				_viewContext.ViewportLeft,
				_viewContext.ViewportTop,
				_viewContext.ViewportRight,
				_viewContext.ViewportBottom
				);
		}

		public virtual void UpdateLookAtMatrix(ref Vector3 movement)
		{
			CalculateDefaultLookAt(ref movement);
		}

		public Ray Pick(int screenX, int screenY)
		{
			float nx = 2.0f * ((float)(screenX - (_viewContext.ViewportWidth / 2))) / ((float)_viewContext.ViewportWidth);
			float ny = 2.0f * -((float)(screenY - (_viewContext.ViewportHeight / 2))) / ((float)_viewContext.ViewportHeight);

			// pick ray calculation method copied from http://code.google.com/p/libgdx/
			Vector3 vz = Vector3.Normalize(Forward * -1.0f);
			Vector3 vx = Vector3.Normalize(Vector3.Cross(Vector3.Up, vz));
			Vector3 vy = Vector3.Normalize(Vector3.Cross(vz, vx));

			Vector3 near_center = Position - (vz * Near);

			Vector3 a = (vx * _nearWidth) * nx;
			Vector3 b = (vy * _nearHeight) * ny;
			Vector3 near_point = a + b + near_center;

			Vector3 dir = Vector3.Normalize(near_point - Position);

			return new Ray(Position, dir);
		}

		public Point2 Project(ref Vector3 objectPosition)
		{
			Matrix4x4 modelview = _viewContext.ModelViewMatrix;
			Matrix4x4 projection = _viewContext.ProjectionMatrix;

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
			result.X = (int)(((transformedX * 0.5f) + 0.5f) * (float)_viewContext.ViewportWidth + (float)_viewContext.ViewportLeft);
			result.Y = (int)(((transformedY * 0.5f) + 0.5f) * (float)_viewContext.ViewportHeight + (float)_viewContext.ViewportTop);
			// float z = (1.0f + transformedZ) * 0.5f;   // would be between 0.0f and 1.0f

			// adjust Y coordinate so that 0 is at the top of the screen instead of the bottom
			result.Y = (int)_viewContext.ViewportHeight - result.Y;

			return result; 
		}

		protected void CalculateDefaultProjection(int left, int top, int right, int bottom)
		{
			ViewportWidth = right - left;
			ViewportHeight = bottom - top;

			AspectRatio = (float)ViewportWidth / (float)ViewportHeight;

			_nearHeight = Near * (float)Math.Tan(FieldOfViewAngle / 2.0f);
			_nearWidth = _nearHeight * AspectRatio;

			Projection = Matrix4x4.CreatePerspectiveFieldOfView(FieldOfViewAngle, AspectRatio, Near, Far);
		}

		protected void CalculateDefaultLookAt(ref Vector3 movement)
		{
			// final camera orientation. angles must be negative (or rather, inverted) for the camera matrix. also the matrix concatenation order is important!
			Matrix4x4 rotation = Matrix4x4.CreateRotationY(-Orientation.Y) * Matrix4x4.CreateRotationX(-Orientation.X);

			// apply orientation to forward, movement and up vectors so they're pointing in the right direction
			Forward = Matrix4x4.Transform(rotation, Vector3.Forward);
			Up = Matrix4x4.Transform(rotation, Vector3.Up);
			Vector3 orientedMovement = Matrix4x4.Transform(rotation, movement);

			// move the camera position
			Position += orientedMovement;

			Vector3 target = Forward + Position;
			LookAt = Matrix4x4.CreateLookAt(Position, target, Vector3.Up);
		}
	}
}
