using Godot;

namespace BottomlessPit.Scripts;

public partial class PixelPerfectCamera2D : Camera2D {
	[Export]
	public Vector2 _referenceResolution = new (480, 270);

	public Window _window;

	public override void _Ready() {
		_window = GetTree().Root;
		_window.SizeChanged += _UpdateScale;

		_UpdateScale();
	}

	public void _UpdateScale() {
		// GetViewportRect().Size // Actual 1:n pixel resolution.
		// GetTree().Root.Size // Window size in screen pixels.

		var viewportSize = _window.Size;
		var scaleFactor = new Vector2(
			Mathf.Floor(viewportSize.X / _referenceResolution.X),
			Mathf.Floor(viewportSize.Y / _referenceResolution.Y)
		);

		var minScaleFactor = Mathf.Min(scaleFactor.X, scaleFactor.Y);

		_window.ContentScaleFactor = minScaleFactor;
	}
}