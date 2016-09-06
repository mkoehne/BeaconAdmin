using System;
using Plugin.BLE.Abstractions.Contracts;

namespace BeaconAdmin
{
	public static class Settings
	{
		public static IAdapter Adapter { get; set; }
		public static IBluetoothLE BLE { get; set; }
	}
}

