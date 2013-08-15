using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework.Graphics
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Color : IEquatable<Color>
	{
		public float R;
		public float G;
		public float B;
		public float A;

		public const float AlphaTransparent = 0.0f;
		public const float AlphaOpaque = 1.0f;
		public const int AlphaTransparentInt = 0;
		public const int AlphaOpaqueInt = 255;

		public static readonly Color White = new Color(1.0f, 1.0f, 1.0f);
		public static readonly Color Red = new Color(1.0f, 0.0f, 0.0f);
		public static readonly Color Green = new Color(0.0f, 1.0f, 0.0f);
		public static readonly Color Blue = new Color(0.0f, 0.0f, 1.0f);
		public static readonly Color Yellow = new Color(1.0f, 1.0f, 0.0f);
		public static readonly Color Cyan = new Color(0.0f, 1.0f, 1.0f);
		public static readonly Color Magenta = new Color(1.0f, 0.0f, 1.0f);
		public static readonly Color Black = new Color(0.0f, 0.0f, 0.0f);

		public int IntR 
		{
			get { return (int)(R * 255); }
			set { R = (float)value / 255; }
		}

		public int IntG
		{
			get { return (int)(G * 255); }
			set { G = (float)value / 255; }
		}

		public int IntB
		{
			get { return (int)(B * 255); }
			set { B = (float)value / 255; }
		}

		public int IntA
		{
			get { return (int)(A * 255); }
			set { A = (float)value / 255; }
		}

		public int RGBA
		{
			get { return ToInt(R, G, B, A); }
		}

		public Color(float red, float green, float blue, float alpha = AlphaOpaque)
		{
			R = red;
			G = green;
			B = blue;
			A = alpha;
		}

		public Color(int red, int green, int blue, int alpha = AlphaOpaqueInt)
		{
			R = (float)red / 255;
			G = (float)green / 255;
			B = (float)blue / 255;
			A = (float)alpha / 255;
		}

		public static void FromInt(int color, out int red, out int green, out int blue, out int alpha)
		{
			alpha = (int)((uint)(color & 0xff000000) >> 24);
			red = (color & 0x00ff0000) >> 16;
			green = (color & 0x0000ff00) >> 8;
			blue = (color & 0x000000ff);
		}

		public static void FromInt(int color, out float red, out float green, out float blue, out float alpha)
		{
			alpha = ((float)((color & 0xff000000) >> 24)) / 255;
			red = ((float)((color & 0x00ff0000) >> 16)) / 255;
			green = ((float)((color & 0x0000ff00) >> 8)) / 255;
			blue = ((float)((color & 0x000000ff))) / 255;
		}

		public static Color FromInt(int color)
		{
			Color result;
			FromInt(color, out result);
			return result;
		}

		public static void FromInt(int color, out Color result)
		{
			FromInt(color, out result.R, out result.G, out result.B, out result.A);
		}

		public static int ToInt(int red, int green, int blue, int alpha = AlphaOpaqueInt)
		{
			return (alpha << 24) | (red << 16) | (green << 8) | blue;
		}

		public static int ToInt(float red, float green, float blue, float alpha = AlphaOpaque)
		{
			return ((int)(alpha * 255) << 24) | ((int)(red * 255) << 16) | ((int)(green * 255) << 8) | (int)(blue * 255);
		}

		public static bool operator ==(Color left, Color right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Color left, Color right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Color)
				return this.Equals((Color)obj);
			else
				return false;
		}

		public bool Equals(Color other)
		{
			return (R == other.R && G == other.G && B == other.B && A == other.A);
		}

		public override int GetHashCode()
		{
			return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{{0}, {1}, {2}, {3}}}", IntR, IntG, IntB, IntA);
		}
	}
}
