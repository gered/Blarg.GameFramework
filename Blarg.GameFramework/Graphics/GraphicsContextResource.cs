using System;

namespace Blarg.GameFramework.Graphics
{
	public abstract class GraphicsContextResource : IDisposable
	{
		public const string LOG_TAG = "GRAPHICS_RESOURCE";

		public GraphicsDevice GraphicsDevice { get; private set; }
		public bool IsReleased { get; private set; }

		public GraphicsContextResource()
		{
			GraphicsDevice = null;
		}

		public GraphicsContextResource(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			GraphicsDevice = graphicsDevice;
			GraphicsDevice.RegisterManagedResource(this);
		}

		public virtual void OnNewContext()
		{
		}

		public virtual void OnLostContext()
		{
		}

		~GraphicsContextResource()
		{
			if (!IsReleased)
			{
				ReleaseResource();
				if (GraphicsDevice != null)
					GraphicsDevice.UnregisterManagedResource(this);
			}
		}

		protected virtual bool ReleaseResource()
		{
			return true;
		}

		public void Dispose()
		{
			if (!IsReleased)
			{
				IsReleased = ReleaseResource();
				if (GraphicsDevice != null)
					GraphicsDevice.UnregisterManagedResource(this);
			}
			GC.SuppressFinalize(this);
		}
	}
}
