using System;
using ControllerCursor.KeyPressSimulator;

namespace ControllerCursor;

public class SlideSwitcher : IDisposable {
	private readonly JoyConRead _reader;

#if GODOT_WINDOWS
	private readonly IKeyPresses _keyPresses = new WindowsKeyPress();
#elif GODOT_OSX
	private readonly IKeyPresses _keyPresses = new MacOsKeyPress();
#elif GODOT_LINUXBSD
	private readonly IKeyPresses _keyPresses = new LinuxKeyPress();
#endif

	public SlideSwitcher(JoyConRead reader) {
		_reader = reader;
		_reader.NextSlide += NextSlide;
		_reader.PrevSlide += PreviousSlide;
	}

	public void Dispose() {
		_reader.NextSlide -= NextSlide;
		_reader.PrevSlide -= PreviousSlide;
	}

	private void NextSlide() => _keyPresses.NextSlide();
	private void PreviousSlide() => _keyPresses.PreviousSlide();
}