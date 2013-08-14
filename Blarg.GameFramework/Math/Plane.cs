using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	public enum PlanePointClassify
	{
		InFront = 0,
		Behind = 1,
		OnPlane = 2
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct Plane : IEquatable<Plane>
	{
		public Vector3 Normal;
		public float D;

		public Plane(float a, float b, float c, float d)
		{
			Normal.X = a;
			Normal.Y = b;
			Normal.Z = c;
			D = d;
		}

		public Plane(Vector3 origin, Vector3 normal)
		: this(ref origin, ref normal)
		{
		}

		public Plane(ref Vector3 origin, ref Vector3 normal)
		{
			Normal = normal;
			D = -Vector3.Dot(ref origin, ref normal);
		}

		public Plane(Vector3 a, Vector3 b, Vector3 c)
		: this(ref a, ref b, ref c)
		{
		}

		public Plane(ref Vector3 a, ref Vector3 b, ref Vector3 c)
		{
			Vector3 e3 = b - a;
			Vector3 e1 = c - b;
			Vector3 crossed = Vector3.Cross(e3, e1);
			float scaleFactor = 1.0f / crossed.Length;

			Normal = crossed * scaleFactor;

			D = -Vector3.Dot(a, Normal);
		}

		public static PlanePointClassify ClassifyPoint(Plane plane, Vector3 point)
		{
			return ClassifyPoint(ref plane, ref point);
		}

		public static PlanePointClassify ClassifyPoint(ref Plane plane, float x, float y, float z)
		{
			Vector3 point = new Vector3(x, y, z);
			return ClassifyPoint(ref plane, ref point);
		}

		public static PlanePointClassify ClassifyPoint(ref Plane plane, ref Vector3 point)
		{
			float planeDot = Vector3.Dot(ref plane.Normal, ref point) + plane.D;

			if (planeDot < 0.0f)
				return PlanePointClassify.Behind;
			else if (planeDot > 0.0f)
				return PlanePointClassify.InFront;
			else
				return PlanePointClassify.OnPlane;
		}

		public static float DistanceBetween(Plane plane, Vector3 point)
		{
			return DistanceBetween(ref plane, ref point);
		}

		public static float DistanceBetween(ref Plane plane, ref Vector3 point)
		{
			return Vector3.Dot(ref point, ref plane.Normal) + plane.D;
		}

		public static bool IsFrontFacingTo(Plane plane, Vector3 direction)
		{
			return IsFrontFacingTo(ref plane, ref direction);
		}

		public static bool IsFrontFacingTo(ref Plane plane, ref Vector3 direction)
		{
			if (Vector3.Dot(plane.Normal, direction) <= 0.0f)
				return true;
			else
				return false;
		}

		public static Plane Normalize(Plane plane)
		{
			Plane result;
			Normalize(ref plane, out result);
			return result;
		}

		public static void Normalize(ref Plane plane, out Plane result)
		{
			float length = (float)Math.Sqrt(
				(plane.Normal.X * plane.Normal.X) + 
				(plane.Normal.Y * plane.Normal.Y) + 
				(plane.Normal.Z * plane.Normal.Z)
				);

			result.Normal.X = plane.Normal.X / length;
			result.Normal.Y = plane.Normal.Y / length;
			result.Normal.Z = plane.Normal.Z / length;
			result.D = plane.D / length;
		}

		public static bool operator ==(Plane left, Plane right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Plane left, Plane right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Plane)
				return this.Equals((Plane)obj);
			else
				return false;
		}

		public bool Equals(Plane other)
		{
			return (Normal == other.Normal && D == other.D);
		}

		public override int GetHashCode()
		{
			return Normal.GetHashCode() ^ D.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{Normal:{0} D:{1}}}", Normal, D);
		}
	}
}
