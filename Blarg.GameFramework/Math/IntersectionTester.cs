using System;

namespace Blarg.GameFramework
{
	public static class IntersectionTester
	{
		public static bool Test(ref BoundingBox box, ref Vector3 point)
		{
			if ((point.X >= box.Min.X && point.X <= box.Max.X) &&
			    (point.Y >= box.Min.Y && point.Y <= box.Max.Y) && 
			    (point.Z >= box.Min.Z && point.Z <= box.Max.Z))
				return true;
			else
				return false;
		}

		public static bool Test(ref BoundingSphere sphere, ref Vector3 point)
		{
			if ((float)Math.Abs(Vector3.Distance(ref point, ref sphere.Center)) < sphere.Radius)
				return true;
			else
				return false;
		}

		public static bool Test(ref BoundingBox box, Vector3[] vertices, ref Vector3 outFirstIntersection)
		{
			for (int i = 0; i < vertices.Length; ++i)
			{
				if ((vertices[i].X >= box.Min.X && vertices[i].X <= box.Max.X) &&
				    (vertices[i].Y >= box.Min.Y && vertices[i].Y <= box.Max.Y) && 
				    (vertices[i].Z >= box.Min.Z && vertices[i].Z <= box.Max.Z))
				{
					outFirstIntersection = vertices[i];
					return true;
				}
			}

			return false;
		}

		public static bool Test(ref BoundingSphere sphere, Vector3[] vertices, ref Vector3 outFirstIntersection)
		{
			for (int i = 0; i < vertices.Length; ++i)
			{
				if ((float)Math.Abs(Vector3.Distance(ref vertices[i], ref sphere.Center)) < sphere.Radius)
				{
					outFirstIntersection = vertices[i];
					return true;
				}
			}

			return false;
		}

		public static bool Test(ref BoundingBox a, ref BoundingBox b)
		{
			if (a.Max.X < b.Min.X || a.Min.X > b.Max.X)
				return false;
			if (a.Max.Y < b.Min.Y || a.Min.Y > b.Max.Y)
				return false;
			if (a.Max.Z < b.Min.Z || a.Min.Z > b.Max.Z)
				return false;

			return true;
		}

		public static bool Test(ref BoundingSphere a, ref BoundingSphere b)
		{
			Vector3 temp = a.Center - b.Center;
			float distanceSquared = Vector3.Dot(temp, temp);

			float radiusSum = a.Radius + b.Radius;
			if (distanceSquared <= radiusSum * radiusSum)
				return true;
			else
				return false;
		}

		public static bool Test(ref BoundingSphere sphere, ref Plane plane)
		{
			float distance = Vector3.Dot(sphere.Center, plane.Normal) - plane.D;
			if ((float)Math.Abs(distance) <= sphere.Radius)
				return true;
			else
				return false;
		}

		public static bool Test(ref BoundingBox box, ref Plane plane)
		{
			Vector3 temp1 = (box.Max + box.Min) / 2.0f;
			Vector3 temp2 = box.Max - temp1;

			float radius = (temp2.X * (float)Math.Abs(plane.Normal.X)) + (temp2.Y * (float)Math.Abs(plane.Normal.Y)) + (temp2.Z * (float)Math.Abs(plane.Normal.Z));

			float distance = Vector3.Dot(plane.Normal, temp1) - plane.D;

			if ((float)Math.Abs(distance) <= radius)
				return true;
			else
				return true;
		}

		public static bool Test(ref Ray ray, ref Plane plane, ref Vector3 outIntersection)
		{
			float denominator = Vector3.Dot(ray.Direction, plane.Normal);
			if (denominator == 0.0f)
				return false;

			float t = ((-plane.D - Vector3.Dot(ray.Position, plane.Normal)) / denominator);
			if (t < 0.0f)
				return false;

			Vector3 temp1 = ray.GetPositionAt(t);
			outIntersection.X = temp1.X;
			outIntersection.Y = temp1.Y;
			outIntersection.Z = temp1.Z;

			return true;
		}

		public static bool Test(ref Ray ray, ref BoundingSphere sphere, ref Vector3 outFirstIntersection)
		{
			Vector3 temp1 = ray.Position - sphere.Center;

			float b = Vector3.Dot(temp1, ray.Direction);
			float c = Vector3.Dot(temp1, temp1) - (sphere.Radius * sphere.Radius);

			if (c > 0.0f && b > 0.0f)
				return false;

			float discriminant = b * b - c;
			if (discriminant < 0.0f)
				return false;

			float t = -b - (float)Math.Sqrt(discriminant);
			if (t < 0.0f)
				t = 0.0f;

			Vector3 temp2 = ray.GetPositionAt(t);
			outFirstIntersection.X = temp2.X;
			outFirstIntersection.Y = temp2.Y;
			outFirstIntersection.Z = temp2.Z;

			return true;
		}

