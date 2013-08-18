using System;

namespace Blarg.GameFramework.Graphics
{
	public class VertexBuffer : BufferObject<float>
	{
		public struct AttributeInfo
		{
			public VertexAttributes type;
			public int offset;
			public int size;
		}

		private float[] _buffer;
		private int _standardVertexAttribs;
		private AttributeInfo[] _attributes;

		public int CurrentPosition { get; private set; }
		public int ElementWidth { get; private set; }

		public int RemainingElements
		{
			get { return (NumElements - 1) - CurrentPosition; }
		}

		public override int NumElements
		{
			get { return (_buffer.Length / ElementWidth); }
		}

		public override int ElementWidthInBytes
		{
			get { return ElementWidth * sizeof(float); }
		}

		public override float[] Data
		{
			get { return _buffer; }
		}

		public int NumAttributes
		{
			get { return _attributes.Length; }
		}

		public VertexAttributes GetAttributeType(int index)
		{
			return _attributes[index].type;
		}

		public int GetAttributeOffset(int index)
		{
			return _attributes[index].offset;
		}

		public int GetAttributeSize(int index)
		{
			return _attributes[index].size;
		}

		public int OffsetPosition2D { get; private set; }
		public int OffsetPosition3D { get; private set; }
		public int OffsetNormal { get; private set; }
		public int OffsetColor { get; private set; }
		public int OffsetTexCoord { get; private set; }

		public int OffsetBytesPosition2D { get { return (OffsetPosition2D * sizeof(float)); } }
		public int OffsetBytesPosition3D { get { return (OffsetPosition3D * sizeof(float)); } }
		public int OffsetBytesNormal { get { return (OffsetNormal * sizeof(float)); } }
		public int OffsetBytesColor { get { return (OffsetColor * sizeof(float)); } }
		public int OffsetBytesTexCoord { get { return (OffsetTexCoord * sizeof(float)); } }

		public VertexBuffer(VertexAttributes[] attributes, int numVertices, BufferObjectUsage usage)
			: base(BufferObjectType.Vertex, usage)
		{
			Initialize(attributes, numVertices);
		}

		public VertexBuffer(GraphicsDevice graphicsDevice, VertexAttributes[] attributes, int numVertices, BufferObjectUsage usage)
			: base(graphicsDevice, BufferObjectType.Vertex, usage)
		{
			Initialize(attributes, numVertices);
			CreateOnGpu();
		}

		public VertexBuffer(VertexBuffer source)
			: base(BufferObjectType.Vertex, source.Usage)
		{
			Initialize(source);
		}

		public VertexBuffer(GraphicsDevice graphicsDevice, VertexBuffer source)
			: base(graphicsDevice, BufferObjectType.Vertex, source.Usage)
		{
			Initialize(source);
			CreateOnGpu();
		}

		private void Initialize(VertexAttributes[] attributes, int numVertices)
		{
			if (attributes == null || attributes.Length == 0)
				throw new ArgumentException("attributes");
			if (numVertices <= 0)
				throw new ArgumentOutOfRangeException("numVertices");

			SetSizesAndOffsets(attributes);
			Resize(numVertices);
		}

		private void Initialize(VertexBuffer source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			var attributes = new VertexAttributes[source.NumAttributes];
			for (int i = 0; i < attributes.Length; ++i)
				attributes[i] = source.GetAttributeType(i);

			SetSizesAndOffsets(attributes);
			Resize(source.NumElements);
			Copy(source, 0);
		}

		public bool HasStandardAttribute(VertexStandardAttributes attribute)
		{
			return (_standardVertexAttribs & (int)attribute) > 0;
		}

