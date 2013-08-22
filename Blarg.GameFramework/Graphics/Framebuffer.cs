using System;
using System.Collections.Generic;
using PortableGL;
using Blarg.GameFramework.Support;

namespace Blarg.GameFramework.Graphics
{
	public enum FramebufferTextureFormat
	{
		RGB = 1,
		RGBA,
		Depth
	}

	public enum FramebufferRenderbufferFormat
	{
		RGB = 1,
		RGBA,
		Depth,
		Stencil
	}

	public class Framebuffer : GraphicsContextResource
	{
		int _fixedWidth;
		int _fixedHeight;
		Dictionary<FramebufferTextureFormat, Texture> _attachedTextures;
		Dictionary<FramebufferRenderbufferFormat, Renderbuffer> _attachedRenderbuffers;
		ViewContext _attachedViewContext;

		public int ID { get; private set; }

		public bool IsInvalidated
		{
			get { return ID == -1; }
		}

		public bool IsUsingFixedDimensions
		{
			get { return (_fixedWidth != 0 && _fixedHeight != 0); }
		}

		public ViewContext AttachedViewContext
		{
			get { return _attachedViewContext; }
		}

		public Renderbuffer GetAttachedRenderbuffer(FramebufferRenderbufferFormat format)
		{
			return _attachedRenderbuffers.Get(format);
		}

		public Texture GetAttachedTexture(FramebufferTextureFormat format)
		{
			return _attachedTextures.Get(format);
		}

		public Framebuffer(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			ID = -1;
			_attachedRenderbuffers = new Dictionary<FramebufferRenderbufferFormat, Renderbuffer>();
			_attachedTextures = new Dictionary<FramebufferTextureFormat, Texture>();
			_attachedViewContext = null;
			Create(0, 0);
		}

		public Framebuffer(GraphicsDevice graphicsDevice, int width, int height)
			: base(graphicsDevice)
		{
			if (width < 1)
				throw new ArgumentOutOfRangeException("width");
			if (height < 1)
				throw new ArgumentOutOfRangeException("height");

			ID = -1;
			_attachedRenderbuffers = new Dictionary<FramebufferRenderbufferFormat, Renderbuffer>();
			_attachedTextures = new Dictionary<FramebufferTextureFormat, Texture>();
			_attachedViewContext = null;
			Create(width, height);
		}

		private void Create(int width, int height)
		{
			if (!IsInvalidated)
				throw new InvalidOperationException();

			ID = Platform.GL.glGenFramebuffers();

			_fixedWidth = width;
			_fixedHeight = height;
		}

		private void Release()
		{
			if (!IsInvalidated)
			{
				foreach (var renderbuffer in _attachedRenderbuffers)
					renderbuffer.Value.Dispose();
				_attachedRenderbuffers.Clear();

				foreach (var texture in _attachedTextures)
					texture.Value.Dispose();
				_attachedTextures.Clear();

				Platform.GL.glDeleteFramebuffers(ID);
				ID = -1;
			}
			if (GraphicsDevice.ViewContext == _attachedViewContext)
			{
				GraphicsDevice.ViewContext = null;
				_attachedViewContext = null;
			}

			_fixedWidth = 0;
			_fixedHeight = 0;
		}

		#region Adding attachments

		public void AttachViewContext()
		{
			if (IsInvalidated)
				throw new InvalidOperationException();
			if (_attachedViewContext != null)
				throw new InvalidOperationException("Existing ViewContext attachment.");

			if (IsUsingFixedDimensions)
				_attachedViewContext = new ViewContext(GraphicsDevice, new Rect(0, 0, _fixedWidth, _fixedHeight));
			else
				_attachedViewContext = new ViewContext(GraphicsDevice);
		}

