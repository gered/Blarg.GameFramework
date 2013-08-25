using System;

namespace Blarg.GameFramework.Graphics.Atlas
{
	internal class TextureAtlasTileAnimation
	{
		public TextureAtlas Atlas;
		public int AnimatingIndex;
		public int Start;
		public int Stop;
		public int Current;
		public float Delay;
		public float CurrentFrameTime;
		public bool IsAnimating;
		public bool Loop;
		public Image OriginalAnimatingTile;
		public Image[] Frames = null;
		public string Name;

		public int NumFrames
		{
			get { return Stop - Start + 1; }
		}

		public bool IsAnimationFinished
		{
			get { return (IsAnimating && !Loop && Current == Stop); }
		}
	}
}