		public int GetIndexOfStandardAttribute(VertexStandardAttributes attribute)
		{
			for (int i = 0; i < _attributes.Length; ++i)
			{
				if ((int)_attributes[i].type == (int)attribute)
					return i;
			}

			return -1;
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

		public int GetVertexStartIndex(int vertexIndex)
		{
			return (vertexIndex * ElementWidth);
		}

		public int GetVertexAttributeStartIndex(int attributeIndex, int vertexIndex)
		{
			return (GetVertexStartIndex(vertexIndex) + _attributes[attributeIndex].offset);
		}

		private void SetSizesAndOffsets(VertexAttributes[] attributes)
		{
			const int FloatsPerGpuAttribSlot = 4;
			const int MaxGpuAttribSlots = 8;

			if (attributes == null || attributes.Length == 0)
				throw new ArgumentException("attributes");
			if (_standardVertexAttribs != 0)
				throw new InvalidOperationException();
			if (_attributes != null)
				throw new InvalidOperationException();

			_standardVertexAttribs = 0;

			int offset = 0;
			int numGpuAttribSlotsUsed = 0;

			_attributes = new AttributeInfo[attributes.Length];

			for (int i = 0; i < attributes.Length; ++i)
			{
				var attrib = attributes[i];
				byte size = (byte)attrib;
				byte standardTypeBitMask = (byte)((ushort)attrib >> 8);

				// using integer division that rounds up (so given size = 13, result is 4, not 3)
				int numGpuSlots = ((int)size + (FloatsPerGpuAttribSlot - 1)) / FloatsPerGpuAttribSlot;
				if ((numGpuAttribSlotsUsed + numGpuSlots) > MaxGpuAttribSlots)
					throw new InvalidOperationException("Exceeded maximum GPU attribute space.");

				if (standardTypeBitMask > 0)
				{
					if ((_standardVertexAttribs & standardTypeBitMask) > 0)
						throw new InvalidOperationException("Duplicate standard attribute type specified.");

					_standardVertexAttribs |= standardTypeBitMask;

					switch ((VertexStandardAttributes)attrib)
					{
						case VertexStandardAttributes.Position2D: OffsetPosition2D = offset; break;
						case VertexStandardAttributes.Position3D: OffsetPosition3D = offset; break;
						case VertexStandardAttributes.Normal:     OffsetNormal = offset; break;
						case VertexStandardAttributes.Color:      OffsetColor = offset; break;
						case VertexStandardAttributes.TexCoord:   OffsetTexCoord = offset; break;
					}
				}

				_attributes[i].offset = offset;
				_attributes[i].size = size;
				_attributes[i].type = attrib;

				ElementWidth += size;
				offset += size;
				numGpuAttribSlotsUsed += numGpuSlots;
			}
		}

		public void Resize(int numVertices)
		{
			if (numVertices <= 0)
				throw new ArgumentOutOfRangeException("numIndices");

			int newSizeInFloats = numVertices * ElementWidth;

			if (_buffer == null)
				_buffer = new float[newSizeInFloats];
			else
				Array.Resize(ref _buffer, newSizeInFloats);

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

		public void Copy(VertexBuffer source, int destVertexIndex)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			Copy(source, destVertexIndex, 0, source.NumElements);
		}

		public void Copy(VertexBuffer source, int destVertexIndex, int sourceVertexIndex, int count)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (destVertexIndex <= 0 || destVertexIndex >= NumElements)
				throw new ArgumentOutOfRangeException("destVertexIndex");
			if (sourceVertexIndex <= 0 || sourceVertexIndex >= source.NumElements)
				throw new ArgumentOutOfRangeException("sourceVertexStart");
			if ((destVertexIndex + count) >= NumElements)
				throw new ArgumentOutOfRangeException("count");
			if ((sourceVertexIndex + count) >= source.NumElements)
				throw new ArgumentOutOfRangeException("count");
			if (NumAttributes != source.NumAttributes)
				throw new InvalidOperationException("Cannot copy different vertex data types.");

			for (int i = 0; i < NumAttributes; ++i)
			{
				if (GetAttributeType(i) != source.GetAttributeType(i))
					throw new InvalidOperationException("Cannot copy different vertex data types.");
			}

			int destIndex = GetVertexStartIndex(destVertexIndex);
			int sourceIndex = source.GetVertexStartIndex(sourceVertexIndex);
			int copyLength = count * ElementWidth;
			Array.Copy(source.Data, sourceIndex, _buffer, destIndex, copyLength);

			IsDirty = true;
		}

		#region Attribute Getters and Setters

		#region Getters

		public Color GetColor(int index)
		{
			Color result;
			GetColor(index, out result);
			return result;
		}

		public void GetColor(int index, out Color result)
		{
			int p = GetVertexStartIndex(index) + OffsetColor;
			result.R = _buffer[p];
			result.G = _buffer[p + 1];
			result.B = _buffer[p + 2];
			result.A = _buffer[p + 3];
		}

		public Vector3 GetPosition3D(int index)
		{
			Vector3 result;
			GetPosition3D(index, out result);
			return result;
		}