		public void AttachTexture(FramebufferTextureFormat format)
		{
			if (IsInvalidated)
				throw new InvalidOperationException();

			var existingTexture = GetAttachedTexture(format);
			if (existingTexture != null)
				throw new InvalidOperationException("Texture attachment already exists for this format.");

			// also need to make sure a renderbuffer isn't already attached with the same format
			var renderbufferFormatToCheck = (FramebufferRenderbufferFormat)format;
			var existingRenderbuffer = GetAttachedRenderbuffer(renderbufferFormatToCheck);
			if (existingRenderbuffer != null)
				throw new InvalidOperationException("Renderbuffer attachment already exists with this same texture format.");

			// determine opengl format stuff equivalent to the format passed in
			TextureFormat textureFormat;
			int attachmentType;
			switch (format)
			{
				case FramebufferTextureFormat.RGB:
					textureFormat = TextureFormat.RGB;
					attachmentType = GL20.GL_COLOR_ATTACHMENT0;
					break;
				case FramebufferTextureFormat.RGBA:
					textureFormat = TextureFormat.RGBA;
					attachmentType = GL20.GL_COLOR_ATTACHMENT0;
					break;
				case FramebufferTextureFormat.Depth:
					textureFormat = TextureFormat.Depth;
					attachmentType = GL20.GL_DEPTH_ATTACHMENT;
					break;
				default:
					throw new InvalidOperationException();
			}

			int width;
			int height;
			GetDimensionsForAttachment(out width, out height);

			// pixelated == unfiltered
			var texture = new Texture(GraphicsDevice, width, height, textureFormat, (TextureParameters)TextureParameters.Pixelated.Clone());

			// don't have the GraphicsDevice automatically restore this texture!
			// since it's dependant on this Framebuffer object, we should let
			// the Framebuffer object do the restore itself
			GraphicsDevice.UnregisterManagedResource(texture);

			GraphicsDevice.BindFramebuffer(this);
			Platform.GL.glFramebufferTexture2D(GL20.GL_FRAMEBUFFER, attachmentType, GL20.GL_TEXTURE_2D, texture.ID, 0);
			GraphicsDevice.UnbindFramebuffer(this);

			_attachedTextures.Add(format, texture);
		}

		public void AttachRenderbuffer(FramebufferRenderbufferFormat format)
		{
			if (IsInvalidated)
				throw new InvalidOperationException();

			var existingRenderbuffer = GetAttachedRenderbuffer(format);
			if (existingRenderbuffer != null)
				throw new InvalidOperationException("Renderbuffer attachment already exists for this format.");

			// also need to make sure a texture isn't already attached with the same format
			var textureFormatToCheck = (FramebufferTextureFormat)format;
			var existingTexture = GetAttachedTexture(textureFormatToCheck);
			if (existingTexture != null)
				throw new InvalidOperationException("Texture attachment already exists with this same renderbuffer format.");

			// determine opengl format stuff equivalent to the format passed in
			RenderbufferFormat renderbufferFormat;
			int attachmentType;
			switch (format)
			{
				case FramebufferRenderbufferFormat.RGB:
					renderbufferFormat = RenderbufferFormat.RGB;
					attachmentType = GL20.GL_COLOR_ATTACHMENT0;
					break;
				case FramebufferRenderbufferFormat.RGBA:
					renderbufferFormat = RenderbufferFormat.RGBA;
					attachmentType = GL20.GL_COLOR_ATTACHMENT0;
					break;
				case FramebufferRenderbufferFormat.Depth:
					renderbufferFormat = RenderbufferFormat.Depth;
					attachmentType = GL20.GL_DEPTH_ATTACHMENT;
					break;
				case FramebufferRenderbufferFormat.Stencil:
					renderbufferFormat = RenderbufferFormat.Stencil;
					attachmentType = GL20.GL_STENCIL_ATTACHMENT;
					break;
				default:
					throw new InvalidOperationException();
			}

			int width;
			int height;
			GetDimensionsForAttachment(out width, out height);

			var renderbuffer = new Renderbuffer(GraphicsDevice, width, height, renderbufferFormat);

			// don't have the GraphicsDevice automatically restore this renderbuffer!
			// since it's dependant on this Framebuffer object, we should let
			// the Framebuffer object do the restore itself
			GraphicsDevice.UnregisterManagedResource(renderbuffer);

			GraphicsDevice.BindFramebuffer(this);
			Platform.GL.glFramebufferRenderbuffer(GL20.GL_FRAMEBUFFER, attachmentType,GL20.GL_RENDERBUFFER, renderbuffer.ID);
			GraphicsDevice.UnbindFramebuffer(this);

			_attachedRenderbuffers.Add(format, renderbuffer);
		}

		#endregion

		#region Removing attachments

