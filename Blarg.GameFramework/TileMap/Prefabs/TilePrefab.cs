using System;

namespace Blarg.GameFramework.TileMap.Prefabs
{
	public class TilePrefab : TileContainer, TileRawDataContainer
	{
		Tile[] _data;
		int _width;
		int _height;
		int _depth;
		BoundingBox _bounds;

		Rotation _rotation;
		int _rotationWidth;
		int _rotationDepth;
		int _rotationXOffset;
		int _rotationZOffset;
		int _rotationXMultiplier;
		int _rotationZMultiplier;
		int _rotationXPreMultiplier;
		int _rotationZPreMultiplier;
		BoundingBox _rotationBounds;

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
			get { return 0; }
		}

		public override int MinY
		{
			get { return 0; }
		}

		public override int MinZ
		{
			get { return 0; }
		}

		public override int MaxX
		{
			get { return _width - 1; }
		}

		public override int MaxY
		{
			get { return _height - 1; }
		}

		public override int MaxZ
		{
			get { return _depth - 1; }
		}

		public override Vector3 Position
		{
			get { return Vector3.Zero; }
		}

		public override BoundingBox Bounds
		{
			get { return _bounds; }
		}

		public Rotation Rotation
		{
			get { return _rotation; }
		}

		public int RotatedWidth
		{
			get { return _rotationWidth; }
		}

		public int RotatedDepth
		{
			get { return _rotationDepth; }
		}

		public BoundingBox RotatedBounds
		{
			get { return _rotationBounds; }
		}

		public Tile[] Data
		{
			get { return _data; }
		}

		public TilePrefab(int width, int height, int depth)
		{
			if (width <= 0)
				throw new ArgumentException("width");
			if (height <= 0)
				throw new ArgumentException("height");
			if (depth <= 0)
				throw new ArgumentException("depth");

			_bounds = new BoundingBox();
			_bounds.Min.Set(Vector3.Zero);
			_bounds.Max.Set(width, height, depth);

			int numTiles = width * height * depth;
			_data = new Tile[numTiles];
			for (int i = 0; i < numTiles; ++i)
				_data[i] = new Tile();

			Rotate(Rotation.Rotation0);
		}

		public override Tile Get(int x, int y, int z)
		{
			int index = GetIndexOf(x, y, z);
			return _data[index];
		}

		public override Tile GetSafe(int x, int y, int z)
		{
			if (!IsWithinLocalBounds(x, y, z))
				return null;
			else
				return Get(x, y, z);
		}

		private int GetIndexOf(int x, int y, int z)
		{
			return (y * _width * _depth) + (z * _width) + x;
		}

		public void Rotate(Rotation rotation)
		{
			_rotation = rotation;

			switch (rotation)
			{
				case Rotation.Rotation0:
					_rotationWidth = _width;
					_rotationDepth = _depth;
					_rotationXOffset = 0;
					_rotationZOffset = 0;
					_rotationXMultiplier = 1;
					_rotationZMultiplier = _rotationWidth;
					_rotationXPreMultiplier = 1;
					_rotationZPreMultiplier = 1;
					_rotationBounds.Min.Set(Vector3.Zero);
					_rotationBounds.Max.Set(_rotationWidth, _height, _rotationDepth);
					break;
				case Rotation.Rotation90:
					_rotationWidth = _depth;
					_rotationDepth = _width;
					_rotationXOffset = _rotationWidth - 1;
					_rotationZOffset = 0;
					_rotationXMultiplier = _rotationDepth;
					_rotationZMultiplier = 1;
					_rotationXPreMultiplier = -1;
					_rotationZPreMultiplier = 1;
					_rotationBounds.Min.Set(Vector3.Zero);
					_rotationBounds.Max.Set(_rotationWidth, _height, _rotationDepth);
					break;
				case Rotation.Rotation180:
					_rotationWidth = _width;
					_rotationDepth = _depth;
					_rotationXOffset = _rotationWidth - 1;
					_rotationZOffset = _rotationDepth - 1;
					_rotationXMultiplier = 1;
					_rotationZMultiplier = _rotationWidth;
					_rotationXPreMultiplier = -1;
					_rotationZPreMultiplier = -1;
					_rotationBounds.Min.Set(Vector3.Zero);
					_rotationBounds.Max.Set(_rotationWidth, _height, _rotationDepth);
					break;
				case Rotation.Rotation270:
					_rotationWidth = _depth;
					_rotationDepth = _width;
					_rotationXOffset = 0;
					_rotationZOffset = _rotationDepth - 1;
					_rotationXMultiplier = _rotationDepth;
					_rotationZMultiplier = 1;
					_rotationXPreMultiplier = 1;
					_rotationZPreMultiplier = -1;
					_rotationBounds.Min.Set(Vector3.Zero);
					_rotationBounds.Max.Set(_rotationWidth, _height, _rotationDepth);
					break;
			}
		}

		public void PlaceIn(TileContainer destination, int minX, int minY, int minZ, Rotation rotation, bool copyEmptyTiles = false)
		{
			if (destination == null)
				throw new ArgumentNullException("destination");

			if (_rotation != rotation)
				Rotate(rotation);
			if (!((minX + _rotationWidth) < destination.Width))
				throw new ArgumentException("Destination not large enough.");
			if (!((minY + _height) < destination.Height))
				throw new ArgumentException("Destination not large enough.");
			if (!((minZ + _rotationDepth) < destination.Depth))
				throw new ArgumentException("Destination not large enough.");

			for (int y = 0; y < _height; ++y)
			{
				for (int z = 0; z < _rotationDepth; ++z)
				{
					for (int x = 0; x < _rotationWidth; ++x)
					{
						var sourceTile = GetWithRotation(x, y, z);
						if (!copyEmptyTiles && sourceTile.IsEmptySpace)
							continue;

						// copy it right away, any modifications that we need to do as part of this copy
						// should only be done against destTile (leave the source data intact! herp derp, references!)
						var destTile = destination.Get(minX + x, minY + y, minZ + z);
						destTile.Set(sourceTile);

						if (destTile.IsRotated)
							destTile.RotateClockwise((int)rotation / 90);
					}
				}
			}
		}

		public Tile GetWithRotation(int x, int y, int z)
		{
			int index = GetIndexOfWithRotation(x, y, z);
			return _data[index];
		}

		private int GetIndexOfWithRotation(int x, int y, int z)
		{
			return 
				(y * _rotationWidth * _rotationDepth)
				+ ((_rotationZPreMultiplier * z + _rotationZOffset) * _rotationZMultiplier)
				+ ((_rotationXPreMultiplier * x + _rotationXOffset) * _rotationXMultiplier);
		}
		}
}

