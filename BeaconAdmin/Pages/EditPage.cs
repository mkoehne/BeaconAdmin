using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Acr.UserDialogs;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;

namespace BeaconAdmin
{
	public class EditPage : ContentPage
	{
		Entry uuid;
		Entry major;
		Entry minor;
		IDevice device;
		IService service;
		ICharacteristic uuidCharacteristic;
		ICharacteristic majorCharacteristic;
		ICharacteristic minorCharacteristic;
		Button updateUUID;
		Button updateMajor;
		Button updateMinor;

		public EditPage(IDevice device)
		{
			this.device = device;
			uuid = new Entry();

			updateUUID = new Button();
			updateUUID.Text = "Update UUID Value";
			updateUUID.Clicked += UpdateUUID_Clicked;

			major = new Entry();
			major.Keyboard = Keyboard.Numeric;

			updateMajor = new Button();
			updateMajor.Text = "Update Major Value";
			updateMajor.Clicked += UpdateMajor_Clicked;

			minor = new Entry();
			minor.Keyboard = Keyboard.Numeric;

			updateMinor = new Button();
			updateMinor.Text = "Update Minor Value";
			updateMinor.Clicked += UpdateMinor_Clicked; ;

			Content = new StackLayout
			{
				Padding = 10,
				Children = {
					new Label { Text = "UUID" },
					uuid,
					updateUUID,
					new Label { Text = "Major" },
					major,
					updateMajor,
					new Label { Text = "Minor" },
					minor,
					updateMinor
				}
			};
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			service = await device.GetServiceAsync(Guid.Parse("f0010200-f001-f001-f001-f001f001f001"));

			uuidCharacteristic = await service.GetCharacteristicAsync(Guid.Parse("f0010201-f001-f001-f001-f001f001f001"));
			uuidCharacteristic.ValueUpdated += UuidCharacteristic_ValueUpdated;
			if (uuidCharacteristic.CanUpdate)
				uuidCharacteristic.StartUpdates();
			ReadUUID();

			majorCharacteristic = await service.GetCharacteristicAsync(Guid.Parse("f0010202-f001-f001-f001-f001f001f001"));
			majorCharacteristic.ValueUpdated += MajorCharacteristic_ValueUpdated;
			if (majorCharacteristic.CanUpdate)
				majorCharacteristic.StartUpdates();
			ReadMajor();

			minorCharacteristic = await service.GetCharacteristicAsync(Guid.Parse("f0010203-f001-f001-f001-f001f001f001"));
			minorCharacteristic.ValueUpdated += MinorCharacteristic_ValueUpdated;
			if (minorCharacteristic.CanUpdate)
				minorCharacteristic.StartUpdates();
			ReadMinor();
		}

		protected override void OnDisappearing()
		{
			Settings.Adapter.DisconnectDeviceAsync(device);
			base.OnDisappearing();
		}

		async void UpdateUUID_Clicked(object sender, EventArgs e)
		{
			if (uuidCharacteristic.CanWrite)
			{
				UserDialogs.Instance.ShowLoading("Writing data...");
				string value = uuid.Text.Replace("-", "");
				value = value.Replace(" ", "");
				byte[] data = StringToByteArray(value);

				try
				{
					await uuidCharacteristic.WriteAsync(data);
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.Message, "Ok");
				}
				UserDialogs.Instance.HideLoading();
			}
		}

		async void UpdateMajor_Clicked(object sender, EventArgs e)
		{
			if (majorCharacteristic.CanWrite)
			{
				UserDialogs.Instance.ShowLoading("Writing data...");

				string hex = Convert.ToInt32(major.Text).ToString("X4");
				byte[] data = StringToByteArray(hex);
				Array.Reverse(data);

				try
				{

					await majorCharacteristic.WriteAsync(data);
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.Message, "Ok");
				}

				UserDialogs.Instance.HideLoading();
			}
		}

		async void UpdateMinor_Clicked(object sender, EventArgs e)
		{
			if (minorCharacteristic.CanWrite)
			{
				UserDialogs.Instance.ShowLoading("Writing data...");
				string hex = Convert.ToInt32(minor.Text).ToString("X4");
				byte[] data = StringToByteArray(hex);
				Array.Reverse(data);

				try
				{
					await minorCharacteristic.WriteAsync(data);
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.Message, "Ok");
				}
				UserDialogs.Instance.HideLoading();
			}
		}

		void UuidCharacteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
		{
			uuid.Text = ByteArrayToString(e.Characteristic.Value);
		}

		void MajorCharacteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
		{
			major.Text = ByteArrayToString(e.Characteristic.Value);
		}

		void MinorCharacteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
		{
			minor.Text = ByteArrayToString(e.Characteristic.Value);
		}

		private async void ReadUUID()
		{
			var uuidBytes = await uuidCharacteristic.ReadAsync();
			uuid.Text = ByteArrayToString(uuidBytes);
		}

		private async void ReadMajor()
		{
			var majorBytes = await majorCharacteristic.ReadAsync();
			major.Text = ByteArrayToString(majorBytes);
		}

		private async void ReadMinor()
		{
			var minorBytes = await minorCharacteristic.ReadAsync();
			minor.Text = ByteArrayToString(minorBytes);
		}

		/// <summary>
		/// Bytes array to string.
		/// </summary>
		/// <returns>The array to string.</returns>
		/// <param name="bytes">Bytes.</param>
		private string ByteArrayToString(byte[] bytes)
		{
			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			string stringValue = BitConverter.ToString(bytes, 0);
			try
			{
				int value = int.Parse(stringValue.Replace("-", ""), NumberStyles.AllowHexSpecifier);
				return value.ToString();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				stringValue = stringValue.Replace("-", "");
			}
			return stringValue;
		}

		/// <summary>
		/// Strings to byte array.
		/// </summary>
		/// <returns>The to byte array.</returns>
		/// <param name="str">String.</param>
		/*private byte[] StringToByteArray(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}*/

		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				.ToArray();
		}
	}
}


