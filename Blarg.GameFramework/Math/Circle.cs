using System;
using System.Runtime.InteropServices;

namespace Blarg.GameFramework
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Circle : IEquatable<Circle>
	{
		public int X;
		public int Y;
		public int Radius;

		public int Diameter
		{
			get { return Radius * 2; }
		}

		public Circle(int x, int y, int radius)
		{
			X = x;
			Y = y;
			Radius = radius;
		}

		public bool Contains(int x, int y)
		{
			int squaredDistance = ((X - x) * (X - x)) + ((Y - y) * (Y - y));
			int squaredRadius = Radius * Radius;
			if (squaredDistance <= squaredRadius)
				return true;
			else
				return false;
		}

		public static bool operator ==(Circle left, Circle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Circle left, Circle right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Circle)
				return this.Equals((Circle)obj);
			else
				return false;
		}

		public bool Equals(Circle other)
		{
			return (X == other.X && Y == other.Y && Radius == other.Radius);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode() ^ Radius.GetHashCode();
		}

		public override string ToString()
		{
			return String.Format("{{Center:{0},{1} Radius:{2}}}", X, Y, Radius);
		}
	}
}
