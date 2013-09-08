using System;
using Blarg.GameFramework.TileMap.Meshes;

namespace Blarg.GameFramework.TileMap
{
	public abstract class TileContainer
	{
		#region Properties

		public abstract int Width { get; }
		public abstract int Height { get; }
		public abstract int Depth { get; }
		public abstract int MinX { get; }
		public abstract int MinY { get; }
		public abstract int MinZ { get; }
		public abstract int MaxX { get; }
		public abstract int MaxY { get; }
		public abstract int MaxZ { get; }

		public abstract Vector3 Position { get; }
		public abstract BoundingBox Bounds { get; }

		#endregion

		public abstract Tile Get(int x, int y, int z);
		public abstract Tile GetSafe(int x, int y, int z);

		#region Bounds Checks

		public bool IsWithinBounds(int x, int y, int z)
		{
			if (x < MinX || x > MaxX)
				return false;
			else if (y < MinY || y > MaxY)
				return false;
			else if (z < MinZ || z > MaxZ)
				return false;
			else
				return true;
		}

		public bool IsWithinLocalBounds(int x, int y, int z)
		{
			if (x < 0 || x >= Width)
				return false;
			else if (y < 0 || y >= Height)
				return false;
			else if (z < 0 || z >= Depth)
				return false;
			else
				return true;
		}

		public void GetBoundingBoxFor(Point3 point, ref BoundingBox result)
		{
			GetBoundingBoxFor(point.X, point.Y, point.Z, ref result);
		}

		public void GetBoundingBoxFor(int x, int y, int z, ref BoundingBox result)
		{
			// local "TileContainer space"
			result.Min.Set(x, y, z);
			result.Max.Set(x + 1.0f, y + 1.0f, z + 1.0f);   // 1.0f = tile width

			// move to "world/tilemap space"
			result.Min += Bounds.Min;
			result.Max += Bounds.Min;
		}

		public bool GetOverlappedTiles(BoundingBox box, ref Point3 min, ref Point3 max)
		{
			// make sure the given box actually intersects with this TileContainer in the first place
			var bounds = Bounds;
			if (!IntersectionTester.Test(ref bounds, ref box))
				return false;

			// convert to tile coords (these will be in "world/tilemap space")
			// HACK: ceil() calls and "-1"'s keep us from picking up too many/too few
			// tiles. these were arrived at through observation
			int minX = (int)box.Min.X;
			int minY = (int)box.Min.Y;
			int minZ = (int)box.Min.Z;
			int maxX = (int)Math.Ceiling(box.Max.X);
			int maxY = (int)Math.Ceiling(box.Max.Y - 1.0f);
			int maxZ = (int)Math.Ceiling(box.Max.Z);

			// trim off the excess bounds so that we end up with a min-to-max area
			// that is completely within the bounds of this TileContainer
			// HACK: "+1"'s ensure we pick up just the right amount of tiles. these were arrived
			// at through observation
			minX = MathHelpers.Clamp(minX, MinX, MaxX + 1);
			minY = MathHelpers.Clamp(minY, MinY, MaxY);
			minZ = MathHelpers.Clamp(minZ, MinZ, MaxZ + 1);
			maxX = MathHelpers.Clamp(maxX, MinX, MaxX + 1);
			maxY = MathHelpers.Clamp(maxY, MinY, MaxY);
			maxZ = MathHelpers.Clamp(maxZ, MinZ, MaxZ + 1);

			// return the leftover area, converted to the local coordinate space of the TileContainer
			min.X = minX - MinX;
			min.Y = minY - MinY;
			min.Z = minZ - MinZ;
			max.X = maxX - MinX;
			max.Y = maxY - MinY;
			max.Z = maxZ - MinZ;

			return true;
		}

		#endregion

		#region Collision Checks