		public static bool Test(ref Ray ray, ref BoundingBox box, ref Vector3 outFirstIntersection)
		{
			float tmin = 0.0f;
			float tmax = float.MaxValue;

			if ((float)Math.Abs(ray.Direction.X) < float.Epsilon)
			{
				if (ray.Position.X < box.Min.X || ray.Position.X > box.Max.X)
					return false;
			}
			else
			{
				float invD = 1.0f / ray.Direction.X;
				float t1 = (box.Min.X - ray.Position.X) * invD;
				float t2 = (box.Max.X - ray.Position.X) * invD;

				if (t1 > t2)
				{
					float tswap = t1;
					t1 = t2;
					t2 = tswap;
				}

				tmin = Math.Max(tmin, t1);
				tmax = Math.Min(tmax, t2);

				if (tmin > tmax)
					return false;
			}

			if ((float)Math.Abs(ray.Direction.Y) < float.Epsilon)
			{
				if (ray.Position.Y < box.Min.Y || ray.Position.Y > box.Max.Y)
					return false;
			}
			else
			{
				float invD = 1.0f / ray.Direction.Y;
				float t1 = (box.Min.Y - ray.Position.Y) * invD;
				float t2 = (box.Max.Y - ray.Position.Y) * invD;

				if (t1 > t2)
				{
					float tswap = t1;
					t1 = t2;
					t2 = tswap;
				}

				tmin = Math.Max(tmin, t1);
				tmax = Math.Min(tmax, t2);

				if (tmin > tmax)
					return false;
			}

			if ((float)Math.Abs(ray.Direction.Z) < float.Epsilon)
			{
				if (ray.Position.Z < box.Min.Z || ray.Position.Z > box.Max.Z)
					return false;
			}
			else
			{
				float invD = 1.0f / ray.Direction.Z;
				float t1 = (box.Min.Z - ray.Position.Z) * invD;
				float t2 = (box.Max.Z - ray.Position.Z) * invD;

				if (t1 > t2)
				{
					float tswap = t1;
					t1 = t2;
					t2 = tswap;
				}

				tmin = Math.Max(tmin, t1);
				tmax = Math.Min(tmax, t2);

				if (tmin > tmax)
					return false;
			}

			Vector3 temp1 = ray.GetPositionAt(tmin);
			outFirstIntersection.X = temp1.X;
			outFirstIntersection.Y = temp1.Y;
			outFirstIntersection.Z = temp1.Z;

			return true;
		}

		public static bool Test(ref BoundingBox box, ref BoundingSphere sphere)
		{
			float distanceSq = BoundingBox.GetSquaredDistanceFromPointToBox(ref sphere.Center, ref box);

			if (distanceSq <= (sphere.Radius * sphere.Radius))
				return true;
			else
				return false;
		}

		public static bool Test(ref Ray ray, ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 outIntersection)
		{
			float r, num1, num2;

			Vector3 temp1 = Vector3.Cross(b - a, c - a);
			if (temp1.X == 0.0f && temp1.Y == 0.0f && temp1.Z == 0.0f)
				return false;

			Vector3 temp2 = ray.Position - a;
			num1 = -Vector3.Dot(temp1, temp2);
			num2 = Vector3.Dot(temp1, ray.Direction);
			if ((float)Math.Abs(num2) < float.Epsilon)
			{
				if (num1 == 0.0f)
				{
					outIntersection = ray.Position;
					return true;
				}
				else
					return false;
			}

			r = num1 / num2;
			if (r < 0.0f)
				return false;

			Vector3 temp3 = ray.GetPositionAt(r);
			if (Test(ref temp3, ref a, ref b, ref c))
			{
				outIntersection = temp3;
				return true;
			}
			else
				return false;
		}

