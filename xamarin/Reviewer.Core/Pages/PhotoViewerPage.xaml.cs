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

            var tapped = new TapGestureRecognizer(async (arg1) =>
           {
               await DisplayAlert("Cog Services!", "We're going to analyze the image", "OK");

               if (!(arg1 is Image img))
                   return;

               if (!(img.Source is UriImageSource uri))
                   return;

               img.Source = ImageSource.FromUri(new Uri($"https://test-azuretour-imageanalyzer.azurewebsites.net/api/analyzeimage?url={uri.Uri.AbsoluteUri}"));
           });
            tapped.NumberOfTapsRequired = 5;
            detailImage.GestureRecognizers.Add(tapped);
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

        protected async void Issue_Clicked(object sender, EventArgs eventArgs)
        {
            const string block_user = "Block User";
            const string report_image = "Report Image";

            var result = await DisplayActionSheet("Image Issue", "Cancel", null, block_user, report_image);

            if (result == block_user)
            {
                var continueBlock = await DisplayAlert("Block User",
                   $"By blocking this user you will no longer see their photos and they will be reported.{Environment.NewLine}{Environment.NewLine}Do you wish to continue?", "Yes", "No");

                if (continueBlock)
                {
                    viewModel.BlockUser();

                    await Navigation.PopAsync(true);
                }
            }
            else if (result == report_image)
            {
                var continueReport = await DisplayAlert("Report Image",
                    $"By reporting this image you will no longer see it in your timeline and it will be reported as objectionable.{Environment.NewLine}{Environment.NewLine}Do you wish to continue?", "Yes", "No");

                if (continueReport)
                {
                    await viewModel.ReportImage();

                    await Navigation.PopAsync(true);
                }
            }
        }
    }
}
