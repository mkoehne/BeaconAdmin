using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Acr.UserDialogs;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Forms;

namespace BeaconAdmin
{

	public class DevicePage : ContentPage
	{
		List<IDevice> deviceList;
		ListView listView;

		public DevicePage()
		{
			this.Title = "Devices";
			listView = new ListView();
			deviceList = new List<IDevice>();

			Settings.BLE = CrossBluetoothLE.Current;
			Settings.Adapter = CrossBluetoothLE.Current.Adapter;

			ToolbarItems.Add(new ToolbarItem("Scan", "", () =>
			 {
				 ScanForDevices();
			 }));

			listView.ItemTemplate = new DataTemplate(typeof(DeviceCell));

			// Using ItemTapped
			listView.ItemTapped += async (sender, e) =>
			{
				IDevice device = (IDevice)e.Item;
				try
				{
					await Settings.Adapter.ConnectToDeviceAsync(device);
				}
				catch (DeviceConnectionException ex)
				{
					// ... could not connect to device
					UserDialogs.Instance.Alert("Could not connect to device" + ex.Message, "Error", "Ok");
				}

				((ListView)sender).SelectedItem = null; // de-select the row

				await Navigation.PushAsync(new EditPage(device));
			};

			// If using ItemSelected
			listView.ItemSelected += (sender, e) =>
			{
				if (e.SelectedItem == null) return;
				Debug.WriteLine("Selected: " + e.SelectedItem);
				((ListView)sender).SelectedItem = null; // de-select the row
			};

			Padding = new Thickness(0, 0, 0, 0);
			Content = listView;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			Settings.Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
		}

		private async void ScanForDevices()
		{
			deviceList.Clear();
			UserDialogs.Instance.ShowLoading("Scanning...");
			await Settings.Adapter.StartScanningForDevicesAsync();
			listView.ItemsSource = deviceList.OrderByDescending(x => x.Rssi);
			UserDialogs.Instance.HideLoading();
		}

		void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Device.Name))
				deviceList.Add(e.Device);
		}
	}
}