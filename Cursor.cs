using Godot;

namespace ControllerCursor;

public partial class Cursor : Sprite2D {
	private Vector2 initialPosition;

	private bool _wasVisible;
	private bool _visible;

	[Export] private float distance = 10;
	[Export] private CollisionPolygon2D clickPolygon;

	private Quaternion _joyconRotation;

	public override void _Ready() {
		Position = GetViewportRect().Size / 2;
		initialPosition = Position;
		Visible = false;
	}

	public override void _Process(double delta) {
		if (ManageVisibility()) {
			RemoveWindowInteractability();
			return;
		}

		UpdateClickPolygon();

		Rotation = -_joyconRotation.GetEuler().Z;

		Vector3? intersection = Intersect();
		if (intersection != null)
			Position = initialPosition + new Vector2(intersection.Value.X, -intersection.Value.Y);
	}

	private void RemoveWindowInteractability() {
		GetWindow().MousePassthroughPolygon = new[] {
			Vector2.Zero,
			Vector2.Zero,
		};
	}

	private void UpdateClickPolygon() {
		Vector2[] clickPolygonPoints = clickPolygon.Polygon;
		for (int i = 0; i < clickPolygonPoints.Length; i++)
			clickPolygonPoints[i] = ToGlobal(clickPolygonPoints[i]);

		GetWindow().MousePassthroughPolygon = clickPolygonPoints;
	}

	private bool ManageVisibility() {
		if (!_visible) {
			if (_wasVisible)
				Visible = false;

			_wasVisible = false;
			return true;
		}

		if (!_wasVisible) {
			Visible = true;
			_joyconRotation = Quaternion.Identity;
		}

		_wasVisible = true;
		return false;
	}

	private Vector3? Intersect() {
		Plane plane = new(Vector3.Back);
		Vector3 rayOrigin = new(0, 0, distance);

		Vector3? intersection = plane.IntersectsRay(rayOrigin, _joyconRotation * Vector3.Forward);
		return intersection;
	}

	public void SetRotationalInput(Vector3 rotation) {
		_joyconRotation *= Quaternion.FromEuler(rotation);
		_joyconRotation = _joyconRotation.Normalized();
	}

	public void SetVisibility(bool visible) {
		_visible = visible;
	}
}
