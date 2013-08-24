using System;
using System.Text;
using Blarg.GameFramework;
using Blarg.GameFramework.Events;
using Blarg.GameFramework.Graphics;
using Blarg.GameFramework.Processes;
using Blarg.GameFramework.Support;

namespace Game
{
	public class DebugInfoProcess : GameProcess
	{
		StringBuilder _sb = new StringBuilder(1024);
		SpriteBatch _spriteBatch;

		public DebugInfoProcess(ProcessManager processManager, EventManager eventManager)
			: base(processManager, eventManager)
		{
			_spriteBatch = Framework.Services.Get<SpriteBatch>();
		}

		public override void OnRender(float delta)
		{
			base.OnRender(delta);

			long gcmem = GC.GetTotalMemory(false);
			_sb.Clear();
			_sb.Append("GC Mem Usage: ").AppendNumber((int)gcmem).Append('\n');
			_sb.Append("FPS: ").AppendNumber(Framework.Application.FPS);
			_sb.Append(", ").AppendNumber(Framework.Application.FrameTime).Append(" ms");
			_sb.Append(", RT: ").AppendNumber(Framework.Application.RenderTime).Append(" (").AppendNumber(Framework.Application.RendersPerSecond).Append(")");
			_sb.Append(", UT: ").AppendNumber(Framework.Application.UpdateTime).Append(" (").AppendNumber(Framework.Application.UpdatesPerSecond).Append(")");
			_sb.Append(", RD: ").AppendNumber(delta);

			_spriteBatch.Begin();
			_spriteBatch.Render(Framework.GraphicsDevice.SansSerifFont, 10, 10, Color.White, _sb);
			_spriteBatch.End();
		}
	}
}

