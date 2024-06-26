using Godot;

namespace ControllerCursor;

public partial class JoyConReader : Node {
	[Export] private Cursor _window;

	private JoyConRead _reader;
	private SlideSwitcher _switcher;

	private Quaternion rotation = new(0, 0, 0, 1);

	public override async void _Ready() {
		_reader = new JoyConRead();
		_switcher = new SlideSwitcher(_reader);
		await _reader.Read();
	}

	public override void _Process(double delta) {
		_window.SetRotationalInput(_reader.GetRotationalInput(delta));
		_window.SetVisibility(_reader.VisibilityPressed());
	}

	public override void _ExitTree() {
		_switcher.Dispose();
	}
}
