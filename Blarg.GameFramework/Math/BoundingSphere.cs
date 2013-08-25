using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundingSphere : IEquatable<BoundingSphere>
	{
		public Vector3 Center;
		public float Radius;

		public BoundingSphere(BoundingSphere other)
		{
			Center = other.Center;
			Radius = other.Radius;
		}

		public BoundingSphere(Vector3 center, float radius)
			: this(ref center, radius)
		{
		}

		public BoundingSphere(ref Vector3 center, float radius)
		{
			Center = center;
			Radius = radius;
		}

		public BoundingSphere(float centerX, float centerY, float centerZ, float radius)
		{
			Center.X = centerX;
			Center.Y = centerY;
			Center.Z = centerZ;
			Radius = radius;
		}

		public BoundingSphere(Vector3[] vertices)
		{
			int min;
			int max;

			int minX = 0;
			int minY = 0;
			int minZ = 0;
			int maxX = 0;
			int maxY = 0;
			int maxZ = 0;

			// find min & max points for x, y and z
			for (int i = 0; i < vertices.Length; ++i)
			{
				if (vertices[i].X < vertices[minX].X)
					minX = i;
				if (vertices[i].X > vertices[maxX].X)
					maxX = i;
				if (vertices[i].Y < vertices[minY].Y)
					minY = i;
				if (vertices[i].Y > vertices[maxY].Y)
					maxY = i;
				if (vertices[i].Z < vertices[minZ].Z)
					minZ = i;
				if (vertices[i].Z > vertices[maxZ].Z)
					maxZ = i;
			}

			// distances between these extremes for x, y and z
			float distanceSqX = Vector3.Dot(vertices[maxX] - vertices[minX], vertices[maxX] - vertices[minX]);
			float distanceSqY = Vector3.Dot(vertices[maxY] - vertices[minY], vertices[maxY] - vertices[minY]);
			float distanceSqZ = Vector3.Dot(vertices[maxZ] - vertices[minZ], vertices[maxZ] - vertices[minZ]);

			// get the pair of points representing the most distance points from each other
			min = minX;
			max = maxX;
			if (distanceSqY > distanceSqX && distanceSqY > distanceSqZ)
			{
				min = minY;
				max = maxY;
			}
			if (distanceSqZ > distanceSqX && distanceSqZ > distanceSqY)
			{
				min = minZ;
				max = maxZ;
			}

			// we now have enough info to set the initial sphere properties
			Center = (vertices[min] + vertices[max]) / 2.0f;
			Radius = (float)Math.Sqrt(Vector3.Dot(vertices[max] - Center, vertices[max] - Center));

			// now expand the sphere to make sure it encompasses all the points (if it doesn't already)
			Vector3 d;
			for (int i = 0; i < vertices.Length; ++i)
			{
				d = vertices[i] - Center;
				float distanceSq = Vector3.Dot(d, d);
				if (distanceSq > (Radius * Radius))
				{
					float distance = (float)Math.Sqrt(distanceSq);
					float newRadius = (Radius + distance) * 0.5f;
					float k = (newRadius - Radius) / distance;
					Radius = newRadius;
					d = d * k;
					Center = Center + d;
				}
			}
		}

		public static bool operator ==(BoundingSphere left, BoundingSphere right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(BoundingSphere left, BoundingSphere right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is BoundingSphere)
				return this.Equals((BoundingSphere)obj);
			else
				return false;
		}

		public bool Equals(BoundingSphere other)
		{
			return (Center == other.Center && Radius == other.Radius);
		}

		public override int GetHashCode()
		{
			return Center.GetHashCode() ^ Radius.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{Center:{0} Radius:{1}}}", Center, Radius);
		}
	}
}
