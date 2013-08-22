using System;

namespace Blarg.GameFramework.Support
{
	// this class exists because i am stupid and can never remember the
	// operators used for setting/clearing/toggling bits
	//
	// ... and of course, you can't use generics with bitwise operations :(

	public static class BitExtensions
	{
		#region Is Set

		public static bool IsBitSet(this long bitfield, long bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this ulong bitfield, ulong bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this int bitfield, int bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this uint bitfield, uint bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this short bitfield, short bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this ushort bitfield, ushort bit)
		{
			return (bitfield & bit) != 0;
		}

		public static bool IsBitSet(this byte bitfield, byte bit)
		{
			return (bitfield & bit) != 0;
		}

		#endregion

		#region Set

		public static long SetBit(this long bitfield, long bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static ulong SetBit(this ulong bitfield, ulong bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static int SetBit(this int bitfield, int bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static uint SetBit(this uint bitfield, uint bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static short SetBit(this short bitfield, short bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static ushort SetBit(this ushort bitfield, ushort bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		public static byte SetBit(this byte bitfield, byte bit)
		{
			bitfield |= bit;
			return bitfield;
		}

		#endregion

		#region Toggle

		public static long ToggleBit(this long bitfield, long bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static ulong ToggleBit(this ulong bitfield, ulong bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static int ToggleBit(this int bitfield, int bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static uint ToggleBit(this uint bitfield, uint bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static short ToggleBit(this short bitfield, short bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static ushort ToggleBit(this ushort bitfield, ushort bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		public static byte ToggleBit(this byte bitfield, byte bit)
		{
			bitfield ^= bit;
			return bitfield;
		}

		#endregion

		#region Clear

		public static long ClearBit(this long bitfield, long bit)
		{
			bitfield &= ~bit;
			return bitfield;
		}

		public static ulong ClearBit(this ulong bitfield, ulong bit)
		{
			bitfield &= ~bit;
			return bitfield;
		}

		public static int ClearBit(this int bitfield, int bit)
		{
			bitfield &= ~bit;
			return bitfield;
		}

		public static uint ClearBit(this uint bitfield, uint bit)
		{
			bitfield &= ~bit;
			return bitfield;
		}

		public static short ClearBit(this short bitfield, short bit)
		{
			bitfield &= (short)~bit;
			return bitfield;
		}

		public static ushort ClearBit(this ushort bitfield, ushort bit)
		{
			bitfield &= (ushort)~bit;
			return bitfield;
		}

		public static byte ClearBit(this byte bitfield, byte bit)
		{
			bitfield &= (byte)~bit;
			return bitfield;
		}

		#endregion
	}
}