		public bool CheckForCollision(Ray ray, ref Point3 collisionCoords)
		{
			// make sure that the ray and this TileContainer can actually collide in the first place
			var bounds = Bounds;
			var position = Vector3.Zero;
			if (!IntersectionTester.Test(ref ray, ref bounds, ref position))
				return false;

			// convert initial collision point to tile coords (this is in "world/tilemap space")
			int currentX = (int)position.X;
			int currentY = (int)position.Y;
			int currentZ = (int)position.Z;

			// make sure the coords are inrange of this container. due to some floating
			// point errors / decimal truncating from the above conversion we could
			// end up with one or more that are very slightly out of bounds.
			// this is still in "world/tilemap space"
			currentX = MathHelpers.Clamp(currentX, MinX, MaxX);
			currentY = MathHelpers.Clamp(currentY, MinY, MaxY);
			currentZ = MathHelpers.Clamp(currentZ, MinZ, MaxZ);

			// convert to the local space of this TileContainer
			currentX -= MinX;
			currentY -= MinY;
			currentZ -= MinZ;

			// is the start position colliding with a solid tile?
			var startTile = Get(currentX, currentY, currentZ);
			if (startTile.IsCollideable)
			{
				// collision found, set the tile coords of the collision
				collisionCoords.X = currentX;
				collisionCoords.Y = currentY;
				collisionCoords.Z = currentZ;

				// and we're done
				return true;
			}

			// no collision initially, continue on with the rest ...

			// step increments in "TileContainer tile" units
			int stepX = Math.Sign(ray.Direction.X);
			int stepY = Math.Sign(ray.Direction.Y);
			int stepZ = Math.Sign(ray.Direction.Z);

			// tile boundary (needs to be in "world/tilemap space")
			int tileBoundaryX = MinX + currentX + (stepX > 0 ? 1 : 0);
			int tileBoundaryY = MinY + currentY + (stepY > 0 ? 1 : 0);
			int tileBoundaryZ = MinZ + currentZ + (stepZ > 0 ? 1 : 0);

			// HACK: for the tMax and tDelta initial calculations below, if any of the
			//       components of ray.direction are zero, it will result in "inf"
			//       components in tMax or tDelta. This is fine, but it has to be
			//       *positive* "inf", not negative. What I found was that sometimes
			//       they would be negative, sometimes positive. So, we force them to be
			//       positive below. Additionally, "nan" components (which will happen
			//       if both sides of the divisions are zero) are bad, and we need to
			//       change that up for "inf" as well.

			// determine how far we can travel along the ray before we hit a tile boundary
			Vector3 tMax;
			tMax.X = (tileBoundaryX - ray.Position.X) / ray.Direction.X;
			tMax.Y = (tileBoundaryY - ray.Position.Y) / ray.Direction.Y;
			tMax.Z = (tileBoundaryZ - ray.Position.Z) / ray.Direction.Z;

			if (tMax.X == Single.NegativeInfinity)
				tMax.X = Single.PositiveInfinity;
			if (tMax.Y == Single.NegativeInfinity)
				tMax.Y = Single.PositiveInfinity;
			if (tMax.Z == Single.NegativeInfinity)
				tMax.Z = Single.PositiveInfinity;
			if (Single.IsNaN(tMax.X))
				tMax.X = Single.PositiveInfinity;
			if (Single.IsNaN(tMax.Y))
				tMax.Y = Single.PositiveInfinity;
			if (Single.IsNaN(tMax.Z))
				tMax.Z = Single.PositiveInfinity;

			// determine how far we must travel along the ray before we cross a grid cell
			Vector3 tDelta;
			tDelta.X = stepX / ray.Direction.X;
			tDelta.Y = stepY / ray.Direction.Y;
			tDelta.Z = stepZ / ray.Direction.Z;

			if (tDelta.X == Single.NegativeInfinity)
				tDelta.X = Single.PositiveInfinity;
			if (tDelta.Y == Single.NegativeInfinity)
				tDelta.Y = Single.PositiveInfinity;
			if (tDelta.Z == Single.NegativeInfinity)
				tDelta.Z = Single.PositiveInfinity;
			if (Single.IsNaN(tDelta.X))
				tDelta.X = Single.PositiveInfinity;
			if (Single.IsNaN(tDelta.Y))
				tDelta.Y = Single.PositiveInfinity;
			if (Single.IsNaN(tDelta.Z))
				tDelta.Z = Single.PositiveInfinity;

			bool collided = false;
			bool outOfContainer = false;
			while (!outOfContainer)
			{
				// step up to the next tile using the lowest step value
				// (in other words, we figure out on which axis, X, Y, or Z, the next
				// tile that lies on the ray is closest, and use that axis step increment
				// to move us up to get to the next tile location)
				if (tMax.X < tMax.Y && tMax.X < tMax.Z)
				{
					// tMax.x is lowest, the YZ tile boundary plane is closest
					currentX += stepX;
					tMax.X += tDelta.X;
				}
				else if (tMax.Y < tMax.Z)
				{
					// tMax.y is lowest, the XZ tile boundary plane is closest
					currentY += stepY;
					tMax.Y += tDelta.Y;
				}
				else
				{
					// tMax.z is lowest, the XY tile boundary plane is closest
					currentZ += stepZ;
					tMax.Z += tDelta.Z;
				}

				// need to figure out if this new position is still inside the bounds of
				// the container before we can attempt to determine if the current tile is
				// solid
				// (remember, currentX/Y/Z is in the local "TileContainer space"
				if (currentX < 0 || currentX >= Width ||
					currentY < 0 || currentY >= Height ||
					currentZ < 0 || currentZ >= Depth
					)
					outOfContainer = true;
				else
				{
					// still inside and at the next position, test for a solid tile
					var tile = Get(currentX, currentY, currentZ);
					if (tile.IsCollideable)
					{
						collided = true;

						// set the tile coords of the collision
						collisionCoords.X = currentX;
						collisionCoords.Y = currentY;
						collisionCoords.Z = currentZ;

						break;
					}
				}
			}

			return collided;
		}

