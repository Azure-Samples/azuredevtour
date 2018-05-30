using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.Core;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;


namespace PhotoTour.Core
{
	public partial class PhotoListPage : ContentPage
	{
		readonly int photoHeight = 120;
		readonly int photoWidth = 120;

		PhotosListViewModel vm;
		public PhotoListPage()
		{
			InitializeComponent();

			On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

			vm = new PhotosListViewModel();
			BindingContext = vm;

			LoadPhotoCollection();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			vm.NewPhotoAdded += HandleNewPhoto;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			vm.NewPhotoAdded -= HandleNewPhoto;
		}

		void HandleNewPhoto(object sender, Uri photoUri)
		{
			var iv = new ImageView();
			iv.WidthRequest = photoWidth;
			iv.HeightRequest = photoHeight;
			iv.TheImage.Source = ImageSource.FromUri(photoUri);

			flexLayout.Children.Add(iv);
		}

		async void LoadPhotoCollection()
		{
			var photos = await vm.LoadPhotos();

			if (photos == null)
				return;

			foreach (var photo in photos)
			{
				var iv = new ImageView();
				iv.WidthRequest = photoWidth;
				iv.HeightRequest = photoHeight;
				iv.TheImage.Source = ImageSource.FromUri(new Uri($"{photo.PhotoUrl}"));

				flexLayout.Children.Add(iv);
			}
		}

	}
}