		public void RemoveViewContext()
		{
			if (IsInvalidated)
				throw new InvalidOperationException();

			if (_attachedViewContext == null)
				return;

			if (GraphicsDevice.ViewContext == _attachedViewContext)
				GraphicsDevice.ViewContext = null;

			_attachedViewContext = null;
		}

		public void RemoveTexture(FramebufferTextureFormat format)
		{
			if (IsInvalidated)
				throw new InvalidOperationException();

			Texture existing = _attachedTextures.Get(format);
			if (existing == null)
				return;

			int attachmentType;
			switch (existing.Format)
			{
				case TextureFormat.RGB:   attachmentType = GL20.GL_COLOR_ATTACHMENT0; break;
				case TextureFormat.RGBA:  attachmentType = GL20.GL_COLOR_ATTACHMENT0; break;
				case TextureFormat.Depth: attachmentType = GL20.GL_DEPTH_ATTACHMENT; break;
				default:
					throw new InvalidOperationException();
			}

			GraphicsDevice.BindFramebuffer(this);
			Platform.GL.glFramebufferTexture2D(GL20.GL_FRAMEBUFFER, attachmentType, GL20.GL_TEXTURE_2D, 0, 0);
			GraphicsDevice.UnbindFramebuffer(this);

			_attachedTextures.Remove(format);
			existing.Dispose();
		}

		public void RemoveRenderbuffer(FramebufferRenderbufferFormat format)
		{
			if (IsInvalidated)
				throw new InvalidOperationException();

			Renderbuffer existing = _attachedRenderbuffers.Get(format);
			if (existing == null)
				return;

			int attachmentType;
			switch (existing.Format)
			{
				case RenderbufferFormat.RGB:     attachmentType = GL20.GL_COLOR_ATTACHMENT0; break;
				case RenderbufferFormat.RGBA:    attachmentType = GL20.GL_COLOR_ATTACHMENT0; break;
				case RenderbufferFormat.Depth:   attachmentType = GL20.GL_DEPTH_ATTACHMENT; break;
				case RenderbufferFormat.Stencil: attachmentType = GL20.GL_STENCIL_ATTACHMENT; break;
				default:
					throw new InvalidOperationException();
			}

			GraphicsDevice.BindFramebuffer(this);
			Platform.GL.glFramebufferRenderbuffer(GL20.GL_FRAMEBUFFER, attachmentType, GL20.GL_RENDERBUFFER, 0);
			GraphicsDevice.UnbindFramebuffer(this);

			_attachedRenderbuffers.Remove(format);
			existing.Dispose();
		}

		#endregion

		#region Re-attaching existing attachments

		private void RecreateAndAttach(FramebufferTextureFormat key)
		{
			var existing = _attachedTextures[key];

			int attachmentType;
			switch (existing.Format)
			{
				case TextureFormat.RGB:   attachmentType = GL20.GL_COLOR_ATTACHMENT0; break;
				case TextureFormat.RGBA:  attachmentType = GL20.GL_COLOR_ATTACHMENT0; break;
				case TextureFormat.Depth: attachmentType = GL20.GL_DEPTH_ATTACHMENT; break;
				default:
					throw new InvalidOperationException();
			}

			var format = existing.Format;
			int width;
			int height;
			GetDimensionsForAttachment(out width, out height);

			// this will essentially do nothing if we're recreating due to a new context
			// (existing.IsInvalidated will be true, so Dispose() won't release anything)
			existing.Dispose();

			// note that we recreate the texture instead of just calling it's OnNewContext()
			// method because OnNewContext() will recreate it using it's initial size and we
			// may be recreating+attaching due to a viewport resize where this framebuffer is
			// to be sized the same as the viewport (non-fixed size)
			// pixelated == unfiltered
			var newTexture = new Texture(GraphicsDevice, width, height, format, (TextureParameters)TextureParameters.Pixelated.Clone());

			// don't have the GraphicsDevice automatically restore this texture!
			// since it's dependant on this Framebuffer object, we should let
			// the Framebuffer object do the restore itself
			GraphicsDevice.UnregisterManagedResource(newTexture);

			GraphicsDevice.BindFramebuffer(this);
			Platform.GL.glFramebufferTexture2D(GL20.GL_FRAMEBUFFER, attachmentType, GL20.GL_TEXTURE_2D, newTexture.ID, 0);
			GraphicsDevice.UnbindFramebuffer(this);

			_attachedTextures[key] = newTexture;
		}

