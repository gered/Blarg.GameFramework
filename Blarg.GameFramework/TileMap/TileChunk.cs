using System;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework.TileMap
{
	public class TileChunk : TileContainer, TileRawDataContainer, IDisposable
	{
		#region Fields

		readonly Tile[] _data;
		readonly int _x;
		readonly int _y;
		readonly int _z;
		readonly int _width;
		readonly int _height;
		readonly int _depth;
		readonly Vector3 _position;
		readonly BoundingBox _bounds;
		int _numMeshVertices;
		VertexBuffer _mesh;
		int _numAlphaMeshVertices;
		VertexBuffer _alphaMesh;

		#endregion

		#region Properties

		public readonly TileMap TileMap;

		public Tile[] Data
		{
			get { return _data; }
		}

		public override int Width
		{
			get { return _width; }
		}

		public override int Height
		{
			get { return _height; }
		}

		public override int Depth
		{
			get { return _depth; }
		}

		public override int MinX
		{
			get { return _x; }
		}

		public override int MinY
		{
			get { return _y; }
		}

		public override int MinZ
		{
			get { return _z; }
		}

		public override int MaxX
		{
			get { return _x + _width - 1; }
		}

		public override int MaxY
		{
			get { return _y + _height - 1; }
		}

		public override int MaxZ
		{
			get { return _z + _depth - 1; }
		}

		public override Vector3 Position
		{
			get { return _position; }
		}

		public override BoundingBox Bounds
		{
			get { return _bounds; }
		}

		public int NumMeshVertices
		{
			get { return _numMeshVertices; }
		}

		public VertexBuffer Mesh
		{
			get { return _mesh; }
		}

		public int NumAlphaMeshVertices
		{
			get { return _numAlphaMeshVertices; }
		}

		public VertexBuffer AlphaMesh
		{
			get { return _alphaMesh; }
		}

		#endregion

		public TileChunk(int x, int y, int z, int width, int height, int depth, TileMap tileMap)
		{
			if (tileMap == null)
				throw new ArgumentNullException("tileMap");

			TileMap = tileMap;
			_x = x;
			_y = y;
			_z = z;
			_width = width;
			_height = height;
			_depth = depth;
			_position = new Vector3(x, y, z);
			_bounds = new BoundingBox();
			_bounds.Min.Set(x, y, z);
			_bounds.Max.Set(x + width, y + height, z + depth);

			int numTiles = width * height * depth;
			_data = new Tile[numTiles];
			for (int i = 0; i < numTiles; ++i)
				_data[i] = new Tile();

			_mesh = null;
			_alphaMesh = null;
		}

		public void UpdateVertices(ChunkVertexGenerator generator)
		{
			generator.Generate(this);
		}

		internal void SetMeshes(VertexBuffer mesh, int numMeshVertices, VertexBuffer alphaMesh, int numAlphaMeshVertices)
		{
			if (_mesh != null)
				_mesh.Dispose();
			_mesh = mesh;

			if (_alphaMesh != null)
				_alphaMesh.Dispose();
			_alphaMesh = alphaMesh;

			_numMeshVertices = numMeshVertices;
			_numAlphaMeshVertices = numAlphaMeshVertices;
		}

		#region Tile Retrieval

		public Tile GetWithinSelfOrNeighbour(int x, int y, int z)
		{
			int checkX = x + _x;
			int checkY = y + _y;
			int checkZ = z + _z;
			return TileMap.Get(checkX, checkY, checkZ);
		}

		public Tile GetWithinSelfOrNeighbourSafe(int x, int y, int z)
		{
			int checkX = x + _x;
			int checkY = y + _y;
			int checkZ = z + _z;
			if (!TileMap.IsWithinBounds(checkX, checkY, checkZ))
				return null;
			else
				return TileMap.Get(checkX, checkY, checkZ);
		}

		public override Tile Get(int x, int y, int z)
		{
			int index = GetIndexOf(x, y, z);
			return _data[index];
		}

		public override Tile Get(Point3 p)
		{
			int index = GetIndexOf(p.X, p.Y, p.Z);
			return _data[index];
		}

		public override Tile GetSafe(int x, int y, int z)
		{
			if (!IsWithinLocalBounds(x, y, z))
				return null;
			else
				return Get(x, y, z);
		}

		public override Tile GetSafe(Point3 p)
		{
			if (!IsWithinLocalBounds(p.X, p.Y, p.Z))
				return null;
			else
				return Get(p.X, p.Y, p.Z);
		}

		private int GetIndexOf(int x, int y, int z)
		{
			return (y * _width * _depth) + (z * _width) + x;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (Mesh != null)
				Mesh.Dispose();
			if (AlphaMesh != null)
				AlphaMesh.Dispose();
		}

		#endregion
	}
}

