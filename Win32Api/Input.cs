#if GODOT_WINDOWS
using System.Runtime.InteropServices;

namespace ControllerCursor.Win32Api;

[StructLayout(LayoutKind.Sequential)]
public struct Input {
	public uint type;
	public InputUnion u;
}
#endif
