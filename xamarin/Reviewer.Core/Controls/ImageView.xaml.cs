using System;
using System.Collections.Generic;
using Reviewer.Core;
using Xamarin.Forms;

namespace PhotoTour.Core
{
	public partial class ImageView : ContentView
	{
		public ImageView()
		{
			InitializeComponent();

			GestureRecognizers.Add(new TapGestureRecognizer(async (obj) =>
			{
				if (!(obj is ImageView img))
					return;

				if (!(img.theImage.Source is UriImageSource uriImage))
					return;

				await Navigation.PushAsync(new PhotoViewerPage(uriImage.Uri.AbsoluteUri));
			}));
		}

		public Image TheImage { get => theImage; }
	}
}