		public bool CheckForCollision(Ray ray, ref Point3 collisionCoords, TileMeshCollection tileMeshes, ref Vector3 tileMeshCollisionPoint)
		{
			// if the ray doesn't collide with any solid tiles in the first place, then
			// we can skip this more expensive triangle collision check...
			if (!CheckForCollision(ray, ref collisionCoords))
				return false;

			// now perform the per-triangle collision check against the tile position
			// where the ray ended up at the end of the above checkForCollision() call
			return CheckForCollisionWithTileMesh(
				ray,
				collisionCoords.X, collisionCoords.Y, collisionCoords.Z,
				tileMeshes,
				ref tileMeshCollisionPoint
				);
		}

		public bool CheckForCollisionWithTileMesh(Ray ray, int x, int y, int z, TileMeshCollection tileMeshes, ref Vector3 outCollisionPoint)
		{
			var tile = Get(x, y, z);
			var mesh = tileMeshes.Get(tile);

			var vertices = mesh.CollisionVertices;

			// world position of this tile, will be used to move each
			// mesh triangle into world space
			var tileWorldPosition = new Vector3((float)x, (float)y, (float)z);

			float closestSquaredDistance = Single.PositiveInfinity;
			bool collided = false;
			var collisionPoint = Vector3.Zero;

			for (int i = 0; i < vertices.Length; i += 3)
			{
				// get the vertices making up this triangle (and move the vertices into world space)
				var a = vertices[i] + tileWorldPosition;
				var b = vertices[i + 1] + tileWorldPosition;
				var c = vertices[i + 2] + tileWorldPosition;

				if (IntersectionTester.Test(ref ray, ref a, ref b, ref c, ref collisionPoint))
				{
					collided = true;

					// if this is the closest collision yet, then keep the distance
					// and point of collision
					float squaredDistance = (collisionPoint - ray.Position).LengthSquared;
					if (squaredDistance < closestSquaredDistance)
					{
						closestSquaredDistance = squaredDistance;
						outCollisionPoint = collisionPoint;
					}
				}
			}

			return collided;
		}

		#endregion
	}
}

