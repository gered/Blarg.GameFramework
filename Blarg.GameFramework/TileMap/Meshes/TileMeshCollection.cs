using System;
using System.Collections.Generic;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Graphics.Atlas;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.TileMap.Meshes
{
	public class TileMeshCollection
	{
		List<TileMesh> _meshes;

		public readonly TextureAtlas Atlas;

		public int Count
		{
			get { return _meshes.Count; }
		}

		public TileMesh this[int index]
		{
			get { return _meshes[index]; }
		}

		public TileMeshCollection(TextureAtlas atlas)
		{
			if (atlas == null)
				throw new ArgumentNullException("atlas");

			Atlas = atlas;
			_meshes = new List<TileMesh>();
			
			// the first mesh (index = 0) should always be a null one as this has special meaning
			// in other TileMap-related objects (basically, representing empty space)
			AddMesh(null);
		}

		public TileMesh Get(Tile tile)
		{
			if (tile == null)
				throw new ArgumentNullException("tile");
			return _meshes[tile.TileIndex];
		}

		public int AddCube(TextureRegion? topTexture,
		                   TextureRegion? bottomTexture,
		                   TextureRegion? frontTexture,
		                   TextureRegion? backTexture,
		                   TextureRegion? leftTexture,
		                   TextureRegion? rightTexture,
		                   byte opaqueSides,
		                   byte lightValue,
		                   bool alpha,
		                   float translucency,
		                   Color color
		                   )
		{
			byte faces = 0;
			if (topTexture != null)
				faces = faces.SetBit(TileMesh.SIDE_TOP);
			if (bottomTexture != null)
				faces = faces.SetBit(TileMesh.SIDE_BOTTOM);
			if (frontTexture != null)
				faces = faces.SetBit(TileMesh.SIDE_FRONT);
			if (backTexture != null)
				faces = faces.SetBit(TileMesh.SIDE_BACK);
			if (leftTexture != null)
				faces = faces.SetBit(TileMesh.SIDE_LEFT);
			if (rightTexture != null)
				faces = faces.SetBit(TileMesh.SIDE_RIGHT);

			return AddMesh(
				new CubeTileMesh(topTexture, 
			                     bottomTexture, 
			                     frontTexture, 
			                     backTexture, 
			                     leftTexture, 
			                     rightTexture,
			                     faces, 
			                     opaqueSides, 
			                     lightValue, 
			                     alpha, 
			                     translucency, 
			                     color
			));
		}

		public int AddCube(TextureRegion texture,
		                   byte faces,
		                   byte opaqueSides,
		                   byte lightValue,
		                   bool alpha,
		                   float translucency,
		                   Color color
		                   )
		{
			return AddMesh(
				new CubeTileMesh(texture,
			                     texture,
			                     texture,
			                     texture,
			                     texture,
			                     texture,
			                     faces,
			                     opaqueSides,
			                     lightValue,
			                     alpha,
			                     translucency,
			                     color
			));
		}

		private int AddMesh(TileMesh mesh)
		{
			_meshes.Add(mesh);
			return _meshes.Count - 1;
		}
	}
}

