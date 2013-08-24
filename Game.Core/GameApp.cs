using System;
using System.Text;
using PortableGL;
using Blarg.GameFramework;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Graphics.Helpers;
using Blarg.GameFramework.Support;

namespace Game
{
	public class GameApp : BasicGameApp
	{
		public override void OnLoad()
		{
			base.OnLoad();
			StateManager.Push<TestGameState>();
		}
	}
}

