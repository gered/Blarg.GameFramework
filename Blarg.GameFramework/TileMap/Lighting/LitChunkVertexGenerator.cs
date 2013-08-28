using System;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Support;
using Blarg.GameFramework.TileMap.Meshes;

namespace Blarg.GameFramework.TileMap.Lighting
{
	public class LitChunkVertexGenerator : ChunkVertexGenerator
	{
		protected override void AddMesh(VertexBuffer buffer, 
		                                Tile tile, 
		                                TileMesh sourceMesh, 
		                                TileChunk chunk, 
		                                ref Point3 position, 
		                                Matrix4x4? transform, 
		                                ref Color color, 
		                                int firstVertex, 
		                                int numVertices)
		{
			// ensure there is enough space in the destination buffer
			int verticesToAdd = numVertices;
			if (buffer.RemainingElements < verticesToAdd)
			{
				// not enough space, need to resize the destination buffer
				// resize by the exact amount needed making sure there's no wasted space at the end
				buffer.Extend(verticesToAdd - buffer.RemainingElements);
			}

			// adjust position by the tilemesh offset. TileMesh's are modeled using the
			// origin (0,0,0) as the center and are 1 unit wide/deep/tall. So, their
			// max extents are from -0.5,-0.5,-0.5 to 0.5,0.5,0.5. For rendering
			// purposes in a chunk, we want the extents to be 0,0,0 to 1,1,1. This
			// adjustment fixes that
			Vector3 offset = TileMesh.OFFSET;
			offset.X += (float)position.X;
			offset.Y += (float)position.Y;
			offset.Z += (float)position.Z;

			var bounds = new BoundingBox();
			chunk.GetBoundingBoxFor(position.X, position.Y, position.Z, ref bounds);

			// figure out what the default lighting value is for this chunk
			byte defaultLightValue = chunk.TileMap.SkyLightValue;
			if (chunk.TileMap.AmbientLightValue > defaultLightValue)
				defaultLightValue = chunk.TileMap.AmbientLightValue;

			bool needsTransform = (transform != null);
			var transformMatrix = transform.GetValueOrDefault();

			var sourceVertices = sourceMesh.Vertices;
			sourceVertices.MoveTo(firstVertex);

			// copy vertices
			for (int i = 0; i < numVertices; ++i) {
				Vector3 v = sourceVertices.GetCurrentPosition3D();
				Vector3 n = sourceVertices.GetCurrentNormal();
				Vector2 t = sourceVertices.GetCurrentTexCoord();
				Color c = sourceVertices.GetCurrentColor();

				if (needsTransform)
				{
					// need to transform the vertex + normal first before copying it
					v = Matrix4x4.Transform(transformMatrix, v);
					n = Matrix4x4.TransformNormal(transformMatrix, n);
				}

				// translate vertex into "tilemap space"
				v += offset;

				// now we need to find the appropriate light source for this vertex. this light source could be either
				// this very tile itself, or an adjacent tile. we'll check using the vertex normal as a direction to
				// "look in" for the light source ...

				// get the exact "world/tilemap space" position to grab a potential light source tile from
				var lightSourcePos = v + n;

				// HACK: if the light source position we just got lies exactly on the max x/y/z boundaries of this tile
				//       then casting it to int and using it as a tilemap grid coord to get the tile will fetch us the
				//       _next_ tile which we probably don't want when the position is exactly at max x/y/z ...
				//       (this was resulting in some incorrect dark edges along certain tile layouts)
				if (lightSourcePos.X == bounds.Max.X)
					lightSourcePos.X -= 0.01f;
				if (lightSourcePos.Y == bounds.Max.Y)
					lightSourcePos.Y -= 0.01f;
				if (lightSourcePos.Z == bounds.Max.Z)
					lightSourcePos.Z -= 0.01f;

				float brightness;

				// if the light source position is off the bounds of the entire world then use the default light value.
				// the below call to TileChunk.GetWithinSelfOrNeighbour() actually does do bounds checking, but we would
				// need to cast from float to int first. this causes some issues when the one or more of the lightSource
				// x/y/z values are between 0 and -1 (rounds up to 0 when using a cast). rather then do some weird custom
				// rounding, we just check for negatives to ensure we catch them and handle it properly
				// NOTE: this is _only_ a problem currently because world coords right now are always >= 0
				if (lightSourcePos.X < 0.0f || lightSourcePos.Y < 0.0f || lightSourcePos.Z < 0.0f)
					brightness = Tile.GetBrightness(defaultLightValue);
				else
				{
					// light source is within the boundaries of the world, get the
					// actual tile (may or may not be in a neighbouring chunk)
					int lightX = (int)lightSourcePos.X - chunk.MinX;
					int lightY = (int)lightSourcePos.Y - chunk.MinY;
					int lightZ = (int)lightSourcePos.Z - chunk.MinZ;

					var lightSourceTile = chunk.GetWithinSelfOrNeighbourSafe(lightX, lightY, lightZ);
					if (lightSourceTile == null)
						// out of bounds of the map
						brightness = Tile.GetBrightness(defaultLightValue);
					else if (lightSourceTile.IsEmptySpace)
						// this tile is getting it's light from another tile that is empty
						// just use the other tile's light value as-is
						brightness = lightSourceTile.Brightness;
					else
					{
						// this tile is getting it's light from another tile that is not empty
						// check if the direction we went in to find the other tile passes through any
						// of the other tile's opaque sides. if so, we cannot use its light value and
						// should instead just use whatever this tile's light value is
						// TODO: i'm pretty sure this is going to produce poor results at some point in the future... fix!

						var lightSourceTileMesh = chunk.TileMap.TileMeshes.Get(lightSourceTile);

						// collect a list of the sides to check for opaqueness with the light source tile .. we check
						// the sides of the light source mesh opposite to the direction of the vertex normal (direction we
						// are "moving" in)
						// TODO: is it better to check each side individually? how would that work if the normal moves us
						//       in a non-orthogonal direction and we need to check 2 sides ... ?
						byte sides = 0;
						if (n.Y < 0.0f) sides |= TileMesh.SIDE_TOP;
						if (n.Y > 0.0f) sides |= TileMesh.SIDE_BOTTOM;
						if (n.X < 0.0f) sides |= TileMesh.SIDE_RIGHT;
						if (n.X > 0.0f) sides |= TileMesh.SIDE_LEFT;
						if (n.Z < 0.0f) sides |= TileMesh.SIDE_BACK;
						if (n.Z > 0.0f) sides |= TileMesh.SIDE_FRONT;

						if (lightSourceTileMesh.IsOpaque(sides))
							brightness = tile.TileLight;
						else
							brightness = lightSourceTile.Brightness;
					}
				}


				// copy to destination
				buffer.SetCurrentPosition3D(v);
				buffer.SetCurrentNormal(n);
				buffer.SetCurrentTexCoord(t);

				// TODO: need to play with vertex/mesh color combinations a bit more to see if this is really correct
				buffer.SetCurrentColor(c.R * color.R * brightness,
				                       c.G * color.G * brightness,
				                       c.B * color.B * brightness,
				                       c.A * color.A);

				sourceVertices.MoveNext();
				buffer.MoveNext();
			}
		}
	}
}

