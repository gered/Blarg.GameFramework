using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct RectF : IEquatable<RectF>
	{
		public float Left;
		public float Top;
		public float Right;
		public float Bottom;

		public float Width
		{
			get { return Math.Abs(Right - Left); }
		}

		public float Height
		{
			get { return Math.Abs(Bottom - Top); }
		}

		public RectF(float left, float top, float right, float bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public RectF(ref RectF r)
		{
			Left = r.Left;
			Top = r.Top;
			Right = r.Right;
			Bottom = r.Bottom;
		}

		public RectF(RectF r)
		{
			Left = r.Left;
			Top = r.Top;
			Right = r.Right;
			Bottom = r.Bottom;
		}

		public void Set(float left, float top, float right, float bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public void Set(ref RectF r)
		{
			Left = r.Left;
			Top = r.Top;
			Right = r.Right;
			Bottom = r.Bottom;
		}

		public void Set(RectF r)
		{
			Left = r.Left;
			Top = r.Top;
			Right = r.Right;
			Bottom = r.Bottom;
		}

		public bool Contains(float x, float y)
		{
			if (x >= Left && y >= Top && x <= Right && y <= Bottom)
				return true;
			else
				return false;
		}

		public static bool operator ==(RectF left, RectF right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RectF left, RectF right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is RectF)
				return this.Equals((RectF)obj);
			else
				return false;
		}

		public bool Equals(RectF other)
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
