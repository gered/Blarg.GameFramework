using System;
using PortableGL;

namespace Blarg.GameFramework.Graphics
{
	public class ViewContext
	{
		readonly GraphicsDevice _graphicsDevice;
		Rect _viewport;
		bool _viewportIsFixedSize;
		ScreenOrientation _screenOrientation;
		Camera _camera;
		bool _isUsingDefaultCamera;

		Matrix4x4 _modelViewMatrix;
		Matrix4x4 _projectionMatrix;

		public int ViewportTop { get { return _viewport.Top; } }
		public int ViewportLeft { get { return _viewport.Left; } }
		public int ViewportBottom { get { return _viewport.Bottom; } }
		public int ViewportRight { get { return _viewport.Right; } }
		public int ViewportWidth { get { return _viewport.Width; } }
		public int ViewportHeight { get { return _viewport.Height; } }

		public bool IsViewportFixedSize { get { return _viewportIsFixedSize; } }
		public bool IgnoringScreenRotation { get { return _viewportIsFixedSize; } }

		public Camera Camera
		{
			get
			{
				return _camera;
			}
			set
			{
				bool cameraWasChanged = false;

				// using the default camera but a new camera is being provided?
				if (_isUsingDefaultCamera && value != null)
				{
					_isUsingDefaultCamera = false;
					_camera = value;
					cameraWasChanged = true;
				}

				// not using the default camera already, but setting a new camera
				else if (!_isUsingDefaultCamera && value != null)
				{
					_camera = value;
					cameraWasChanged = true;
				}

				// not using the default camera, and clearing ("nulling") the camera
				else if (!_isUsingDefaultCamera && value == null)
				{
					_camera = new Camera(this);
					_isUsingDefaultCamera = true;
					cameraWasChanged = true;
				}

				// update our local projection matrix if a new camera was applied
				// (otherwise, we wouldn't get it until the next OnResize...)
				if (cameraWasChanged)
					ProjectionMatrix = _camera.Projection;
			}
		}

		public Matrix4x4 ProjectionMatrix
		{
			get
			{
				return _projectionMatrix;
			}
			set
			{
				if (!IgnoringScreenRotation && _screenOrientation != ScreenOrientation.Rotation0)
				{
					// apply a rotation immediately _after_ the projection matrix transform
					Matrix4x4 rotation;
					Matrix4x4.CreateRotationZ(MathHelpers.DegreesToRadians(-((float)_screenOrientation)), out rotation);
					Matrix4x4.Multiply(ref rotation, ref value, out _projectionMatrix);
				}
				else
					_projectionMatrix = value;
			}
		}

		public Matrix4x4 ModelViewMatrix
		{
			get
			{
				return _modelViewMatrix;
			}
			set
			{
				_modelViewMatrix = value;
			}
		}

		public Matrix4x4 OrthographicProjectionMatrix
		{
			get
			{
				Matrix4x4 ortho;
				Matrix4x4.CreateOrthographic((float)_viewport.Left, (float)_viewport.Right, (float)_viewport.Top, (float)_viewport.Bottom, 0.0f, 1.0f, out ortho);

				if (!IgnoringScreenRotation && _screenOrientation != ScreenOrientation.Rotation0)
				{
					// apply a rotation immediately _after_ the projection matrix transform
					Matrix4x4 rotation;
					Matrix4x4.CreateRotationZ(MathHelpers.DegreesToRadians(-((float)_screenOrientation)), out rotation);
					Matrix4x4.Multiply(ref rotation, ref ortho, out ortho);
				}

				return ortho;
			}
		}

		public ViewContext(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			_graphicsDevice = graphicsDevice;
			/*
			var r = new Rect(
				_graphicsDevice.Window.ClientRectangle.Left,
				_graphicsDevice.Window.ClientRectangle.Top,
				_graphicsDevice.Window.ClientRectangle.Right,
				_graphicsDevice.Window.ClientRectangle.Bottom
				);
				*/
			var r = new Rect(0, 0, Framework.Application.Window.ClientWidth, Framework.Application.Window.ClientHeight);
			Init(r, false);
		}

		public ViewContext(GraphicsDevice graphicsDevice, Rect fixedViewportSize)
		{
			if (graphicsDevice == null)
				throw new ArgumentNullException("graphicsDevice");

			_graphicsDevice = graphicsDevice;
			Init(fixedViewportSize, true);
		}

		private void Init(Rect viewport, bool isFixedSizeViewport)
		{
			_viewport = viewport;
			_viewportIsFixedSize = isFixedSizeViewport;
			_screenOrientation = ScreenOrientation.Rotation0;
			_camera = new Camera(this);
			_isUsingDefaultCamera = true;
		}

		public void OnNewContext()
		{
			_modelViewMatrix = Matrix4x4.Identity;
			_projectionMatrix = Matrix4x4.Identity;
		}

		public void OnLostContext()
		{
		}

		public void OnResize(ref Rect size, ScreenOrientation screenOrientation = ScreenOrientation.Rotation0)
		{
			SetupViewport(ref size, screenOrientation);
		}

		public void OnRender(float delta)
		{
			if (_camera != null)
				_camera.OnRender(delta);
		}

		public void OnApply(ref Rect size, ScreenOrientation screenOrientation = ScreenOrientation.Rotation0)
		{
			SetupViewport(ref size, screenOrientation);

			// ensures it's set up for rendering immediately when this call returns
			// NOTE: we assume OnApply() is going to be called in some other class's
			//       OnRender() event only (like, e.g. if a new framebuffer is bound)
			if (_camera != null)
				_camera.OnRender(0.0f);
		}

		private void SetupViewport(ref Rect size, ScreenOrientation screenOrientation)
		{
			Rect viewport;

			if (_viewportIsFixedSize)
			{
				viewport = _viewport;
				_screenOrientation = ScreenOrientation.Rotation0;
			}
			else
			{
				// based on the orientation, we may need to swap the width/height
				// of the passed viewport dimensions
				// (we don't do viewport rotation if the viewport is fixed)
				if (!IgnoringScreenRotation && (screenOrientation == ScreenOrientation.Rotation90 || screenOrientation == ScreenOrientation.Rotation270))
				{
					// swap width and height
					viewport.Left = size.Top;
					viewport.Top = size.Left;
					viewport.Right = size.Bottom;
					viewport.Bottom = size.Right;
				}
				else
					viewport = size;

				// we **don't** want this to be rotated
				_viewport = size;

				_screenOrientation = screenOrientation;
			}

			// we **do** obviously want this to be rotated (if there is a rotation)
			_graphicsDevice.GL.glViewport(viewport.Left, viewport.Top, viewport.Width, viewport.Height);

			// we also **don't** want the camera to work with a rotated viewport
			if (_camera != null)
				_camera.OnResize(ref _viewport);
		}
	}
}
