using System;
using System.Collections.Generic;
using Blarg.GameFramework.Graphics;

namespace Blarg.GameFramework.TileMap.Meshes.Json
{
	public class JsonTileDefinition
	{
		public bool Cube;
		public JsonCubeTextures Textures;
		public int? Texture;
		public List<string> Faces;
		public string Model;
		public List<JsonTileSubModel> Models;
		public string CollisionModel;
		public string CollisionShape;
		public List<string> OpaqueSides;
		public byte Light;
		public bool Alpha;
		public float Translucency;
		public Color? Color;
		public Vector3? ScaleToSize;
		public Vector3? PositionOffset;
		public Vector3? CollisionPositionOffset;
	}
}

