using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using HidSharp;
using wtf.cluster.JoyCon;
using wtf.cluster.JoyCon.Calibration;
using wtf.cluster.JoyCon.ExtraData;
using wtf.cluster.JoyCon.InputData;
using wtf.cluster.JoyCon.InputReports;
using Environment = System.Environment;

namespace ControllerCursor;

public class JoyConRead : IGamepadReader {
	public event Action NextSlide;
	public event Action PrevSlide;

	private CalibrationData _calibration;
	private ImuReport _imuReport;
	private bool visibility;

	private bool wasPrevPressed;
	private bool wasNextPressed;

	public async Task Read() {
		HidDevice? device = GetHidDevice();
		if (device == null) {
			GD.PrintErr("No controller. Please connect Joy-Con via Bluetooth.");
			return;
		}
		JoyCon joycon = new(device);
		joycon.Start();
		await joycon.SetInputReportModeAsync(JoyCon.InputReportType.Full);

		await LogDeviceInfo(joycon);

		await joycon.EnableImuAsync(true);
		CalibrationData facCal = await joycon.GetFactoryCalibrationAsync();
		CalibrationData userCal = await joycon.GetUserCalibrationAsync();
		_calibration = facCal + userCal;

		joycon.ReportReceived += OnJoyConOnReportReceived;

		joycon.StoppedOnError += (_, ex) => {
			GD.Print();
			GD.PrintErr($"Critical error: {ex.Message}");
			GD.PrintErr("Controller polling stopped.");
			return Task.CompletedTask;
		};

		GD.Print("JoyCon ready for presenting.");
		// while (true) {
		// 	await Task.Yield();
		// }
	}

	private static async Task LogDeviceInfo(JoyCon joycon) {
		DeviceInfo deviceInfo = await joycon.GetDeviceInfoAsync();
		GD.Print(
			$"Type: {deviceInfo.ControllerType}, Firmware: {deviceInfo.FirmwareVersionMajor}.{deviceInfo.FirmwareVersionMinor}");
		string? serial = await joycon.GetSerialNumberAsync();
		GD.Print($"Serial number: {serial ?? "<none>"}");
		ControllerColors? colors = await joycon.GetColorsAsync();
		GD.Print(colors != null
			? $"Body color: {colors.BodyColor}, buttons color: {colors.ButtonsColor}"
			: "Colors not specified, seems like the controller is grey.");
	}

	private static HidDevice? GetHidDevice() {
		DeviceList list = DeviceList.Local;
		IEnumerable<HidDevice>? nintendos = list.GetHidDevices(0x057e);

		return nintendos.FirstOrDefault();
	}

	private Task OnJoyConOnReportReceived(JoyCon _, IJoyConReport input) {
		if (input is not InputFullWithImu j) {
			GD.Print($"Invalid input report type: {input.GetType()}");
			return Task.CompletedTask;
		}

		bool prev = PreviousPressed(j.Buttons);
		bool next = NextPressed(j.Buttons);

		_imuReport = j.Imu;

		visibility = j.Buttons.Y;

		if (prev && !wasPrevPressed)
			PrevSlide?.Invoke();
		if (next && !wasNextPressed)
			NextSlide?.Invoke();

		wasPrevPressed = prev;
		wasNextPressed = next;

		return Task.CompletedTask;
	}

	private static bool PreviousPressed(ButtonsFull input) {
		return input.L || input.R || input.B;
	}
	private static bool NextPressed(ButtonsFull input) {
		return input.ZL || input.ZR || input.A;
	}

	public Vector3 GetRotationalInput(double deltaTime) {
		if (_calibration.ImuCalibration == null)
			return Vector3.Zero;

		ImuFrameCalibrated calibratedImu = _imuReport.Frames[0].GetCalibrated(_calibration.ImuCalibration);
		return new Vector3(
			(float)Mathf.DegToRad(calibratedImu.GyroY * deltaTime),
			(float)Mathf.DegToRad(-calibratedImu.GyroZ * deltaTime),
			(float)Mathf.DegToRad(-calibratedImu.GyroX * deltaTime)
		);
	}

	public bool VisibilityPressed() {
		return visibility;
	}
}
