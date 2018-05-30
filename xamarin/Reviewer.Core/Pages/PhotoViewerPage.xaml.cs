using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Reviewer.Core
{
	public partial class PhotoViewerPage : ContentPage
	{
		PhotoViewerViewModel viewModel;
		public PhotoViewerPage(string photoUrl)
		{
			InitializeComponent();

			On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

			viewModel = new PhotoViewerViewModel(photoUrl);

			BindingContext = viewModel;
		}

		public PhotoViewerPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
		}
	}
}
