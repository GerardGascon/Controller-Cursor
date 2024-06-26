﻿#if GODOT_WINDOWS
using System.Runtime.InteropServices;

namespace ControllerCursor.Win32Api;

[StructLayout(LayoutKind.Explicit)]
public struct InputUnion {
	[FieldOffset(0)] public MouseInput mi;
	[FieldOffset(0)] public KeyboardInput ki;
	[FieldOffset(0)] public HardwareInput hi;
}
#endif
