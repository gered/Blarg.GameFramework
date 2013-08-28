using System;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.TileMap.Meshes;

namespace Blarg.GameFramework.TileMap
{
	public class ChunkVertexGenerator
	{
		public ChunkVertexGenerator()
		{
		}

		private VertexBuffer MakeVertexBuffer()
		{
			var buffer = new VertexBuffer(Framework.GraphicsDevice,
			                              VertexAttributeDeclarations.TextureColorNormalPosition3D,
			                              0,
			                              BufferObjectUsage.Static);
			return buffer;
		}

		public void Generate(TileChunk chunk)
		{
			VertexBuffer mesh = null;
			VertexBuffer alphaMesh = null;

			int numMeshVertices = 0;
			int numAlphaVertices = 0;

			for (int y = 0; y < chunk.Height; ++y)
			{
				for (int z = 0; z < chunk.Depth; ++z)
				{
					for (int x = 0; x < chunk.Width; ++x)
					{
						var tile = chunk.Get(x, y, z);
						if (tile.TileIndex == Tile.NO_TILE)
							continue;

						var tileMesh = chunk.TileMap.TileMeshes.Get(tile);

						// choose the right vertex buffer to add this tile's mesh
						// to and, if necessary, create the buffer
						VertexBuffer buffer;
						if (tileMesh.Alpha)
						{
							if (alphaMesh == null)
								alphaMesh = MakeVertexBuffer();
							buffer = alphaMesh;
						}
						else
						{
							if (mesh == null)
								mesh = MakeVertexBuffer();
							buffer = mesh;
						}

						// "world/tilemap space" position that this tile is at
						Point3 position;
						position.X = x + (int)chunk.Position.X;
						position.Y = y + (int)chunk.Position.Y;
						position.Z = z + (int)chunk.Position.Z;

						Matrix4x4? transform = Tile.GetTransformationFor(tile);

						// tile color
						Color color;
						if (tile.HasCustomColor)
							color = Color.FromInt(tile.Color);
						else
							color = tileMesh.Color;

						int numVertices = 0;
						if (tileMesh is CubeTileMesh)
							numVertices = HandleCubeMesh(buffer, x, y, z, tile, chunk, (CubeTileMesh)tileMesh, ref position, transform, ref color);
						else
							numVertices = HandleGenericMesh(buffer, x, y, z, tile, chunk, tileMesh, ref position, transform, ref color);

						if (tileMesh.Alpha)
							numAlphaVertices += numVertices;
						else
							numMeshVertices += numVertices;
					}
				}
			}

			chunk.SetMeshes(mesh, numMeshVertices, alphaMesh, numAlphaVertices);
		}

		private int HandleCubeMesh(VertexBuffer buffer,
			                        int x, 
		                            int y, 
		                            int z, 
		                            Tile tile, 
		                            TileChunk chunk, 
		                            CubeTileMesh mesh, 
		                            ref Point3 tileMapPosition, 
		                            Matrix4x4? transform, 
		                            ref Color color) 
		{
			// determine what's next to each cube face
			Tile left = chunk.GetWithinSelfOrNeighbourSafe(x - 1, y, z);
			Tile right = chunk.GetWithinSelfOrNeighbourSafe(x + 1, y, z);
			Tile forward = chunk.GetWithinSelfOrNeighbourSafe(x, y, z - 1);
			Tile backward = chunk.GetWithinSelfOrNeighbourSafe(x, y, z + 1);
			Tile down = chunk.GetWithinSelfOrNeighbourSafe(x, y - 1, z);
			Tile up = chunk.GetWithinSelfOrNeighbourSafe(x, y + 1, z);

			int numVertices = 0;

			// evaluate each face's visibility and add it's vertices if needed one at a time
			if ((left == null || left.TileIndex == Tile.NO_TILE || !chunk.TileMap.TileMeshes.Get(left).IsOpaque(TileMesh.SIDE_RIGHT)) && mesh.HasFace(TileMesh.SIDE_LEFT)) {
				// left face is visible
				AddMesh(buffer,
				        tile,
				        mesh,
				        chunk,
				        ref tileMapPosition,
				        transform,
				        ref color,
				        mesh.LeftFaceVertexOffset,
				        TileMesh.CUBE_VERTICES_PER_FACE);
				numVertices += TileMesh.CUBE_VERTICES_PER_FACE;
			}
			if ((right == null || right.TileIndex == Tile.NO_TILE || !chunk.TileMap.TileMeshes.Get(right).IsOpaque(TileMesh.SIDE_LEFT)) && mesh.HasFace(TileMesh.SIDE_RIGHT)) {
				// right face is visible
				AddMesh(buffer,
				        tile,
				        mesh,
				        chunk,
				        ref tileMapPosition,
				        transform,
				        ref color,
				        mesh.RightFaceVertexOffset,
				        TileMesh.CUBE_VERTICES_PER_FACE);
				numVertices += TileMesh.CUBE_VERTICES_PER_FACE;
			}
			if ((forward == null || forward.TileIndex == Tile.NO_TILE || !chunk.TileMap.TileMeshes.Get(forward).IsOpaque(TileMesh.SIDE_BACK)) && mesh.HasFace(TileMesh.SIDE_FRONT)) {
				// front face is visible
				AddMesh(buffer,
				        tile,
				        mesh,
				        chunk,
				        ref tileMapPosition,
				        transform,
				        ref color,
				        mesh.FrontFaceVertexOffset,
				        TileMesh.CUBE_VERTICES_PER_FACE);
				numVertices += TileMesh.CUBE_VERTICES_PER_FACE;
			}
			if ((backward == null || backward.TileIndex == Tile.NO_TILE || !chunk.TileMap.TileMeshes.Get(backward).IsOpaque(TileMesh.SIDE_FRONT)) && mesh.HasFace(TileMesh.SIDE_BACK)) {
				// back face is visible
				AddMesh(buffer,
				        tile,
				        mesh,
				        chunk,
				        ref tileMapPosition,
				        transform,
				        ref color,
				        mesh.BackFaceVertexOffset,
				        TileMesh.CUBE_VERTICES_PER_FACE);
				numVertices += TileMesh.CUBE_VERTICES_PER_FACE;
			}
			if ((down == null || down.TileIndex == Tile.NO_TILE || !chunk.TileMap.TileMeshes.Get(down).IsOpaque(TileMesh.SIDE_TOP)) && mesh.HasFace(TileMesh.SIDE_BOTTOM)) {
				// bottom face is visible
				AddMesh(buffer,
				        tile,
				        mesh,
				        chunk,
				        ref tileMapPosition,
				        transform,
				        ref color,
				        mesh.BottomFaceVertexOffset,
				        TileMesh.CUBE_VERTICES_PER_FACE);
				numVertices += TileMesh.CUBE_VERTICES_PER_FACE;
			}
			if ((up == null || up.TileIndex == Tile.NO_TILE || !chunk.TileMap.TileMeshes.Get(up).IsOpaque(TileMesh.SIDE_BOTTOM)) && mesh.HasFace(TileMesh.SIDE_TOP)) {
				// top face is visible
				AddMesh(buffer,
				        tile,
				        mesh,
				        chunk,
				        ref tileMapPosition,
				        transform,
				        ref color,
				        mesh.TopFaceVertexOffset,
				        TileMesh.CUBE_VERTICES_PER_FACE);
				numVertices += TileMesh.CUBE_VERTICES_PER_FACE;
			}

			return numVertices;
		}