		private void RecreateAndAttach(FramebufferRenderbufferFormat key)
		{
			var existing = _attachedRenderbuffers[key];

			int attachmentType;
			switch (existing.Format)
			{
				case RenderbufferFormat.RGB:     attachmentType = GL20.GL_COLOR_ATTACHMENT0; break;
				case RenderbufferFormat.RGBA:    attachmentType = GL20.GL_COLOR_ATTACHMENT0; break;
				case RenderbufferFormat.Depth:   attachmentType = GL20.GL_DEPTH_ATTACHMENT; break;
				case RenderbufferFormat.Stencil: attachmentType = GL20.GL_STENCIL_ATTACHMENT; break;
				default:
					throw new InvalidOperationException();
			}

			var format = existing.Format;
			int width;
			int height;
			GetDimensionsForAttachment(out width, out height);

			// this will essentially do nothing if we're recreating due to a new context
			// (existing.IsInvalidated will be true, so Dispose() won't release anything)
			existing.Dispose();

			var newRenderbuffer = new Renderbuffer(GraphicsDevice, width, height, format);

			// don't have the GraphicsDevice automatically restore this renderbuffer!
			// since it's dependant on this Framebuffer object, we should let
			// the Framebuffer object do the restore itself
			GraphicsDevice.UnregisterManagedResource(newRenderbuffer);

			GraphicsDevice.BindFramebuffer(this);
			Platform.GL.glFramebufferRenderbuffer(GL20.GL_FRAMEBUFFER, attachmentType, GL20.GL_RENDERBUFFER, newRenderbuffer.ID);
			GraphicsDevice.UnbindFramebuffer(this);

			_attachedRenderbuffers[key] = newRenderbuffer;
		}

		#endregion

		private void GetDimensionsForAttachment(out int width, out int height)
		{
			if (IsUsingFixedDimensions)
			{
				width = _fixedWidth;
				height = _fixedHeight;
			}
			else
			{
				ViewContext currentViewContext = (_attachedViewContext == null ? GraphicsDevice.ViewContext : _attachedViewContext);
				width = currentViewContext.ViewportWidth;
				height = currentViewContext.ViewportHeight;
			}
		}

		public void OnResize()
		{
			if (IsInvalidated)
				return;

			// TODO: check that the check for GraphicsDevice.ViewContext != _attachedViewContext is actually needed
			if (_attachedViewContext != null && GraphicsDevice.ViewContext != _attachedViewContext)
			{
				Rect r = Platform.Application.Window.ClientRectangle;
				_attachedViewContext.OnResize(ref r, GraphicsDevice.ScreenOrientation);
			}

			// now recreate & reattach all the attachment points that were set
			foreach (var texture in _attachedTextures)
				RecreateAndAttach(texture.Key);
			foreach (var renderbuffer in _attachedRenderbuffers)
				RecreateAndAttach(renderbuffer.Key);
		}

		#region GraphicsContextResource

		public override void OnNewContext()
		{
			// recreate using the same settings
			Create(_fixedWidth, _fixedHeight);

			// TODO: check that the check for GraphicsDevice.ViewContext != _attachedViewContext is actually needed
			if (_attachedViewContext != null && GraphicsDevice.ViewContext != _attachedViewContext)
				_attachedViewContext.OnNewContext();

			// now recreate & reattach all the attachment points that were set
			foreach (var texture in _attachedTextures)
				RecreateAndAttach(texture.Key);
			foreach (var renderbuffer in _attachedRenderbuffers)
				RecreateAndAttach(renderbuffer.Key);
		}

		public override void OnLostContext()
		{
			ID = -1;
			// TODO: check that the check for GraphicsDevice.ViewContext != _attachedViewContext is actually needed
			if (_attachedViewContext != null && GraphicsDevice.ViewContext != _attachedViewContext)
				_attachedViewContext.OnLostContext();
			foreach (var texture in _attachedTextures)
				texture.Value.OnLostContext();
			foreach (var renderbuffer in _attachedRenderbuffers)
				renderbuffer.Value.OnLostContext();
		}

		protected override bool ReleaseResource()
		{
			if (!IsInvalidated)
			{
				Release();
				ID = -1;
			}

			return true;
		}

		#endregion
	}
}
