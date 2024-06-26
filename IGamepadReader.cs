using System;
using System.Threading.Tasks;

namespace ControllerCursor;

public interface IGamepadReader {
	public event Action NextSlide;
	public event Action PrevSlide;

	public Task Read();
}