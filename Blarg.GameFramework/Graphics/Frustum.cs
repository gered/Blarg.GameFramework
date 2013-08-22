using System;

namespace Blarg.GameFramework.Graphics
{
	public enum FrustumSides
	{
		Right = 0,
		Left = 1,
		Bottom = 2,
		Top = 3,
		Back = 4,
		Front = 5
	}

	public class Frustum
	{
		ViewContext _viewContext;
		Plane[] _planes = new Plane[6];

		public Frustum(ViewContext viewContext)
		{
			if (viewContext == null)
				throw new ArgumentNullException("viewContext");

			_viewContext = viewContext;
			Calculate();
		}

		public void Calculate()
		{
			Matrix4x4 combined = _viewContext.ProjectionMatrix * _viewContext.ModelViewMatrix;

			// Extract the sides of each of the 6 planes from this to get our viewing frustum
			_planes[(int)FrustumSides.Right].Normal.X = combined.M41 - combined.M11;
			_planes[(int)FrustumSides.Right].Normal.Y = combined.M42 - combined.M12;
			_planes[(int)FrustumSides.Right].Normal.Z = combined.M43 - combined.M13;
			_planes[(int)FrustumSides.Right].D = combined.M44 - combined.M14;

			_planes[(int)FrustumSides.Left].Normal.X = combined.M41 + combined.M11;
			_planes[(int)FrustumSides.Left].Normal.Y = combined.M42 + combined.M12;
			_planes[(int)FrustumSides.Left].Normal.Z = combined.M43 + combined.M13;
			_planes[(int)FrustumSides.Left].D = combined.M44 + combined.M14;

			_planes[(int)FrustumSides.Bottom].Normal.X = combined.M41 + combined.M21;
			_planes[(int)FrustumSides.Bottom].Normal.Y = combined.M42 + combined.M22;
			_planes[(int)FrustumSides.Bottom].Normal.Z = combined.M43 + combined.M23;
			_planes[(int)FrustumSides.Bottom].D = combined.M44 + combined.M24;

			_planes[(int)FrustumSides.Top].Normal.X = combined.M41 - combined.M21;
			_planes[(int)FrustumSides.Top].Normal.Y = combined.M42 - combined.M22;
			_planes[(int)FrustumSides.Top].Normal.Z = combined.M43 - combined.M23;
			_planes[(int)FrustumSides.Top].D = combined.M44 - combined.M24;

			_planes[(int)FrustumSides.Back].Normal.X = combined.M41 - combined.M31;
			_planes[(int)FrustumSides.Back].Normal.Y = combined.M42 - combined.M32;
			_planes[(int)FrustumSides.Back].Normal.Z = combined.M43 - combined.M33;
			_planes[(int)FrustumSides.Back].D = combined.M44 - combined.M34;

			_planes[(int)FrustumSides.Front].Normal.X = combined.M41 + combined.M31;
			_planes[(int)FrustumSides.Front].Normal.Y = combined.M42 + combined.M32;
			_planes[(int)FrustumSides.Front].Normal.Z = combined.M43 + combined.M33;
			_planes[(int)FrustumSides.Front].D = combined.M44 + combined.M34;

			Plane.Normalize(ref _planes[(int)FrustumSides.Right], out _planes[(int)FrustumSides.Right]);
			Plane.Normalize(ref _planes[(int)FrustumSides.Left], out _planes[(int)FrustumSides.Left]);
			Plane.Normalize(ref _planes[(int)FrustumSides.Bottom], out _planes[(int)FrustumSides.Bottom]);
			Plane.Normalize(ref _planes[(int)FrustumSides.Top], out _planes[(int)FrustumSides.Top]);
			Plane.Normalize(ref _planes[(int)FrustumSides.Back], out _planes[(int)FrustumSides.Back]);
			Plane.Normalize(ref _planes[(int)FrustumSides.Front], out _planes[(int)FrustumSides.Front]);
		}