		public static bool Test(ref Vector3 point, ref Vector3 a, ref Vector3 b, ref Vector3 c)
		{
			Vector3 v0 = c - a;
			Vector3 v1 = b - a;
			Vector3 v2 = point - a;

			float dot00 = (v0.X * v0.X) + (v0.Y * v0.Y) + (v0.Z * v0.Z);
			float dot01 = (v0.X * v1.X) + (v0.Y * v1.Y) + (v0.Z * v1.Z);
			float dot02 = (v0.X * v2.X) + (v0.Y * v2.Y) + (v0.Z * v2.Z);
			float dot11 = (v1.X * v1.X) + (v1.Y * v1.Y) + (v1.Z * v1.Z);
			float dot12 = (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);

			float denom = dot00 * dot11 - dot01 * dot01;
			if (denom == 0)
				return false;

			float u = (dot11 * dot02 - dot01 * dot12) / denom;
			float v = (dot00 * dot12 - dot01 * dot02) / denom;

			if (u >= 0 && v >= 0 && u + v <= 1)
				return true;
			else
				return false;
		}

		public static bool Test(ref SweptEllipsoidCollisionPacket packet, ref Vector3 v1, ref Vector3 v2, ref Vector3 v3)
		{
			bool foundCollision = false;

			Vector3 p1;
			Vector3 p2;
			Vector3 p3;
			Vector3.Divide(ref v1, ref packet.EllipsoidRadius, out p1);
			Vector3.Divide(ref v2, ref packet.EllipsoidRadius, out p2);
			Vector3.Divide(ref v3, ref packet.EllipsoidRadius, out p3);

			var trianglePlane = new Plane(ref p1, ref p2, ref p3);

			// Is the triangle front-facing to the entity's velocity?
			if (Plane.IsFrontFacingTo(ref trianglePlane, ref packet.esNormalizedVelocity))
			{
				float t0;
				float t1;
				bool embeddedInPlane = false;
				float distToTrianglePlane = Plane.DistanceBetween(ref trianglePlane, ref packet.esPosition);
				float normalDotVelocity = Vector3.Dot(trianglePlane.Normal, packet.esVelocity);

				// Is the sphere travelling parallel to the plane?
				if (normalDotVelocity == 0.0f)
				{
					if ((float)Math.Abs(distToTrianglePlane) >= 1.0f)
					{
						// Sphere is not embedded in the plane, no collision possible
						return false;
					}
					else
					{
						// Sphere is embedded in the plane, it intersects throughout the whole time period
						embeddedInPlane = true;
						t0 = 0.0f;
						t1 = 1.0f;
					}
				}
				else
				{
					// Not travelling parallel to the plane
					t0 = (-1.0f - distToTrianglePlane) / normalDotVelocity;
					t1 = (1.0f - distToTrianglePlane) / normalDotVelocity;

					// Swap so t0 < t1
					if (t0 > t1)
					{
						float temp = t1;
						t1 = t0;
						t0 = temp;
					}

					// Check that at least one result is within range
					if (t0 > 1.0f || t1 < 0.0f)
					{
						// Both values outside the range [0,1], no collision possible
						return false;
					}

					t0 = MathHelpers.Clamp(t0, 0.0f, 1.0f);
					t1 = MathHelpers.Clamp(t1, 0.0f, 1.0f);
				}

				// At this point, we have two time values (t0, t1) between which the
				// swept sphere intersects with the triangle plane
				Vector3 collisionPoint = new Vector3();
				float t = 1.0f;

				// First, check for a collision inside the triangle. This will happen
				// at time t0 if at all as this is when the sphere rests on the front
				// side of the triangle plane.
				if (!embeddedInPlane)
				{
					Vector3 planeIntersectionPoint = (packet.esPosition - trianglePlane.Normal) + packet.esVelocity * t0;

					if (Test(ref planeIntersectionPoint, ref p1, ref p2, ref p3))
					{
						foundCollision = true;
						t = t0;
						collisionPoint = planeIntersectionPoint;
					}
				}

				// If we haven't found a collision at this point, we need to check the 
				// points and edges of the triangle
				if (foundCollision == false)
				{
					Vector3 velocity = packet.esVelocity;
					Vector3 basePoint = packet.esPosition;
					float velocitySquaredLength = velocity.LengthSquared;
					float a, b, c;
					float newT = 0.0f;

					// For each vertex or edge, we have a quadratic equation to be solved
					// Check against the points first

					a = velocitySquaredLength;

					// P1
					b = 2.0f * Vector3.Dot(velocity, basePoint - p1);
					c = (p1 - basePoint).LengthSquared - 1.0f;
					if (MathHelpers.GetLowestQuadraticRoot(a, b, c, t, out newT))
					{
						t = newT;
						foundCollision = true;
						collisionPoint = p1;
					}

					// P2
					b = 2.0f * Vector3.Dot(velocity, basePoint - p2);
					c = (p2 - basePoint).LengthSquared - 1.0f;
					if (MathHelpers.GetLowestQuadraticRoot(a, b, c, t, out newT))
					{
						t = newT;
						foundCollision = true;
						collisionPoint = p2;
					}

					// P3
					b = 2.0f * Vector3.Dot(velocity, basePoint - p3);
					c = (p3 - basePoint).LengthSquared - 1.0f;
					if (MathHelpers.GetLowestQuadraticRoot(a, b, c, t, out newT))
					{
						t = newT;
						foundCollision = true;
						collisionPoint = p3;
					}

					// Now check against the edges

					// P1 -> P2
					Vector3 edge = p2 - p1;
					Vector3 baseToVertex = p1 - basePoint;
					float edgeSquaredLength = edge.LengthSquared;
					float edgeDotVelocity = Vector3.Dot(edge, velocity);
					float edgeDotBaseToVertex = Vector3.Dot(edge, baseToVertex);

					a = edgeSquaredLength * -velocitySquaredLength + edgeDotVelocity * edgeDotVelocity;
					b = edgeSquaredLength * (2.0f * Vector3.Dot(velocity, baseToVertex)) - 2.0f * edgeDotVelocity * edgeDotBaseToVertex;
					c = edgeSquaredLength * (1.0f - baseToVertex.LengthSquared) + edgeDotBaseToVertex * edgeDotBaseToVertex;

					if (MathHelpers.GetLowestQuadraticRoot(a, b, c, t, out newT))
					{
						// Check if intersection is within line segment
						float f = (edgeDotVelocity * newT - edgeDotBaseToVertex) / edgeSquaredLength;
						if (f >= 0.0f && f <= 1.0f)
						{
							// Intersection took place within the segment
							t = newT;
							foundCollision = true;
							collisionPoint = p1 + edge * f;
						}
					}

					// P2 -> P3
					edge = p3 - p2;
					baseToVertex = p2 - basePoint;
					edgeSquaredLength = edge.LengthSquared;
					edgeDotVelocity = Vector3.Dot(edge, velocity);
					edgeDotBaseToVertex = Vector3.Dot(edge, baseToVertex);

					a = edgeSquaredLength * -velocitySquaredLength + edgeDotVelocity * edgeDotVelocity;
					b = edgeSquaredLength * (2.0f * Vector3.Dot(velocity, baseToVertex)) - 2.0f * edgeDotVelocity * edgeDotBaseToVertex;
					c = edgeSquaredLength * (1.0f - baseToVertex.LengthSquared) + edgeDotBaseToVertex * edgeDotBaseToVertex;

					if (MathHelpers.GetLowestQuadraticRoot(a, b, c, t, out newT))
					{
						// Check if intersection is within line segment
						float f = (edgeDotVelocity * newT - edgeDotBaseToVertex) / edgeSquaredLength;
						if (f >= 0.0f && f <= 1.0f)
						{
							// Intersection took place within the segment
							t = newT;
							foundCollision = true;
							collisionPoint = p2 + edge * f;
						}
					}

					// P3 -> P1
					edge = p1 - p3;
					baseToVertex = p3 - basePoint;
					edgeSquaredLength = edge.LengthSquared;
					edgeDotVelocity = Vector3.Dot(edge, velocity);
					edgeDotBaseToVertex = Vector3.Dot(edge, baseToVertex);

					a = edgeSquaredLength * -velocitySquaredLength + edgeDotVelocity * edgeDotVelocity;
					b = edgeSquaredLength * (2.0f * Vector3.Dot(velocity, baseToVertex)) - 2.0f * edgeDotVelocity * edgeDotBaseToVertex;
					c = edgeSquaredLength * (1.0f - baseToVertex.LengthSquared) + edgeDotBaseToVertex * edgeDotBaseToVertex;

					if (MathHelpers.GetLowestQuadraticRoot(a, b, c, t, out newT))
					{
						// Check if intersection is within line segment
						float f = (edgeDotVelocity * newT - edgeDotBaseToVertex) / edgeSquaredLength;
						if (f >= 0.0f && f <= 1.0f)
						{
							// Intersection took place within the segment
							t = newT;
							foundCollision = true;
							collisionPoint = p3 + edge * f;
						}
					}
				}

				// Set result of test
				if (foundCollision == true)
				{
					float distanceToCollision = t * packet.esVelocity.Length;

					// Does this triangle qualify for the closest collision?
					if (packet.FoundCollision == false || distanceToCollision < packet.NearestDistance)
					{
						packet.NearestDistance = distanceToCollision;
						packet.esIntersectionPoint = collisionPoint;
						packet.FoundCollision = true;
					}
				}
			}

			return foundCollision;
		}
	}
}
