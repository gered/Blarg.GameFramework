using System;

namespace Blarg.GameFramework
{
	public struct SweptEllipsoidCollisionPacket
	{
		// defines the x/y/z radius of the entity being checked
		public Vector3 EllipsoidRadius;

		public bool FoundCollision;
		public float NearestDistance;

		// the below fields are all in "ellipsoid space"

		public Vector3 esVelocity;                  // velocity of the entity
		public Vector3 esNormalizedVelocity;
		public Vector3 esPosition;                  // current position of the entity

		public Vector3 esIntersectionPoint;         // if an intersection is found ...
	}
}
