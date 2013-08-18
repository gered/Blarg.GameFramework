using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PortableGL;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.Graphics
{
	#region Support Classes / Structures

	public class ShaderUniform
	{
		public string Name;
		public int Location;
		public int Type;
		public int Size;
	}

	public class ShaderAttribute
	{
		public string Name;
		public int Location;
		public int Type;
		public int Size;
		public bool IsTypeBound;
	}

	public class ShaderAttributeMapInfo
	{
		public bool UsesStandardType;
		public VertexStandardAttributes StandardType;
		public int AttributeIndex;
	}

	public enum ShaderCachedUniformType
	{
		Float1,
		Int1,
		Float2,
		Int2,
		Float3,
		Int3,
		Float4,
		Int4,
		Float9,
		Float16
	}

	public class ShaderCachedUniform
	{
		public string Name;
		public bool HasNewValue;
		public ShaderCachedUniformType Type;

		public float[] Floats = new float[16];
		public int[] Ints = new int[4];
	}

	#endregion

	public class Shader : GraphicsContextResource
	{
		// NOTE: using simple arrays instead of Dictionary, or some other key/value storage
		//       to avoid having to use IEnumerables + foreach-loops to walk through
		//       these lists (which would allocate memory).
		//       These lists will contain less then 10 items 99% of the time anyway
		//       so name lookups via a for-loop won't be too bad... I think
		//       Might be worthwhile to populate them via the uniform/attribute
		//       location value (same ordering) assuming the location value isn't
		//       some weird large number sometimes... that if it's zero-based in all cases
		private ShaderUniform[] _uniforms;
		private ShaderAttribute[] _attributes;
		private ShaderAttributeMapInfo[] _attributeMapping;
		private ShaderCachedUniform[] _cachedUniforms;

		private string _inlineVertexShaderSource;
		private string _inlineFragmentShaderSource;

		private string _cachedVertexShaderSource;
		private string _cachedFragmentShaderSource;

		public int ProgramID { get; private set; }
		public int VertexShaderID { get; private set; }
		public int FragmentShaderID { get; private set; }

		public bool IsLinked { get; private set; }
		public bool IsVertexShaderCompiled { get; private set; }
		public bool IsFragmentShaderCompiled { get; private set; }

		public bool IsBound { get; private set; }

		public int NumUniforms { get; private set; }
		public int NumAttributes { get; private set; }

		public bool IsReadyForUse
		{
			get { return (IsLinked && IsVertexShaderCompiled && IsFragmentShaderCompiled); }
		}

		public Shader(GraphicsDevice graphicsDevice, string vertexShaderSource, string fragmentShaderSource)
			: base(graphicsDevice)
		{
			if (String.IsNullOrEmpty(vertexShaderSource))
				throw new ArgumentNullException("vertexShaderSource");
			if (String.IsNullOrEmpty(fragmentShaderSource))
				throw new ArgumentNullException("fragmentShaderSource");

			Initialize();
			LoadCompileAndLink(vertexShaderSource, fragmentShaderSource);
			CacheShaderSources(vertexShaderSource, fragmentShaderSource);
		}

		public Shader(GraphicsDevice graphicsDevice, TextReader vertexShaderSourceReader, TextReader fragmentShaderSourceReader)
			: base(graphicsDevice)
		{
			if (vertexShaderSourceReader == null)
				throw new ArgumentNullException("vertexShaderSourceReader");
			if (fragmentShaderSourceReader == null)
				throw new ArgumentNullException("fragmentShaderSourceReader");

			string vertexShaderSource = vertexShaderSourceReader.ReadToEnd();
			string fragmentShaderSource = fragmentShaderSourceReader.ReadToEnd();

			Initialize();
			LoadCompileAndLink(vertexShaderSource, fragmentShaderSource);
			CacheShaderSources(vertexShaderSource, fragmentShaderSource);
		}

		protected Shader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			Initialize();
		}

		private void Initialize()
		{
			IsBound = false;
			ProgramID = -1;
			VertexShaderID = -1;
			FragmentShaderID = -1;
			IsLinked = false;
			IsVertexShaderCompiled = false;
			IsFragmentShaderCompiled = false;
			_uniforms = null;
			_attributes = null;
			_cachedUniforms = null;
		}

		#region Compile and Linking

		private void Compile(string vertexShaderSource, string fragmentShaderSource)
		{
			if (String.IsNullOrEmpty(vertexShaderSource))
				throw new ArgumentException("vertexShaderSource");
			if (String.IsNullOrEmpty(fragmentShaderSource))
				throw new ArgumentException("fragmentShaderSource");
			if (ProgramID != -1 || VertexShaderID != -1 || FragmentShaderID != -1)
				throw new InvalidOperationException();

			// first load and compile the vertex shader

			int vertexShaderId = Platform.GL.glCreateShader(GL20.GL_VERTEX_SHADER);
			if (vertexShaderId == 0)
				throw new Exception("Failed to create OpenGL Vertex Shader object.");

			// add in a special #define for convenience when we want to put both
			// vertex and fragment shaders in the same source file
			string[] vertexSources = { "#define VERTEX\n", vertexShaderSource };
			int[] vertexSourcesLength = { vertexSources[0].Length, vertexSources[1].Length };

			Platform.GL.glShaderSource(vertexShaderId, 2, vertexSources, vertexSourcesLength);
			Platform.GL.glCompileShader(vertexShaderId);

			int vertexShaderCompileStatus = Platform.GL.glGetShaderiv(vertexShaderId, GL20.GL_COMPILE_STATUS);

			// log compiler error
			if (vertexShaderCompileStatus == 0)
			{
				string log = GetShaderLog(vertexShaderId);
				Platform.Logger.Error("OpenGL", "Error compiling vertex shader:\n{0}", log);
			}

			// and now the fragment shader

			int fragmentShaderId = Platform.GL.glCreateShader(GL20.GL_FRAGMENT_SHADER);
			if (fragmentShaderId == 0)
				throw new Exception("Failed to create OpenGL Fragment Shader object.");

			// add in a special #define for convenience when we want to put both
			// vertex and fragment shaders in the same source file
			string[] fragmentSources = { "#define FRAGMENT\n", fragmentShaderSource };
			int[] fragmentSourcesLength = { fragmentSources[0].Length, fragmentSources[1].Length };

			Platform.GL.glShaderSource(fragmentShaderId, 2, fragmentSources, fragmentSourcesLength);
			Platform.GL.glCompileShader(fragmentShaderId);

			int fragmentShaderCompileStatus = Platform.GL.glGetShaderiv(fragmentShaderId, GL20.GL_COMPILE_STATUS);

			// log compiler error
			if (fragmentShaderCompileStatus == 0)
			{
				string log = GetShaderLog(fragmentShaderId);
				Platform.Logger.Error("OpenGL", "Error compiling fragment shader:\n{0}", log);
			}

			// only return success if both compiled successfully
			if (vertexShaderCompileStatus != 0 && fragmentShaderCompileStatus != 0)
			{
				VertexShaderID = vertexShaderId;
				IsVertexShaderCompiled = true;
				FragmentShaderID = fragmentShaderId;
				IsFragmentShaderCompiled = true;
			}
			else
				throw new Exception("Vertex and/or fragment shader failed to compile.");
		}

		private string GetShaderLog(int shaderId)
		{
			return Platform.GL.glGetShaderInfoLog(shaderId);
		}

		private void Link()
		{
			if (ProgramID != -1)
				throw new InvalidOperationException();
			if (VertexShaderID == -1 || FragmentShaderID == -1)
				throw new InvalidOperationException();

			int programId = Platform.GL.glCreateProgram();
			if (programId == 0)
				throw new Exception("Failed to create OpenGL Shader Program object.");

			Platform.GL.glAttachShader(programId, VertexShaderID);
			Platform.GL.glAttachShader(programId, FragmentShaderID);
			Platform.GL.glLinkProgram(programId);

			int programLinkStatus = Platform.GL.glGetProgramiv(programId, GL20.GL_LINK_STATUS);

			// log linker error
			if (programLinkStatus == 0)
			{
				string log = Platform.GL.glGetProgramInfoLog(programId);
				Platform.Logger.Error("OpenGL", "Error linking program:\n{0}", log);
			}

			if (programLinkStatus != 0)
			{
				ProgramID = programId;
				IsLinked = true;
			}
			else
				throw new Exception("Shader program failed to link.");
		}

		protected void LoadCompileAndLink(string vertexShaderSource, string fragmentShaderSource)
		{
			string vertexShaderToUse = vertexShaderSource;
			string fragmentShaderToUse = fragmentShaderSource;

			// if no source was provided, see if there is some cached source to load instead
			if (vertexShaderToUse == null)
				vertexShaderToUse = _cachedVertexShaderSource;
			if (fragmentShaderToUse == null)
				fragmentShaderToUse = _cachedFragmentShaderSource;

			Compile(vertexShaderToUse, fragmentShaderToUse);
			Link();

			LoadUniformInfo();
			LoadAttributeInfo();
		}

		protected void LoadCompileAndLinkInlineSources(string vertexShaderSource, string fragmentShaderSource)
		{
			if (String.IsNullOrEmpty(vertexShaderSource))
				throw new ArgumentNullException("vertexShaderSource");
			if (String.IsNullOrEmpty(fragmentShaderSource))
				throw new ArgumentNullException("fragmentShaderSource");

			LoadCompileAndLink(vertexShaderSource, fragmentShaderSource);
			_inlineVertexShaderSource = vertexShaderSource;
			_inlineFragmentShaderSource = fragmentShaderSource;
		}

		protected void ReloadCompileAndLink(string vertexShaderSource, string fragmentShaderSource)
		{
			// clear out all existing data that will be reset during the compile/link/init process
			IsBound = false;
			ProgramID = -1;
			VertexShaderID = -1;
			FragmentShaderID = -1;
			IsLinked = false;
			IsVertexShaderCompiled = false;
			IsFragmentShaderCompiled = false;
			_uniforms = null;
			_attributes = null;
			_cachedUniforms = null;

			// TODO: leaving the attribute type mappings intact. This could maybe be a problem?
			//       I think only if the attribute ID's can be assigned randomly by OpenGL even if
			//       the source remains the same each time would it ever be a problem to keep the
			//       old type mappings intact. Since OpenGL assigns the attribute index a zero-based
			//       number I have a feeling it is based on the declaration order in the shader
			//       source... so as long as the source doesn't change it should be the same

			LoadCompileAndLink(vertexShaderSource, fragmentShaderSource);
		}

		protected void CacheShaderSources(string vertexShaderSource, string fragmentShaderSource)
		{
			if (String.IsNullOrEmpty(vertexShaderSource))
				throw new ArgumentException("vertexShaderSource");
			if (String.IsNullOrEmpty(fragmentShaderSource))
				throw new ArgumentException("fragmentShaderSource");

			// ensure we have our own copy of the source strings...
			_cachedVertexShaderSource = StringExtensions.Copy(vertexShaderSource);
			_cachedFragmentShaderSource = StringExtensions.Copy(fragmentShaderSource);

			_inlineVertexShaderSource = null;
			_inlineFragmentShaderSource = null;
		}

		#endregion

		#region Uniform and Attribute info loading

		private void LoadUniformInfo()
		{
			if (ProgramID == -1)
				throw new InvalidOperationException();

			int numUniforms = Platform.GL.glGetProgramiv(ProgramID, GL20.GL_ACTIVE_UNIFORMS);
			NumUniforms = numUniforms;

			if (NumUniforms == 0)
				return;

			_uniforms = new ShaderUniform[NumUniforms];
			_cachedUniforms = new ShaderCachedUniform[NumUniforms];

			// collect information about each uniform
			for (int i = 0; i < NumUniforms; ++i)
			{
				var uniform = new ShaderUniform();

				int uniformType;
				uniform.Name = Platform.GL.glGetActiveUniform(ProgramID, i, out uniform.Size, out uniformType);
				uniform.Location = Platform.GL.glGetUniformLocation(ProgramID, uniform.Name);
				uniform.Type = (int)uniformType;

				// it seems Windows/Mac (possibly Linux too) have differing opinions on
				// including "[0]" in the uniform name for uniforms that are arrays
				// we'll just chop any "[0]" off if found in the uniform name before we 
				// add it to our list
				int arraySubscriptPos = uniform.Name.IndexOf("[0]");
				if (arraySubscriptPos != -1)
					uniform.Name = uniform.Name.Substring(0, arraySubscriptPos);

				_uniforms[i] = uniform;

				// add an empty cached uniform value object as well
				var cachedUniform = new ShaderCachedUniform();
				cachedUniform.Name = StringExtensions.Copy(uniform.Name);
				_cachedUniforms[i] = cachedUniform;
			}
		}

		private void LoadAttributeInfo()
		{
			if (ProgramID == -1)
				throw new InvalidOperationException();

			int numAttributes = Platform.GL.glGetProgramiv(ProgramID, GL20.GL_ACTIVE_ATTRIBUTES);

			// sanity checking, which only matters for shader reloading (e.g. when a context is lost)
			if (_attributeMapping != null)
				System.Diagnostics.Debug.Assert(numAttributes == NumAttributes);

			NumAttributes = numAttributes;

			if (NumAttributes == 0)
				return;

			_attributes = new ShaderAttribute[NumAttributes];

			// leave existing attribute type mappings (they will already be there if, e.g. the context is lost and reloaded)
			// (so, this array will only be allocated here on the first load)
			if (_attributeMapping == null)
			{
				_attributeMapping = new ShaderAttributeMapInfo[NumAttributes];

				// fill with a bunch of empty mapping objects to start out with
				for (int i = 0; i < _attributeMapping.Length; ++i)
					_attributeMapping[i] = new ShaderAttributeMapInfo();
			}

			// collect information about each attribute
			for (int i = 0; i < NumAttributes; ++i)
			{
				var attribute = new ShaderAttribute();

				int attributeType;
				attribute.Name = Platform.GL.glGetActiveAttrib(ProgramID, i, out attribute.Size, out attributeType);
				attribute.Location = Platform.GL.glGetAttribLocation(ProgramID, attribute.Name);
				attribute.Type = (int)attributeType;

				_attributes[i] = attribute;
			}
		}

		#endregion

		#region General Uniform and Attribute access

		public bool HasUniform(string name)
		{
			if (_uniforms == null)
				return false;

			for (int i = 0; i < _uniforms.Length; ++i)
			{
				if (_uniforms[i].Name == name)
					return true;
			}

			return false;
		}

		public bool HasAttribute(string name)
		{
			if (_attributes == null)
				return false;

			for (int i = 0; i < _attributes.Length; ++i)
			{
				if (_attributes[i].Name == name)
					return true;
			}

			return false;
		}

		protected ShaderUniform GetUniform(string name)
		{
			if (_uniforms == null)
				return null;

			for (int i = 0; i < _uniforms.Length; ++i)
			{
				if (_uniforms[i].Name == name)
					return _uniforms[i];
			}

			return null;
		}

		protected ShaderAttribute GetAttribute(string name)
		{
			if (_attributes == null)
				return null;

			for (int i = 0; i < _attributes.Length; ++i)
			{
				if (_attributes[i].Name == name)
					return _attributes[i];
			}

			return null;
		}

		#endregion

		#region Shader-To-VBO Attribute Mapping

		public bool IsMappedToVBOStandardAttribute(int attributeIndex)
		{
			return _attributeMapping[attributeIndex].UsesStandardType;
		}

		public int GetMappedVBOAttributeIndexFor(int attributeIndex)
		{
			return _attributeMapping[attributeIndex].AttributeIndex;
		}

		public VertexStandardAttributes GetMappedVBOStandardAttribute(int attributeIndex)
		{
			return _attributeMapping[attributeIndex].StandardType;
		}

		public void MapToVBOAttribute(string name, int vboAttributeIndex)
		{
			var shaderAttrib = GetAttribute(name);
			if (shaderAttrib == null)
				throw new InvalidOperationException("Attribute does not exist.");
			System.Diagnostics.Debug.Assert(shaderAttrib.Location < NumAttributes);

			var mappingInfo = _attributeMapping[shaderAttrib.Location];
			mappingInfo.UsesStandardType = false;
			mappingInfo.AttributeIndex = vboAttributeIndex;

			shaderAttrib.IsTypeBound = true;
		}

		public void MapToVBOStandardAttribute(string name, VertexStandardAttributes type)
		{
			var shaderAttrib = GetAttribute(name);
			if (shaderAttrib == null)
				throw new InvalidOperationException("Attribute does not exist.");
			System.Diagnostics.Debug.Assert(shaderAttrib.Location < NumAttributes);

			var mappingInfo = _attributeMapping[shaderAttrib.Location];
			mappingInfo.UsesStandardType = true;
			mappingInfo.StandardType = type;

			shaderAttrib.IsTypeBound = true;
		}

		#endregion

		#region Callbacks

		internal virtual void OnBind()
		{
			System.Diagnostics.Debug.Assert(IsBound == false);
			IsBound = true;
			FlushCachedUniforms();
		}

		internal virtual void OnUnbind()
		{
			System.Diagnostics.Debug.Assert(IsBound == true);
			IsBound = false;
			if (_cachedUniforms != null)
			{
				for (int i = 0; i < _cachedUniforms.Length; ++i)
					_cachedUniforms[i].HasNewValue = false;
			}
		}

		#endregion

		#region Cached Uniform value management

		private ShaderCachedUniform GetCachedUniform(string name)
		{
			if (_cachedUniforms == null)
				return null;

			for (int i = 0; i < _cachedUniforms.Length; ++i)
			{
				if (_cachedUniforms[i].Name == name)
					return _cachedUniforms[i];
			}

			return null;
		}

		private void FlushCachedUniforms()
		{
			if (!IsBound)
				throw new InvalidOperationException();
			if (_cachedUniforms == null || _cachedUniforms.Length == 0)
				return;

			for (int i = 0; i < _cachedUniforms.Length; ++i)
			{
				var uniform = _cachedUniforms[i];
				if (!uniform.HasNewValue)
					continue;

				switch (uniform.Type)
				{
					case ShaderCachedUniformType.Float1:    SetUniform(uniform.Name, uniform.Floats[0]); break;
					case ShaderCachedUniformType.Int1:      SetUniform(uniform.Name, uniform.Ints[0]); break;
					case ShaderCachedUniformType.Float2:    SetUniform(uniform.Name, uniform.Floats[0], uniform.Floats[1]); break;
					case ShaderCachedUniformType.Int2:      SetUniform(uniform.Name, uniform.Ints[0], uniform.Ints[1]); break;
					case ShaderCachedUniformType.Float3:    SetUniform(uniform.Name, uniform.Floats[0], uniform.Floats[1], uniform.Floats[2]); break;
					case ShaderCachedUniformType.Int3:      SetUniform(uniform.Name, uniform.Ints[0], uniform.Ints[1], uniform.Ints[2]); break;
					case ShaderCachedUniformType.Float4:    SetUniform(uniform.Name, uniform.Floats[0], uniform.Floats[1], uniform.Floats[2], uniform.Floats[3]); break;
					case ShaderCachedUniformType.Int4:      SetUniform(uniform.Name, uniform.Ints[0], uniform.Ints[1], uniform.Ints[2], uniform.Ints[3]); break;
					case ShaderCachedUniformType.Float9:
						Matrix3x3 m3x3 = new Matrix3x3(uniform.Floats);
						SetUniform(uniform.Name, ref m3x3);
						break;
					case ShaderCachedUniformType.Float16:
						Matrix4x4 m4x4 = new Matrix4x4(uniform.Floats);
						SetUniform(uniform.Name, ref m4x4);
						break;
				}

				uniform.HasNewValue = false;
			}
		}

		#endregion

		#region Uniform Setters

		public void SetUniform(string name, float x)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform1f(uniform.Location, x);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float1;
				cachedUniform.Floats[0] = x;
			}
		}

		public void SetUniform(string name, int x)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform1i(uniform.Location, x);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Int1;
				cachedUniform.Ints[0] = x;
			}
		}

		public void SetUniform(string name, float x, float y)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform2f(uniform.Location, x, y);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float2;
				cachedUniform.Floats[0] = x;
				cachedUniform.Floats[1] = y;
			}
		}

		public void SetUniform(string name, int x, int y)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform2i(uniform.Location, x, y);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Int2;
				cachedUniform.Ints[0] = x;
				cachedUniform.Ints[1] = y;
			}
		}

		public void SetUniform(string name, Vector2 v)
		{
			SetUniform(name, ref v);
		}

		public void SetUniform(string name, ref Vector2 v)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform2f(uniform.Location, v.X, v.Y);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float2;
				cachedUniform.Floats[0] = v.X;
				cachedUniform.Floats[1] = v.Y;
			}
		}

		public void SetUniform(string name, Point2 p)
		{
			SetUniform(name, ref p);
		}

		public void SetUniform(string name, ref Point2 p)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform2i(uniform.Location, p.X, p.Y);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Int2;
				cachedUniform.Ints[0] = p.X;
				cachedUniform.Ints[1] = p.Y;
			}
		}

		public void SetUniform(string name, float x, float y, float z)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform3f(uniform.Location, x, y, z);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float3;
				cachedUniform.Floats[0] = x;
				cachedUniform.Floats[1] = y;
				cachedUniform.Floats[2] = z;
			}
		}

		public void SetUniform(string name, int x, int y, int z)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform3i(uniform.Location, x, y, z);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Int3;
				cachedUniform.Ints[0] = x;
				cachedUniform.Ints[1] = y;
				cachedUniform.Ints[2] = z;
			}
		}

		public void SetUniform(string name, Vector3 v)
		{
			SetUniform(name, ref v);
		}

		public void SetUniform(string name, ref Vector3 v)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform3f(uniform.Location, v.X, v.Y, v.Z);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float3;
				cachedUniform.Floats[0] = v.X;
				cachedUniform.Floats[1] = v.Y;
				cachedUniform.Floats[2] = v.Z;
			}
		}

		public void SetUniform(string name, Point3 p)
		{
			SetUniform(name, ref p);
		}

		public void SetUniform(string name, ref Point3 p)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform3i(uniform.Location, p.X, p.Y, p.Z);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Int3;
				cachedUniform.Ints[0] = p.X;
				cachedUniform.Ints[1] = p.Y;
				cachedUniform.Ints[2] = p.Z;
			}
		}

		public void SetUniform(string name, float x, float y, float z, float w)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform4f(uniform.Location, x, y, z, w);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float4;
				cachedUniform.Floats[0] = x;
				cachedUniform.Floats[1] = y;
				cachedUniform.Floats[2] = z;
				cachedUniform.Floats[3] = w;
			}
		}

		public void SetUniform(string name, int x, int y, int z, int w)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform4i(uniform.Location, x, y, z, w);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Int4;
				cachedUniform.Ints[0] = x;
				cachedUniform.Ints[1] = y;
				cachedUniform.Ints[2] = z;
				cachedUniform.Ints[3] = w;
			}
		}

		public void SetUniform(string name, Vector4 v)
		{
			SetUniform(name, ref v);
		}

		public void SetUniform(string name, ref Vector4 v)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform4f(uniform.Location, v.X, v.Y, v.Z, v.W);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float4;
				cachedUniform.Floats[0] = v.X;
				cachedUniform.Floats[1] = v.Y;
				cachedUniform.Floats[2] = v.Z;
				cachedUniform.Floats[3] = v.W;
			}
		}

		public void SetUniform(string name, Quaternion q)
		{
			SetUniform(name, ref q);
		}

		public void SetUniform(string name, ref Quaternion q)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform4f(uniform.Location, q.X, q.Y, q.Z, q.W);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float4;
				cachedUniform.Floats[0] = q.X;
				cachedUniform.Floats[1] = q.Y;
				cachedUniform.Floats[2] = q.Z;
				cachedUniform.Floats[3] = q.W;
			}
		}

		public void SetUniform(string name, Color c)
		{
			SetUniform(name, ref c);
		}

		public void SetUniform(string name, ref Color c)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform4f(uniform.Location, c.R, c.G, c.B, c.A);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float4;
				cachedUniform.Floats[0] = c.R;
				cachedUniform.Floats[1] = c.G;
				cachedUniform.Floats[2] = c.B;
				cachedUniform.Floats[3] = c.A;
			}
		}

		public void SetUniform(string name, Matrix3x3 m)
		{
			SetUniform(name, ref m);
		}

		public void SetUniform(string name, ref Matrix3x3 m)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniformMatrix3fv(uniform.Location, 1, false, ref m.M11);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float9;
				m.ToArray(cachedUniform.Floats);
			}
		}

		public void SetUniform(string name, Matrix4x4 m)
		{
			SetUniform(name, ref m);
		}

		public void SetUniform(string name, ref Matrix4x4 m)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size != 1)
					throw new InvalidOperationException();
				Platform.GL.glUniformMatrix4fv(uniform.Location, 1, false, ref m.M11);
			}
			else
			{
				var cachedUniform = GetCachedUniform(name);
				if (cachedUniform == null)
					throw new ArgumentException("Invalid uniform.");
				cachedUniform.Type = ShaderCachedUniformType.Float16;
				m.ToArray(cachedUniform.Floats);
			}
		}

		public void SetUniform(string name, float[] x)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();
				Platform.GL.glUniform1fv(uniform.Location, x.Length, x);
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		public void SetUniform(string name, int[] x)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();

				unsafe
				{
					fixed (int *p = x)
					{
						Platform.GL.glUniform1fv(uniform.Location, x.Length, new IntPtr((long)p));
					}
				}
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		public void SetUniform(string name, Vector2[] v)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();

				Platform.GL.glUniform2fv(uniform.Location, v.Length, ref v[0].X);
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		public void SetUniform(string name, Vector3[] v)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();

				Platform.GL.glUniform3fv(uniform.Location, v.Length, ref v[0].X);
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		public void SetUniform(string name, Vector4[] v)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();

				Platform.GL.glUniform4fv(uniform.Location, v.Length, ref v[0].X);
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		public void SetUniform(string name, Quaternion[] q)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();

				Platform.GL.glUniform4fv(uniform.Location, q.Length, ref q[0].X);
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		public void SetUniform(string name, Color[] c)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();

				Platform.GL.glUniform4fv(uniform.Location, c.Length, ref c[0].R);
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		public void SetUniform(string name, Matrix3x3[] m)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();

				Platform.GL.glUniformMatrix3fv(uniform.Location, m.Length, false, ref m[0].M11);
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		public void SetUniform(string name, Matrix4x4[] m)
		{
			if (IsBound)
			{
				var uniform = GetUniform(name);
				if (uniform == null)
					throw new ArgumentException("Invalid uniform.");
				if (uniform.Size <= 1)
					throw new InvalidOperationException();

				Platform.GL.glUniformMatrix4fv(uniform.Location, m.Length, false, ref m[0].M11);
			}
			else
			{
				throw new NotImplementedException("Caching uniform value arrays not implemented yet.");
			}
		}

		#endregion

		#region GraphicsContextResource

		public override void OnNewContext()
		{
			if (!String.IsNullOrEmpty(_inlineVertexShaderSource) && !String.IsNullOrEmpty(_inlineFragmentShaderSource))
				ReloadCompileAndLink(_inlineVertexShaderSource, _inlineFragmentShaderSource);
			else
				ReloadCompileAndLink(null, null);
		}

		public override void OnLostContext()
		{
			ProgramID = -1;
			VertexShaderID = -1;
			FragmentShaderID = -1;
		}

		protected override bool ReleaseResource()
		{
			// this needs to happen before the OpenGL context is destroyed
			// which is not guaranteed if we rely 100% on the GC to clean 
			// everything up. best solution is to ensure all Shader
			// objects are not being referenced before the window is
			// closed and do a GC.Collect()

			if (VertexShaderID != -1)
			{
				Platform.GL.glDeleteShader(VertexShaderID);
				VertexShaderID = -1;
			}
			if (FragmentShaderID != -1)
			{
				Platform.GL.glDeleteShader(FragmentShaderID);
				FragmentShaderID = -1;
			}
			if (ProgramID != -1)
			{
				Platform.GL.glDeleteProgram(ProgramID);
				ProgramID = -1;
			}

			return true;
		}

		#endregion
	}
}
