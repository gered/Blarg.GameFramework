using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Rect : IEquatable<Rect>
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public int Width
		{
			get { return Math.Abs(Right - Left); }
		}

		public int Height
		{
			get { return Math.Abs(Bottom - Top); }
		}

		public Rect(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public Rect(ref Rect r)
		{
			Left = r.Left;
			Top = r.Top;
			Right = r.Right;
			Bottom = r.Bottom;
		}

		public Rect(Rect r)
		: this(ref r)
		{
		}

		public void Set(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public void Set(ref Rect r)
		{
			Left = r.Left;
			Top = r.Top;
			Right = r.Right;
			Bottom = r.Bottom;
		}

		public void Set(Rect r)
		{
			Set(ref r);
		}

		public bool Contains(int x, int y)
		{
			if (x >= Left && y >= Top && x <= Right && y <= Bottom)
				return true;
			else
				return false;
		}

		public static bool operator ==(Rect left, Rect right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Rect left, Rect right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Rect)
				return this.Equals((Rect)obj);
			else
				return false;
		}

		public bool Equals(Rect other)
		{
			return (Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom);
		}

		public override int GetHashCode()
		{
			return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{{0}-{1},{2}-{3}}}", Left, Top, Right, Bottom);
		}
	}
}
