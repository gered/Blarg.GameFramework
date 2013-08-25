using System;
using System.Collections.Generic;
using Blarg.GameFramework.Content;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.Graphics.Atlas
{
	public class TextureAtlasAnimator : IDisposable
	{
		Dictionary<string, TextureAtlasTileAnimation> _animations;
		ContentManager _contentManager;

		public TextureAtlasAnimator()
		{
			_contentManager = Framework.Services.Get<ContentManager>();
			if (_contentManager == null)
				throw new InvalidOperationException("Could not find ContentManager object.");

			_animations = new Dictionary<string, TextureAtlasTileAnimation>();
		}

		public void AddSequence(string name, TextureAtlas atlas, int tileToBeAnimated, int start, int stop, float delay, bool loop)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("name");
			if (atlas == null)
				throw new ArgumentNullException("atlas");
			if (start >= atlas.NumTiles)
				throw new InvalidOperationException();
			if (stop >= atlas.NumTiles)
				throw new InvalidOperationException();
			if (start >= stop)
				throw new InvalidOperationException();
			if (tileToBeAnimated >= start && tileToBeAnimated <= stop)
				throw new InvalidOperationException();

			if (_animations.ContainsKey(name))
				return;

			var sequence = new TextureAtlasTileAnimation();
			sequence.Atlas = atlas;
			sequence.AnimatingIndex = tileToBeAnimated;
			sequence.Start = start;
			sequence.Stop = stop;
			sequence.Delay = delay;
			sequence.IsAnimating = true;
			sequence.Loop = loop;
			sequence.Frames = new Image[sequence.NumFrames];
			sequence.Name = name;

			sequence.Current = sequence.Start;
			sequence.CurrentFrameTime = 0.0f;

			// since we can't read a texture back from OpenGL after we've uploaded it
			// (??? or can we somehow .. ?), we need to load the image again so that
			// we can copy out the image data for tiles "start" to "stop"
			string textureFilename = _contentManager.GetNameOf<Texture>(atlas.Texture);
			if (String.IsNullOrEmpty(textureFilename))
				throw new InvalidOperationException("Texture atlas is using a texture not loaded via ContentManager. Cannot automatically obtain backing Image asset.");
			var textureImage = _contentManager.Get<Image>(textureFilename);
			if (textureImage == null)
				throw new InvalidOperationException("Could not obtain backing Image asset for this texture atlas.");

			// first, copy the original tile image that's at the "tileToBeAnimated" spot
			// in the atlas texture so we can restore it back again if necessary
			var originalTile = atlas.GetTile(sequence.AnimatingIndex);
			sequence.OriginalAnimatingTile = new Image(textureImage, originalTile.Dimensions);

			// copy each frame ("start" to "stop") from the source texture image
			for (int i = 0; i < sequence.NumFrames; ++i)
			{
				var tile = atlas.GetTile(i + sequence.Start);
				sequence.Frames[i] = new Image(textureImage, tile.Dimensions);
			}

			_animations.Add(name, sequence);
		}

		public void RemoveSequence(string name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("name");

			var sequence = _animations.Get(name);
			if (sequence == null)
				throw new InvalidOperationException("Sequence not found.");

			RestoreTextureWithOriginalTile(sequence);
			_animations.Remove(name);
		}

		public void ResetSequence(string name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("name");

			var sequence = _animations.Get(name);
			if (sequence == null)
				throw new InvalidOperationException("Sequence not found.");

			sequence.IsAnimating = true;
			sequence.Current = sequence.Start;
			sequence.CurrentFrameTime = 0.0f;

			UpdateTextureWithCurrentTileFrame(sequence);
		}

		public void StopSequence(string name, bool restoreOriginalTile = false)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("name");

			var sequence = _animations.Get(name);
			if (sequence == null)
				throw new InvalidOperationException("Sequence not found.");

			sequence.IsAnimating = false;
			sequence.Current = sequence.Stop;
			sequence.CurrentFrameTime = 0.0f;

			if (restoreOriginalTile)
				RestoreTextureWithOriginalTile(sequence);
			else
				UpdateTextureWithCurrentTileFrame(sequence);
		}

		public void ToggleSequence(string name, bool enable)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("name");

			var sequence = _animations.Get(name);
			if (sequence == null)
				throw new InvalidOperationException("Sequence not found.");

			sequence.IsAnimating = enable;
			sequence.CurrentFrameTime = 0.0f;

			UpdateTextureWithCurrentTileFrame(sequence);
		}

		public void OnUpdate(float delta)
		{
			foreach (var i in _animations)
			{
				var sequence = i.Value;

				if (!sequence.IsAnimationFinished && sequence.IsAnimating)
				{
					sequence.CurrentFrameTime += delta;

					if (sequence.CurrentFrameTime >= sequence.Delay)
					{
						// move to the next frame
						sequence.CurrentFrameTime = 0.0f;

						++sequence.Current;
						if (sequence.Current > sequence.Stop)
							sequence.Current = sequence.Start;

						UpdateTextureWithCurrentTileFrame(sequence);
					}
				}
			}
		}

		public void OnNewContext()
		{
			foreach (var i in _animations)
				UpdateTextureWithCurrentTileFrame(i.Value);
		}

		private void UpdateTextureWithCurrentTileFrame(TextureAtlasTileAnimation sequence)
		{
			int frameIndex = sequence.Current - sequence.Start;
			var frameImage = sequence.Frames[frameIndex];
			var tile = sequence.Atlas.GetTile(sequence.AnimatingIndex);

			sequence.Atlas.Texture.Update(frameImage, tile.Dimensions.Left, tile.Dimensions.Top);
		}

		private void RestoreTextureWithOriginalTile(TextureAtlasTileAnimation sequence)
		{
			var tile = sequence.Atlas.GetTile(sequence.AnimatingIndex);
			sequence.Atlas.Texture.Update(sequence.OriginalAnimatingTile, tile.Dimensions.Left, tile.Dimensions.Top);
		}

		public void Dispose()
		{
			foreach (var i in _animations)
				RestoreTextureWithOriginalTile(i.Value);
			_animations.Clear(); 
		}
	}
}