		public void GetPosition3D(int index, out Vector3 result)
		{
			int p = GetVertexStartIndex(index) + OffsetPosition3D;
			result.X = _buffer[p];
			result.Y = _buffer[p + 1];
			result.Z = _buffer[p + 2];
		}

		public Vector2 GetPosition2D(int index)
		{
			Vector2 result;
			GetPosition2D(index, out result);
			return result;
		}

		public void GetPosition2D(int index, out Vector2 result)
		{
			int p = GetVertexStartIndex(index) + OffsetPosition2D;
			result.X = _buffer[p];
			result.Y = _buffer[p + 1];
		}

		public Vector3 GetNormal(int index)
		{
			Vector3 result;
			GetNormal(index, out result);
			return result;
		}

		public void GetNormal(int index, out Vector3 result)
		{
			int p = GetVertexStartIndex(index) + OffsetNormal;
			result.X = _buffer[p];
			result.Y = _buffer[p + 1];
			result.Z = _buffer[p + 2];
		}

		public Vector2 GetTexCoord(int index)
		{
			Vector2 result;
			GetTexCoord(index, out result);
			return result;
		}

		public void GetTexCoord(int index, out Vector2 result)
		{
			int p = GetVertexStartIndex(index) + OffsetTexCoord;
			result.X = _buffer[p];
			result.Y = _buffer[p + 1];
		}

		public float Get1f(int attrib, int index)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			return _buffer[p];
		}

