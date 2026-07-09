using System;
using Godot;

namespace ControllerCursor;

public class ScreenSwitcher : IDisposable {
	private readonly JoyConRead _reader;
	private readonly Window _window;

	public ScreenSwitcher(JoyConRead reader, Window window) {
		_reader = reader;
		_window = window;
		_reader.SwitchScreen += SwitchScreen;
	}

	public void Dispose() {
		_reader.SwitchScreen -= SwitchScreen;
	}

	private void SwitchScreen() {
		_window.CurrentScreen = Mathf.Clamp(_window.CurrentScreen + 1, 0, DisplayServer.GetScreenCount() - 1);
	}
}