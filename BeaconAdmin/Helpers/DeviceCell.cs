using System;
using Xamarin.Forms;

namespace BeaconAdmin
{
	public class DeviceCell : ViewCell
	{
		public DeviceCell()
		{
			var nameContent = new Label
			{
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.Center
			};
			nameContent.SetBinding(Label.TextProperty, "Name");

			var nameLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(5, 0, 5, 0),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { nameContent }
			};

			var rssiContent = new Label
			{
				HorizontalTextAlignment = TextAlignment.End,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.Center
			};
			rssiContent.SetBinding(Label.TextProperty, "Rssi");

			var rssiLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(5, 0, 5, 0),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { rssiContent }
			};

			var layout = new StackLayout
			{
				Padding = new Thickness(10, 0, 10, 0),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Children = { nameLayout, rssiLayout }
			};

			View = layout;
		}
	}
}
