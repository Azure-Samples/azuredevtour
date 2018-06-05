using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.Core;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Newtonsoft.Json.Bson;
using System.Threading.Tasks;


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

            MessagingCenter.Subscribe<BlockUserMessage>(this, BlockUserMessage.Message, async (obj) =>
            {
                vm.CurrentPage = 1;
                await LoadPhotoCollection();
            });
        }

        async protected override void OnAppearing()
        {
            base.OnAppearing();

            vm.NewPhotoAdded += HandleNewPhoto;

            if (flexLayout.Children.Count == 0)
            {
                await LoadPhotoCollection();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            vm.NewPhotoAdded -= HandleNewPhoto;
        }

        async void HandleNewPhoto(object sender, EventArgs eventArgs)
        {
            //var iv = new ImageView();
            //iv.WidthRequest = photoWidth;
            //iv.HeightRequest = photoHeight;
            //iv.TheImage.Source = ImageSource.FromUri(photoUri);

            //flexLayout.Children.Add(iv);

            // Jump to the last page to show the latest photo
            vm.CurrentPage = vm.TotalPages;
            await LoadPhotoCollection();
        }

        async Task LoadPhotoCollection()
        {
            var photos = await vm.LoadPhotos();

            if (photos == null)
                return;

            flexLayout.Children.Clear();

            foreach (var photo in photos)
            {
                var iv = new ImageView();
                iv.WidthRequest = photoWidth;
                iv.HeightRequest = photoHeight;

                Uri photoUri = null;

                iv.TheImage.Source = ImageSource.FromUri(new Uri($"{photo.PhotoUrl}"));

                flexLayout.Children.Add(iv);
            }
        }

        async void Handle_NextPageClicked(object sender, EventArgs args)
        {
            vm.CurrentPage += 1;
            await LoadPhotoCollection();
        }

        async void Handle_PreviousPageClicked(object sender, EventArgs args)
        {
            vm.CurrentPage -= 1;
            await LoadPhotoCollection();
        }
    }
}

