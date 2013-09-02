using System;

namespace Blarg.GameFramework.Graphics
{
	public class PerspectiveCamera : Camera
	{
		public float FieldOfViewAngle;

		public PerspectiveCamera(ViewContext viewContext, float near = 1.0f, float far = 50.0f, float fieldOfView = MathConstants.Radians60)
			: base(viewContext, near, far)
		{
			FieldOfViewAngle = fieldOfView;
		}

		public override void Update()
		{
			CalculateProjection();
			CalculateLookAt();
		}

		protected void CalculateLookAt()
		{
			var target = Position + Direction;
			Matrix4x4.CreateLookAt(ref Position, ref target, ref Up, out LookAt);
			ViewContext.ModelViewMatrix = LookAt;
		}

		protected void CalculateProjection()
		{
			ViewportWidth = ViewContext.ViewportRight - ViewContext.ViewportLeft;
			ViewportHeight = ViewContext.ViewportBottom - ViewContext.ViewportTop;

			float aspectRatio = (float)ViewportWidth / (float)ViewportHeight;

			NearHeight = Near * (float)Math.Tan(FieldOfViewAngle / 2.0f);
			NearWidth = NearHeight * aspectRatio;

			Matrix4x4.CreatePerspectiveFieldOfView(FieldOfViewAngle, aspectRatio, Near, Far, out Projection);
			ViewContext.ProjectionMatrix = Projection;
		}
	}
}

