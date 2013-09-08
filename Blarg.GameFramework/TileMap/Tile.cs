using System;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.TileMap
{
	public class Tile
	{
		static readonly Matrix4x4 FaceNorthRotation = Matrix4x4.CreateRotationY(MathConstants.Radians0);
		static readonly Matrix4x4 FaceEastRotation = Matrix4x4.CreateRotationY(MathConstants.Radians90);
		static readonly Matrix4x4 FaceSouthRotation = Matrix4x4.CreateRotationY(MathConstants.Radians180);
		static readonly Matrix4x4 FaceWestRotation = Matrix4x4.CreateRotationY(MathConstants.Radians270);

		public const byte ROTATION_0 = 0;
		public const byte ROTATION_90 = 1;
		public const byte ROTATION_180 = 2;
		public const byte ROTATION_270 = 3;

		public const short NO_TILE = 0;

		public const byte LIGHT_VALUE_MAX = 15;
		public const byte LIGHT_VALUE_SKY = LIGHT_VALUE_MAX;

		public const short FLAG_COLLIDEABLE = 1;
		public const short FLAG_ROTATED = 2;
		public const short FLAG_LARGE_TILE = 4;
		public const short FLAG_LARGE_TILE_OWNER = 8;
		public const short FLAG_CUSTOM_COLOR = 16;
		public const short FLAG_FRICTION_SLIPPERY = 32;
		public const short FLAG_LIGHT_SKY = 64;
		public const short FLAG_WALKABLE_SURFACE = 128;

		public short TileIndex;
		public short Flags;
		public byte TileLight;
		public byte SkyLight;
		public byte Rotation;
		public byte ParentTileOffsetX;
		public byte ParentTileOffsetY;
		public byte ParentTileOffsetZ;
		public byte ParentTileWidth;
		public byte ParentTileHeight;
		public byte ParentTileDepth;
		public int Color;

		public float Brightness
		{
			get
			{
				if (TileLight > SkyLight)
					return GetBrightness(TileLight);
				else
					return GetBrightness(SkyLight);
			}
		}

		public float RotationAngle
		{
			get
			{
				if (Rotation < 0 || Rotation > 3)
					return 0.0f;
				else
					return Rotation * MathConstants.Radians90;
			}
		}

		public bool IsEmptySpace
		{
			get { return TileIndex == NO_TILE; }
		}

		public bool IsCollideable
		{
			get { return Flags.IsBitSet(FLAG_COLLIDEABLE); }
		}

		public bool HasCustomColor
		{
			get { return Flags.IsBitSet(FLAG_CUSTOM_COLOR); }
		}

		public bool IsSlippery
		{
			get { return Flags.IsBitSet(FLAG_FRICTION_SLIPPERY); }
		}

		public bool IsSkyLit
		{
			get { return Flags.IsBitSet(FLAG_LIGHT_SKY); }
		}

		public bool IsRotated
		{
			get { return Flags.IsBitSet(FLAG_ROTATED); }
		}

		public bool IsLargeTile
		{
			get { return Flags.IsBitSet(FLAG_LARGE_TILE); }
		}

		public bool IsLargeTileRoot
		{
			get { return Flags.IsBitSet(FLAG_LARGE_TILE_OWNER); }
		}

		public Tile()
		{
			TileIndex = NO_TILE;
		}

		public Tile Set(short tileIndex)
		{
			TileIndex = tileIndex;
			return this;
		}

		public Tile Set(short tileIndex, short flags)
		{
			TileIndex = tileIndex;
			Flags = flags;
			return this;
		}

		public Tile Set(short tileIndex, short flags, Color color)
		{
			return Set(tileIndex, flags, color.RGBA);
		}

		public Tile Set(short tileIndex, short flags, int color)
		{
			TileIndex = tileIndex;
			Flags = Flags.SetBit(FLAG_CUSTOM_COLOR);
			Color = color;
			return this;
		}

		public Tile Set(Tile other)
		{
			TileIndex = other.TileIndex;
			Flags = other.Flags;
			TileLight = other.TileLight;
			SkyLight = other.SkyLight;
			Rotation = other.Rotation;
			ParentTileOffsetX = other.ParentTileOffsetX;
			ParentTileOffsetY = other.ParentTileOffsetY;
			ParentTileOffsetZ = other.ParentTileOffsetZ;
			ParentTileWidth = other.ParentTileWidth;
			ParentTileHeight = other.ParentTileHeight;
			ParentTileDepth = other.ParentTileDepth;
			Color = other.Color;
			return this;
		}

		public Tile Clear()
		{
			TileIndex = NO_TILE;
			Flags = 0;
			TileLight = 0;
			SkyLight = 0;
			Rotation = 0;
			ParentTileOffsetX = 0;
			ParentTileOffsetY = 0;
			ParentTileOffsetZ = 0;
			ParentTileWidth = 0;
			ParentTileHeight = 0;
			ParentTileDepth = 0;
			Color = 0;
			return this;
		}

		public Tile SetCustomColor(Color color)
		{
			return SetCustomColor(color.RGBA);
		}

		public Tile SetCustomColor(int color)
		{
			Flags = Flags.SetBit(FLAG_CUSTOM_COLOR);
			Color = color;
			return this;
		}

		public Tile ClearCustomColor()
		{
			Flags = Flags.ClearBit(FLAG_CUSTOM_COLOR);
			Color = 0;
			return this;
		}

		public Tile Rotate(byte facingDirection)
		{
			if (facingDirection < 0 || facingDirection > 3)
				throw new ArgumentException("Use one of the ROTATION_X constants.");
			Flags = Flags.SetBit(FLAG_ROTATED);
			Rotation = facingDirection;
			return this;
		}

		public Tile RotateClockwise()
		{
			Flags = Flags.SetBit(FLAG_ROTATED);
			Rotation -= 1;
			if (Rotation < ROTATION_0)
				Rotation = ROTATION_270;
			return this;
		}

		public Tile RotateClockwise(int times)
		{
			Flags = Flags.SetBit(FLAG_ROTATED);
			Rotation = (byte)MathHelpers.RolloverClamp((int)Rotation - times, (int)ROTATION_0, (int)ROTATION_270 + 1);
			return this;
		}

		public Tile RotateCounterClockwise()
		{
			Flags = Flags.SetBit(FLAG_ROTATED);
			Rotation += 1;
			if (Rotation > ROTATION_270)
				Rotation = ROTATION_0;
			return this;
		}

		public Tile RotateCounterClockwise(int times)
		{
			Flags = Flags.SetBit(FLAG_ROTATED);
			Rotation = (byte)MathHelpers.RolloverClamp((int)Rotation + times, (int)ROTATION_0, (int)ROTATION_270 + 1);
			return this;
		}

		public static float GetBrightness(byte light)
		{
			// this is a copy of the brightness formula listed here:
			// http://gamedev.stackexchange.com/a/21247
			const float BASE_BRIGHTNESS = 0.086f;

			float normalizedLightValue = (float)light / (float)(LIGHT_VALUE_MAX + 1);
			return (float)Math.Pow((float)normalizedLightValue, 1.4f) + BASE_BRIGHTNESS;
		}

		public static byte AdjustLightForTranslucency(byte light, float translucency)
		{
			return (byte)Math.Round((float)light * (1.0f - translucency));
		}

		public static bool GetTransformationFor(Tile tile, ref Matrix4x4 rotation)
		{
			if (!tile.IsRotated)
				return false;

			switch (tile.Rotation)
			{
				case 0:
					rotation = FaceNorthRotation;
					return true;
				case 1:
					rotation = FaceEastRotation;
					return true;
				case 2:
					rotation = FaceSouthRotation;
					return true;
				case 3:
					rotation = FaceWestRotation;
					return true;
				default:
					return false;
			}
		}

		public static Matrix4x4? GetTransformationFor(Tile tile)
		{
			if (!tile.IsRotated)
				return null;

			switch (tile.Rotation)
			{
				case 0: return FaceNorthRotation;
				case 1: return FaceEastRotation;
				case 2: return FaceSouthRotation;
				case 3: return FaceWestRotation;
				default: return null;
			}	
		}
	}
}