		private int HandleGenericMesh(VertexBuffer buffer,
		                               int x,
		                               int y,
		                               int z,
		                               Tile tile,
		                               TileChunk chunk,
		                               TileMesh mesh,
		                               ref Point3 tileMapPosition,
		                               Matrix4x4? transform,
		                               ref Color color)
		{
			bool visible = false;

			// visibility determination. we check for at least one
			// adjacent empty space / non-opaque tile
			Tile left = chunk.GetWithinSelfOrNeighbourSafe(x - 1, y, z);
			Tile right = chunk.GetWithinSelfOrNeighbourSafe(x + 1, y, z);
			Tile forward = chunk.GetWithinSelfOrNeighbourSafe(x, y, z - 1);
			Tile backward = chunk.GetWithinSelfOrNeighbourSafe(x, y, z + 1);
			Tile down = chunk.GetWithinSelfOrNeighbourSafe(x, y - 1, z);
			Tile up = chunk.GetWithinSelfOrNeighbourSafe(x, y + 1, z);

			// null == empty space (off the edge of the entire map)
			if ((left == null || left.IsEmptySpace || !chunk.TileMap.TileMeshes.Get(left).IsOpaque(TileMesh.SIDE_RIGHT)) ||
				(right == null || right.IsEmptySpace || !chunk.TileMap.TileMeshes.Get(right).IsOpaque(TileMesh.SIDE_LEFT)) ||
				(forward == null || forward.IsEmptySpace || !chunk.TileMap.TileMeshes.Get(forward).IsOpaque(TileMesh.SIDE_BACK)) ||
				(backward == null || backward.IsEmptySpace || !chunk.TileMap.TileMeshes.Get(backward).IsOpaque(TileMesh.SIDE_FRONT)) ||
				(up == null || up.IsEmptySpace || !chunk.TileMap.TileMeshes.Get(up).IsOpaque(TileMesh.SIDE_BOTTOM)) ||
				(down == null || down.IsEmptySpace || !chunk.TileMap.TileMeshes.Get(down).IsOpaque(TileMesh.SIDE_TOP))
				)
				visible = true;

			if (visible)
				AddMesh(buffer,
				        tile,
				        mesh,
				        chunk,
				        ref tileMapPosition,
				        transform,
				        ref color,
				        0,
				        mesh.Vertices.NumElements);

			return mesh.Vertices.NumElements;
		}

		protected virtual void AddMesh(VertexBuffer buffer,
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

			bool needsTransform = (transform != null);
			var transformMatrix = transform.GetValueOrDefault();

			var sourceVertices = sourceMesh.Vertices;
			sourceVertices.MoveTo(firstVertex);

			// copy vertices
			for (int i = 0; i < numVertices; ++i) {
				Vector3 v = sourceVertices.GetCurrentPosition3D();
				Vector3 n = sourceVertices.GetCurrentNormal();

				if (needsTransform)
				{
					// need to transform the vertex + normal first before copying it
					v = Matrix4x4.Transform(transformMatrix, v);
					n = Matrix4x4.TransformNormal(transformMatrix, n);
				}

				// translate vertex into "tilemap space"
				v += offset;

				// copy to destination
				buffer.SetCurrentPosition3D(v);
				buffer.SetCurrentNormal(n);

				// just directly copy the tex coord as-is
				buffer.SetCurrentTexCoord(sourceVertices.GetCurrentTexCoord());

				// color is the same for the entire mesh
				buffer.SetCurrentColor(color);

				sourceVertices.MoveNext();
				buffer.MoveNext();
			}
		}
	}
}

