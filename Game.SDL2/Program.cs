using System;
using Blarg.GameFramework;

namespace Game.SDL2
{
	public static class MainClass
	{
		public static void Main(string[] args)
		{
			var game = new GameApp();
			var config = new SDLConfiguration();
			config.Title = "Test Game";

			var looper = new SDLLooper();
			looper.Run(game, config);
		}
	}
}