		public void Get1f(int attrib, int index, out float x)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			x = _buffer[p];
		}

		public void Get2f(int attrib, int index, out float x, out float y)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			x = _buffer[p];
			y = _buffer[p + 1];
		}

		public Vector2 Get2f(int attrib, int index)
		{
			Vector2 result;
			Get2f(attrib, index, out result);
			return result;
		}

		public void Get2f(int attrib, int index, out Vector2 v)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			v.X = _buffer[p];
			v.Y = _buffer[p + 1];
		}

		public void Get3f(int attrib, int index, out float x, out float y, out float z)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			x = _buffer[p];
			y = _buffer[p + 1];
			z = _buffer[p + 2];
		}

		public Vector3 Get3f(int attrib, int index)
		{
			Vector3 result;
			Get3f(attrib, index, out result);
			return result;
		}

		public void Get3f(int attrib, int index, out Vector3 v)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			v.X = _buffer[p];
			v.Y = _buffer[p + 1];
			v.Z = _buffer[p + 2];
		}

		public void Get4f(int attrib, int index, out float x, out float y, out float z, out float w)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			x = _buffer[p];
			y = _buffer[p + 1];
			z = _buffer[p + 2];
			w = _buffer[p + 3];
		}

		public Vector4 Get4f(int attrib, int index)
		{
			Vector4 result;
			Get4f(attrib, index, out result);
			return result;
		}

		public void Get4f(int attrib, int index, out Vector4 v)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			v.X = _buffer[p];
			v.Y = _buffer[p + 1];
			v.Z = _buffer[p + 2];
			v.W = _buffer[p + 3];
		}

		public void Get4f(int attrib, int index, out Quaternion q)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			q.X = _buffer[p];
			q.Y = _buffer[p + 1];
			q.Z = _buffer[p + 2];
			q.W = _buffer[p + 3];
		}

		public void Get4f(int attrib, int index, out Color c)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			c.R = _buffer[p];
			c.G = _buffer[p + 1];
			c.B = _buffer[p + 2];
			c.A = _buffer[p + 3];
		}

		public void Get9f(int attrib, int index, float[] x)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			Array.Copy(_buffer, p, x, 0, 9);
		}

		public Matrix3x3 Get9f(int attrib, int index)
		{
			Matrix3x3 result = new Matrix3x3();
			Get9f(attrib, index, ref result);
			return result;
		}

		public void Get9f(int attrib, int index, ref Matrix3x3 m)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			m.M11 = _buffer[p];
			m.M21 = _buffer[p + 1];
			m.M31 = _buffer[p + 2];
			m.M12 = _buffer[p + 3];
			m.M22 = _buffer[p + 4];
			m.M32 = _buffer[p + 5];
			m.M13 = _buffer[p + 6];
			m.M23 = _buffer[p + 7];
			m.M33 = _buffer[p + 8];
		}

		public void Get16f(int attrib, int index, float[] x)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			Array.Copy(_buffer, p, x, 0, 16);
		}

		public Matrix4x4 Get16f(int attrib, int index)
		{
			Matrix4x4 result = new Matrix4x4();
			Get16f(attrib, index, ref result);
			return result;
		}

		public void Get16f(int attrib, int index, ref Matrix4x4 m)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			m.M11 = _buffer[p];
			m.M21 = _buffer[p + 1];
			m.M31 = _buffer[p + 2];
			m.M41 = _buffer[p + 3];
			m.M12 = _buffer[p + 4];
			m.M22 = _buffer[p + 5];
			m.M32 = _buffer[p + 6];
			m.M42 = _buffer[p + 7];
			m.M13 = _buffer[p + 8];
			m.M23 = _buffer[p + 9];
			m.M33 = _buffer[p + 10];
			m.M43 = _buffer[p + 11];
			m.M14 = _buffer[p + 12];
			m.M24 = _buffer[p + 13];
			m.M34 = _buffer[p + 14];
			m.M44 = _buffer[p + 15];
		}

		public Color GetCurrentColor()
		{
			Color result;
			GetColor(CurrentPosition, out result);
			return result;
		}

		public void GetCurrentColor(out Color result)
		{
			GetColor(CurrentPosition, out result);
		}

		public Vector3 GetCurrentPosition3D()
		{
			Vector3 result;
			GetPosition3D(CurrentPosition, out result);
			return result;
		}

		public void GetCurrentPosition3D(out Vector3 result)
		{
			GetPosition3D(CurrentPosition, out result);
		}

		public Vector2 GetCurrentPosition2D()
		{
			Vector2 result;
			GetPosition2D(CurrentPosition, out result);
			return result;
		}

		public void GetCurrentPosition2D(out Vector2 result)
		{
			GetPosition2D(CurrentPosition, out result);
		}

		public Vector3 GetCurrentNormal()
		{
			Vector3 result;
			GetNormal(CurrentPosition, out result);
			return result;
		}

		public void GetCurrentNormal(out Vector3 result)
		{
			GetNormal(CurrentPosition, out result);
		}

		public Vector2 GetCurrentTexCoord()
		{
			Vector2 result;
			GetTexCoord(CurrentPosition, out result);
			return result;
		}

		public void GetCurrentTexCoord(out Vector2 result)
		{
			GetTexCoord(CurrentPosition, out result);
		}

		public float GetCurrent1f(int attrib)
		{
			return Get1f(attrib, CurrentPosition);
		}

		public void GetCurrent1f(int attrib, out float x)
		{
			Get1f(attrib, CurrentPosition, out x);
		}

		public void GetCurrent2f(int attrib, out float x, out float y)
		{
			Get2f(attrib, CurrentPosition, out x, out y);
		}

		public Vector2 GetCurrent2f(int attrib)
		{
			Vector2 result;
			Get2f(attrib, CurrentPosition, out result);
			return result;
		}

		public void GetCurrent2f(int attrib, out Vector2 v)
		{
			Get2f(attrib, CurrentPosition, out v);
		}

		public void GetCurrent3f(int attrib, out float x, out float y, out float z)
		{
			Get3f(attrib, CurrentPosition, out x, out y, out z);
		}

		public Vector3 GetCurrent3f(int attrib)
		{
			Vector3 result;
			Get3f(attrib, CurrentPosition, out result);
			return result;
		}

		public void GetCurrent3f(int attrib, out Vector3 v)
		{
			Get3f(attrib, CurrentPosition, out v);
		}

		public void GetCurrent4f(int attrib, out float x, out float y, out float z, out float w)
		{
			Get4f(attrib, CurrentPosition, out x, out y, out z, out w);
		}

		public Vector4 GetCurrent4f(int attrib)
		{
			Vector4 result;
			Get4f(attrib, CurrentPosition, out result);
			return result;
		}

		public void GetCurrent4f(int attrib, out Vector4 v)
		{
			Get4f(attrib, CurrentPosition, out v);
		}

		public void GetCurrent4f(int attrib, out Quaternion q)
		{
			Get4f(attrib, CurrentPosition, out q);
		}

		public void GetCurrent4f(int attrib, out Color c)
		{
			Get4f(attrib, CurrentPosition, out c);
		}

		public Matrix3x3 GetCurrent9f(int attrib)
		{
			Matrix3x3 result = new Matrix3x3();
			Get9f(attrib, CurrentPosition, ref result);
			return result;
		}

		public void GetCurrent9f(int attrib, float[] x)
		{
			Get9f(attrib, CurrentPosition, x);
		}

		public void GetCurrent9f(int attrib, ref Matrix3x3 m)
		{
			Get9f(attrib, CurrentPosition, ref m);
		}

		public void GetCurrent16f(int attrib, float[] x)
		{
			Get16f(attrib, CurrentPosition, x);
		}

		public Matrix4x4 GetCurrent16f(int attrib)
		{
			Matrix4x4 result = new Matrix4x4();
			Get16f(attrib, CurrentPosition, ref result);
			return result;
		}

		public void GetCurrent16f(int attrib, ref Matrix4x4 m)
		{
			Get16f(attrib, CurrentPosition, ref m);
		}

		#endregion

		#region Setters

		public void SetColor(int index, float r, float g, float b, float a)
		{
			int p = GetVertexStartIndex(index) + OffsetColor;
			_buffer[p] = r;
			_buffer[p + 1] = g;
			_buffer[p + 2] = b;
			_buffer[p + 3] = a;
			IsDirty = true;
		}

		public void SetColor(int index, float r, float g, float b)
		{
			int p = GetVertexStartIndex(index) + OffsetColor;
			_buffer[p] = r;
			_buffer[p + 1] = g;
			_buffer[p + 2] = b;
			_buffer[p + 3] = Color.AlphaOpaque;
			IsDirty = true;
		}

		public void SetColor(int index, Color c)
		{
			SetColor(index, ref c);
		}

		public void SetColor(int index, ref Color c)
		{
			int p = GetVertexStartIndex(index) + OffsetColor;
			_buffer[p] = c.R;
			_buffer[p + 1] = c.G;
			_buffer[p + 2] = c.B;
			_buffer[p + 3] = c.A;
			IsDirty = true;
		}

		public void SetPosition3D(int index, float x, float y, float z)
		{
			int p = GetVertexStartIndex(index) + OffsetPosition3D;
			_buffer[p] = x;
			_buffer[p + 1] = y;
			_buffer[p + 2] = z;
			IsDirty = true;
		}

		public void SetPosition3D(int index, Vector3 v)
		{
			SetPosition3D(index, ref v);
		}

		public void SetPosition3D(int index, ref Vector3 v)
		{
			int p = GetVertexStartIndex(index) + OffsetPosition3D;
			_buffer[p] = v.X;
			_buffer[p + 1] = v.Y;
			_buffer[p + 2] = v.Z;
			IsDirty = true;
		}

		public void SetPosition2D(int index, float x, float y)
		{
			int p = GetVertexStartIndex(index) + OffsetPosition2D;
			_buffer[p] = x;
			_buffer[p + 1] = y;
			IsDirty = true;
		}

		public void SetPosition2D(int index, Vector2 v)
		{
			SetPosition2D(index, ref v);
		}

		public void SetPosition2D(int index, ref Vector2 v)
		{
			int p = GetVertexStartIndex(index) + OffsetPosition2D;
			_buffer[p] = v.X;
			_buffer[p + 1] = v.Y;
			IsDirty = true;
		}

		public void SetNormal(int index, float x, float y, float z)
		{
			int p = GetVertexStartIndex(index) + OffsetNormal;
			_buffer[p] = x;
			_buffer[p + 1] = y;
			_buffer[p + 2] = z;
			IsDirty = true;
		}

		public void SetNormal(int index, Vector3 v)
		{
			SetNormal(index, ref v);
		}

		public void SetNormal(int index, ref Vector3 v)
		{
			int p = GetVertexStartIndex(index) + OffsetNormal;
			_buffer[p] = v.X;
			_buffer[p + 1] = v.Y;
			_buffer[p + 2] = v.Z;
			IsDirty = true;
		}

		public void SetTexCoord(int index, float u, float v)
		{
			int p = GetVertexStartIndex(index) + OffsetTexCoord;
			_buffer[p] = u;
			_buffer[p + 1] = v;
			IsDirty = true;
		}

		public void SetTexCoord(int index, Vector2 v)
		{
			SetTexCoord(index, ref v);
		}

		public void SetTexCoord(int index, ref Vector2 v)
		{
			int p = GetVertexStartIndex(index) + OffsetTexCoord;
			_buffer[p] = v.X;
			_buffer[p + 1] = v.Y;
			IsDirty = true;
		}

		public void Set1f(int attrib, int index, float x)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = x;
			IsDirty = true;
		}

		public void Set2f(int attrib, int index, float x, float y)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = x;
			_buffer[p + 1] = y;
			IsDirty = true;
		}

		public void Set2f(int attrib, int index, Vector2 v)
		{
			Set2f(attrib, index, ref v);
		}

		public void Set2f(int attrib, int index, ref Vector2 v)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = v.X;
			_buffer[p + 1] = v.Y;
			IsDirty = true;
		}

		public void Set3f(int attrib, int index, float x, float y, float z)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = x;
			_buffer[p + 1] = y;
			_buffer[p + 2] = z;
			IsDirty = true;
		}

		public void Set3f(int attrib, int index, Vector3 v)
		{
			Set3f(attrib, index, ref v);
		}

		public void Set3f(int attrib, int index, ref Vector3 v)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = v.X;
			_buffer[p + 1] = v.Y;
			_buffer[p + 2] = v.Z;
			IsDirty = true;
		}

		public void Set4f(int attrib, int index, float x, float y, float z, float w)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = x;
			_buffer[p + 1] = y;
			_buffer[p + 2] = z;
			_buffer[p + 3] = z;
			IsDirty = true;
		}

		public void Set4f(int attrib, int index, Vector4 v)
		{
			Set4f(attrib, index, ref v);
		}

		public void Set4f(int attrib, int index, ref Vector4 v)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = v.X;
			_buffer[p + 1] = v.Y;
			_buffer[p + 2] = v.Z;
			_buffer[p + 3] = v.W;
			IsDirty = true;
		}

		public void Set4f(int attrib, int index, Quaternion q)
		{
			Set4f(attrib, index, ref q);
		}

		public void Set4f(int attrib, int index, ref Quaternion q)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = q.X;
			_buffer[p + 1] = q.Y;
			_buffer[p + 2] = q.Z;
			_buffer[p + 3] = q.W;
			IsDirty = true;
		}

		public void Set4f(int attrib, int index, Color c)
		{
			Set4f(attrib, index, ref c);
		}

		public void Set4f(int attrib, int index, ref Color c)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = c.R;
			_buffer[p + 1] = c.G;
			_buffer[p + 2] = c.B;
			_buffer[p + 3] = c.A;
			IsDirty = true;
		}

		public void Set9f(int attrib, int index, float[] x)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			Array.Copy(x, 0, _buffer, p, 9);
		}

		public void Set9f(int attrib, int index, Matrix3x3 m)
		{
			Set9f(attrib, index, ref m);
		}

		public void Set9f(int attrib, int index, ref Matrix3x3 m)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = m.M11;
			_buffer[p + 1] = m.M21;
			_buffer[p + 2] = m.M31;
			_buffer[p + 3] = m.M12;
			_buffer[p + 4] = m.M22;
			_buffer[p + 5] = m.M32;
			_buffer[p + 6] = m.M13;
			_buffer[p + 7] = m.M23;
			_buffer[p + 8] = m.M33;
		}

		public void Set16f(int attrib, int index, float[] x)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			Array.Copy(x, 0, _buffer, p, 16);
		}

		public void Set16f(int attrib, int index, Matrix4x4 m)
		{
			Set16f(attrib, index, ref m);
		}

		public void Set16f(int attrib, int index, ref Matrix4x4 m)
		{
			int p = GetVertexAttributeStartIndex(attrib, index);
			_buffer[p] = m.M11;
			_buffer[p + 1] = m.M21;
			_buffer[p + 2] = m.M31;
			_buffer[p + 3] = m.M41;
			_buffer[p + 4] = m.M12;
			_buffer[p + 5] = m.M22;
			_buffer[p + 6] = m.M32;
			_buffer[p + 7] = m.M42;
			_buffer[p + 8] = m.M13;
			_buffer[p + 9] = m.M23;
			_buffer[p + 10] = m.M33;
			_buffer[p + 11] = m.M43;
			_buffer[p + 12] = m.M14;
			_buffer[p + 13] = m.M24;
			_buffer[p + 14] = m.M34;
			_buffer[p + 15] = m.M44;
		}

		public void SetCurrentColor(float r, float g, float b, float a)
		{
			SetColor(CurrentPosition, r, g, b, a);
		}

		public void SetCurrentColor(float r, float g, float b)
		{
			SetColor(CurrentPosition, r, g, b);
		}

		public void SetCurrentColor(Color c)
		{
			SetColor(CurrentPosition, ref c);
		}

		public void SetCurrentColor(ref Color c)
		{
			SetColor(CurrentPosition, ref c);
		}

		public void SetCurrentPosition3D(float x, float y, float z)
		{
			SetPosition3D(CurrentPosition, x, y, z);
		}

		public void SetCurrentPosition3D(Vector3 v)
		{
			SetPosition3D(CurrentPosition, ref v);
		}

		public void SetCurrentPosition3D(ref Vector3 v)
		{
			SetPosition3D(CurrentPosition, ref v);
		}

		public void SetCurrentPosition2D(float x, float y)
		{
			SetPosition2D(CurrentPosition, x, y);
		}

		public void SetCurrentPosition2D(Vector2 v)
		{
			SetPosition2D(CurrentPosition, ref v);
		}

		public void SetCurrentPosition2D(ref Vector2 v)
		{
			SetPosition2D(CurrentPosition, ref v);
		}

		public void SetCurrentNormal(float x, float y, float z)
		{
			SetNormal(CurrentPosition, x, y, z);
		}

		public void SetCurrentNormal(Vector3 n)
		{
			SetNormal(CurrentPosition, ref n);
		}

		public void SetCurrentNormal(ref Vector3 n)
		{
			SetNormal(CurrentPosition, ref n);
		}

		public void SetCurrentTexCoord(float u, float v)
		{
			SetTexCoord(CurrentPosition, u, v);
		}

		public void SetCurrentTexCoord(Vector2 t)
		{
			SetTexCoord(CurrentPosition, ref t);
		}

		public void SetCurrentTexCoord(ref Vector2 t)
		{
			SetTexCoord(CurrentPosition, ref t);
		}

		public void SetCurrent1f(int attrib, float x)
		{
			Set1f(attrib, CurrentPosition, x);
		}

		public void SetCurrent2f(int attrib, float x, float y)
		{
			Set2f(attrib, CurrentPosition, x, y);
		}

		public void SetCurrent2f(int attrib, Vector2 v)
		{
			Set2f(attrib, CurrentPosition, ref v);
		}

		public void SetCurrent2f(int attrib, ref Vector2 v)
		{
			Set2f(attrib, CurrentPosition, ref v);
		}

		public void SetCurrent3f(int attrib, float x, float y, float z)
		{
			Set3f(attrib, CurrentPosition, x, y, z);
		}

		public void SetCurrent3f(int attrib, Vector3 v)
		{
			Set3f(attrib, CurrentPosition, ref v);
		}

		public void SetCurrent3f(int attrib, ref Vector3 v)
		{
			Set3f(attrib, CurrentPosition, ref v);
		}

		public void SetCurrent4f(int attrib, float x, float y, float z, float w)
		{
			Set4f(attrib, CurrentPosition, x, y, z, w);
		}

		public void SetCurrent4f(int attrib, Vector4 v)
		{
			Set4f(attrib, CurrentPosition, ref v);
		}

		public void SetCurrent4f(int attrib, ref Vector4 v)
		{
			Set4f(attrib, CurrentPosition, ref v);
		}

		public void SetCurrent4f(int attrib, Quaternion q)
		{
			Set4f(attrib, CurrentPosition, ref q);
		}

		public void SetCurrent4f(int attrib, ref Quaternion q)
		{
			Set4f(attrib, CurrentPosition, ref q);
		}

		public void SetCurrent4f(int attrib, Color c)
		{
			Set4f(attrib, CurrentPosition, ref c);
		}

		public void SetCurrent4f(int attrib, ref Color c)
		{
			Set4f(attrib, CurrentPosition, ref c);
		}

		public void SetCurrent9f(int attrib, float[] x)
		{
			Set9f(attrib, CurrentPosition, x);
		}

		public void SetCurrent9f(int attrib, Matrix3x3 m)
		{
			Set9f(attrib, CurrentPosition, ref m);
		}

		public void SetCurrent9f(int attrib, ref Matrix3x3 m)
		{
			Set9f(attrib, CurrentPosition, ref m);
		}

		public void SetCurrent16f(int attrib, float[] x)
		{
			Set16f(attrib, CurrentPosition, x);
		}

		public void SetCurrent16f(int attrib, Matrix4x4 m)
		{
			Set16f(attrib, CurrentPosition, ref m);
		}

		public void SetCurrent16f(int attrib, ref Matrix4x4 m)
		{
			Set16f(attrib, CurrentPosition, ref m);
		}

		#endregion

		#endregion
	}
}
