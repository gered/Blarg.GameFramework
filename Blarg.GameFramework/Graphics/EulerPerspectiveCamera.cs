using System;
using Blarg.GameFramework.TileMap;

namespace Blarg.GameFramework.Graphics
{
	public class EulerPerspectiveCamera : PerspectiveCamera
	{
		public float Yaw;
		public float Pitch;

		public Quaternion Rotation;

		public EulerPerspectiveCamera(ViewContext viewContext, float near = 1.0f, float far = 50.0f, float fieldOfView = MathConstants.Radians60)
			: base(viewContext, near, far, fieldOfView)
		{
		}

		public override void Update()
		{
			UpdateRotation();

			CalculateProjection();
			CalculateLookAt();
		}

		public void UpdateRotation()
		{
			var rotationX = Quaternion.CreateFromAxisAngle(-Pitch, Vector3.XAxis);
			var rotationY = Quaternion.CreateFromAxisAngle(-Yaw, Vector3.YAxis);
			Rotation = rotationY * rotationX;

			Direction = Quaternion.Transform(Rotation, Vector3.Forward);
			Up = Quaternion.Transform(Rotation, Vector3.Up);
		}
	}
}

