using System;
using Godot;

namespace ControllerCursor;

public class ScreenSwitcher : IDisposable {
	private readonly JoyConRead _reader;
	private readonly Window _window;

	public ScreenSwitcher(JoyConRead reader, Window window) {
		_reader = reader;
		_window = window;
	}

	public void Dispose() { }

	public void SwitchScreen() {
		_window.CurrentScreen = (_window.CurrentScreen + 1) % DisplayServer.GetScreenCount();
	}
}