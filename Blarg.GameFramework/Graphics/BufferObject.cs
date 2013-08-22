using System;
using PortableGL;

namespace Blarg.GameFramework.Graphics
{
	public enum BufferObjectType
	{
		Vertex,
		Index
	}

	public enum BufferObjectUsage
	{
		Static,
		Stream,
		Dynamic
	}

	public abstract class BufferObject<T> : GraphicsContextResource where T : struct
	{
		public int ID { get; private set; }
		public bool IsClientSide { get; private set; }
		public bool IsDirty { get; protected set; }
		public BufferObjectType Type { get; private set; }
		public BufferObjectUsage Usage { get; private set; }
		public int SizeInBytes { get; private set; }

		public bool IsInvalidated
		{
			get { return ID == -1; }
		}

		public abstract int NumElements { get; }
		public abstract int ElementWidthInBytes { get; }
		public abstract T[] Data { get; }

		public BufferObject(BufferObjectType type, BufferObjectUsage usage)
		: base()
		{
			Initialize(type, usage);
		}

		public BufferObject(GraphicsDevice graphicsDevice, BufferObjectType type, BufferObjectUsage usage)
		: base(graphicsDevice)
		{
			Initialize(type, usage);
		}

		private void Initialize(BufferObjectType type, BufferObjectUsage usage)
		{
			Type = type;
			Usage = usage;
			ID = -1;
			IsClientSide = true;
			IsDirty = false;
		}

		protected void CreateOnGpu()
		{
			if (GraphicsDevice == null)
				throw new InvalidOperationException("Buffer object was not created with a GraphicsDevice object.");
			if (!IsInvalidated)
				throw new InvalidOperationException();
			CreateBufferObject();
		}

		protected void RecreateOnGpu()
		{
			if (GraphicsDevice == null)
				throw new InvalidOperationException("Buffer object was not created with a GraphicsDevice object.");
			if (IsInvalidated)
				throw new InvalidOperationException();
			FreeBufferObject();
			CreateBufferObject();
		}

		protected void FreeFromGpu()
		{
			if (GraphicsDevice == null)
				throw new InvalidOperationException("Buffer object was not created with a GraphicsDevice object.");
			if (IsInvalidated)
				throw new InvalidOperationException();
			FreeBufferObject();
		}

		public void Update()
		{
			if (!IsDirty)
				return;
			if (IsClientSide)
			{
				// pretend we updated! (I guess this is pointless anyway)
				IsDirty = false;
				return;
			}

			if (IsInvalidated)
				throw new InvalidOperationException();
			if (NumElements <= 0)
				throw new InvalidOperationException();
			if (ElementWidthInBytes <= 0)
				throw new InvalidOperationException();

			int currentSizeInBytes = NumElements * ElementWidthInBytes;

			var usage = GLUsageHint;
			var target = GLTarget;

			Platform.GraphicsDevice.GL.glBindBuffer(target, ID);

			if (SizeInBytes != currentSizeInBytes)
			{
				// means that the buffer object hasn't been allocated. So let's allocate and update at the same time
				// figure out the size...
				SizeInBytes = currentSizeInBytes;

				// and then allocate + update
				Platform.GraphicsDevice.GL.glBufferData<T>(target, SizeInBytes, Data, usage);
			}
			else
			{
				// possible performance enhancement? passing a NULL pointer to
				// glBufferData tells the driver that we don't care about the buffer's
				// previous contents allowing it to do some extra optimizations which is
				// fine since our glBufferSubData call is going to completely replace 
				// the contents anyway
				Platform.GraphicsDevice.GL.glBufferData(target, SizeInBytes, IntPtr.Zero, usage);
				Platform.GraphicsDevice.GL.glBufferSubData<T>(target, 0, SizeInBytes, Data);
			}

			Platform.GraphicsDevice.GL.glBindBuffer(target, 0);

			IsDirty = false;
		}

		protected void CreateBufferObject()
		{
			ID = Platform.GraphicsDevice.GL.glGenBuffers();

			SizeBufferObject();

			IsDirty = true;
			IsClientSide = false;
		}

		protected void FreeBufferObject()
		{
			if (IsInvalidated)
				throw new InvalidOperationException();

			Platform.GraphicsDevice.GL.glDeleteBuffers(ID);

			ID = -1;
			IsClientSide = true;
			IsDirty = false;
			SizeInBytes = 0;
		}

		protected void SizeBufferObject()
		{
			if (IsInvalidated)
				throw new InvalidOperationException();
			if (NumElements <= 0)
				throw new InvalidOperationException();
			if (ElementWidthInBytes <= 0)
				throw new InvalidOperationException();

			var usage = GLUsageHint;
			var target = GLTarget;

			SizeInBytes = NumElements * ElementWidthInBytes;

			// resize the buffer object without initializing it's data
			Platform.GraphicsDevice.GL.glBindBuffer(target, ID);
			Platform.GraphicsDevice.GL.glBufferData(target, SizeInBytes, IntPtr.Zero, usage);
			Platform.GraphicsDevice.GL.glBindBuffer(target, 0);

			IsDirty = true;
		}

		private int GLUsageHint
		{
			get
			{
				if (Usage == BufferObjectUsage.Static)
					return GL20.GL_STATIC_DRAW;
				else if (Usage == BufferObjectUsage.Stream)
					return GL20.GL_STREAM_DRAW;
				else if (Usage == BufferObjectUsage.Dynamic)
					return GL20.GL_DYNAMIC_DRAW;
				else
					throw new InvalidOperationException();
			}
		}

		private int GLTarget
		{
			get
			{
				if (Type == BufferObjectType.Index)
					return GL20.GL_ELEMENT_ARRAY_BUFFER;
				else if (Type == BufferObjectType.Vertex)
					return GL20.GL_ARRAY_BUFFER;
				else
					throw new InvalidOperationException();
			}
		}

		#region GraphicsContextResource

		public override void OnNewContext()
		{
			RecreateOnGpu();
		}

		public override void OnLostContext()
		{
		}

		protected override bool ReleaseResource()
		{
			if (!IsInvalidated && !IsClientSide)
			{
				FreeFromGpu();
				ID = -1;
			}

			return true;
		}

		#endregion
	}
}
