using System;

namespace Blarg.GameFramework.Graphics
{
	public class IndexBuffer : BufferObject<ushort>
	{
		ushort[] _buffer;

		public int CurrentPosition { get; private set; }

		public int RemainingElements
		{
			get { return (NumElements - 1) - CurrentPosition; }
		}

		public override int NumElements
		{
			get { return _buffer.Length; }
		}

		public override int ElementWidthInBytes
		{
			get { return sizeof(ushort); }
		}

		public override ushort[] Data
		{
			get { return _buffer; }
		}

		public IndexBuffer(int numIndices, BufferObjectUsage usage)
			: base(BufferObjectType.Index, usage)
		{
			Initialize(numIndices);
		}

		public IndexBuffer(GraphicsDevice graphicsDevice, int numIndices, BufferObjectUsage usage)
			: base(graphicsDevice, BufferObjectType.Index, usage)
		{
			Initialize(numIndices);
			CreateOnGpu();
		}

		public IndexBuffer(IndexBuffer source)
			: base(BufferObjectType.Index, source.Usage)
		{
			Initialize(source);
		}

		public IndexBuffer(GraphicsDevice graphicsDevice, IndexBuffer source)
			: base(graphicsDevice, BufferObjectType.Index, source.Usage)
		{
			Initialize(source);
			CreateOnGpu();
		}

		private void Initialize(int numIndices)
		{
			Resize(numIndices);
		}

		private void Initialize(IndexBuffer source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (source.NumElements <= 0)
				throw new InvalidOperationException();

			Resize(source.NumElements);
			Array.Copy(source.Data, 0, _buffer, 0, NumElements);
		}

		public void Set(ushort[] indices)
		{
			if (indices == null)
				throw new ArgumentNullException("indices");
			if (indices.Length == 0)
				throw new InvalidOperationException();
			if (indices.Length > _buffer.Length)
				throw new InvalidOperationException();

			Array.Copy(indices, 0, _buffer, 0, indices.Length);
		}

		public void Set(int index, ushort value)
		{
			_buffer[index] = value;
		}

		public bool MoveNext()
		{
			++CurrentPosition;
			if (CurrentPosition >= NumElements)
			{
				--CurrentPosition;
				return false;
			}
			else
				return true;
		}

		public bool MovePrevious()
		{
			if (CurrentPosition == 0)
				return false;
			else
			{
				--CurrentPosition;
				return true;
			}
		}

		public void Move(int amount)
		{
			CurrentPosition += amount;
			if (CurrentPosition < 0)
				CurrentPosition = 0;
			else if (CurrentPosition >= NumElements)
				CurrentPosition = NumElements - 1;
		}

		public void MoveToStart()
		{
			CurrentPosition = 0;
		}

		public void MoveToEnd()
		{
			CurrentPosition = NumElements - 1;
		}

		public void MoveTo(int index)
		{
			CurrentPosition = index;
		}

		public void SetCurrent(ushort value)
		{
			Set(CurrentPosition, value);
		}

		public void Resize(int numIndices)
		{
			if (numIndices <= 0)
				throw new ArgumentOutOfRangeException("numIndices");

			if (_buffer == null)
				_buffer = new ushort[numIndices];
			else
				Array.Resize(ref _buffer, numIndices);

			if (!IsClientSide)
				SizeBufferObject();

			if (CurrentPosition >= NumElements)
				CurrentPosition = NumElements - 1;
		}

		public void Extend(int amount)
		{
			int newSize = NumElements + amount;
			if (newSize <= 0)
				throw new ArgumentOutOfRangeException("amount");

			Resize(newSize);
		}
	}
}