		public bool Test(ref Vector3 point)
		{
			for (int p = 0; p < 6; ++p)
			{
				if (Plane.ClassifyPoint(ref _planes[p], ref point) == PlanePointClassify.Behind)
					return false;
			}

			return true;
		}

		public bool Test(ref BoundingBox box)
		{
			if (!TestPlaneAgainstBox(ref _planes[(int)FrustumSides.Right], box.Min.X, box.Min.Y, box.Min.Z, box.Width, box.Height, box.Depth))
				return false;
			if (!TestPlaneAgainstBox(ref _planes[(int)FrustumSides.Left], box.Min.X, box.Min.Y, box.Min.Z, box.Width, box.Height, box.Depth))
				return false;
			if (!TestPlaneAgainstBox(ref _planes[(int)FrustumSides.Bottom], box.Min.X, box.Min.Y, box.Min.Z, box.Width, box.Height, box.Depth))
				return false;
			if (!TestPlaneAgainstBox(ref _planes[(int)FrustumSides.Top], box.Min.X, box.Min.Y, box.Min.Z, box.Width, box.Height, box.Depth))
				return false;
			if (!TestPlaneAgainstBox(ref _planes[(int)FrustumSides.Back], box.Min.X, box.Min.Y, box.Min.Z, box.Width, box.Height, box.Depth))
				return false;
			if (!TestPlaneAgainstBox(ref _planes[(int)FrustumSides.Front], box.Min.X, box.Min.Y, box.Min.Z, box.Width, box.Height, box.Depth))
				return false;

			return true;
		}

		public bool Test(ref BoundingSphere sphere)
		{
			if (!TestPlaneAgainstSphere(ref _planes[(int)FrustumSides.Right], ref sphere.Center, sphere.Radius))
				return false;
			if (!TestPlaneAgainstSphere(ref _planes[(int)FrustumSides.Left], ref sphere.Center, sphere.Radius))
				return false;
			if (!TestPlaneAgainstSphere(ref _planes[(int)FrustumSides.Bottom], ref sphere.Center, sphere.Radius))
				return false;
			if (!TestPlaneAgainstSphere(ref _planes[(int)FrustumSides.Top], ref sphere.Center, sphere.Radius))
				return false;
			if (!TestPlaneAgainstSphere(ref _planes[(int)FrustumSides.Back], ref sphere.Center, sphere.Radius))
				return false;
			if (!TestPlaneAgainstSphere(ref _planes[(int)FrustumSides.Front], ref sphere.Center, sphere.Radius))
				return false;

			return true;
		}

		private bool TestPlaneAgainstBox(ref Plane plane, float minX, float minY, float minZ, float width, float height, float depth)
		{
			if (Plane.ClassifyPoint(ref plane, minX,         minY,          minZ)         != PlanePointClassify.Behind)
				return true;
			if (Plane.ClassifyPoint(ref plane, minX,         minY,          minZ + depth) != PlanePointClassify.Behind)
				return true;
			if (Plane.ClassifyPoint(ref plane, minX + width, minY,          minZ + depth) != PlanePointClassify.Behind)
				return true;
			if (Plane.ClassifyPoint(ref plane, minX + width, minY,          minZ)         != PlanePointClassify.Behind)
				return true;
			if (Plane.ClassifyPoint(ref plane, minX,         minY + height, minZ)         != PlanePointClassify.Behind)
				return true;
			if (Plane.ClassifyPoint(ref plane, minX,         minY + height, minZ + depth) != PlanePointClassify.Behind)
				return true;
			if (Plane.ClassifyPoint(ref plane, minX + width, minY + height, minZ + depth) != PlanePointClassify.Behind)
				return true;
			if (Plane.ClassifyPoint(ref plane, minX + width, minY + height, minZ)         != PlanePointClassify.Behind)
				return true;

			return false;
		}

		private bool TestPlaneAgainstSphere(ref Plane plane, ref Vector3 center, float radius)
		{
			float distance = Plane.DistanceBetween(ref plane, ref center);
			if (distance <= -radius)
				return false;
			else
				return true;
		}
	}
}
