using System;
using Blarg.GameFramework;

namespace Game.SDL2
{
	public static class MainClass
	{
		public static void Main(string[] args)
		{
			var config = new SDLConfiguration();
			config.Title = "Test Game";
			config.Resizeable = true;

			var app = new SDLApplication();
			app.Run(new GameApp(), config);
			app.Dispose();
		}
	}
}
