using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using Plugin.Connectivity;
using PhotoTour.Services;
using Microsoft.Identity.Client;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

namespace PhotoTour.Core
{
	public class PhotosListViewModel : BaseViewModel
	{
		readonly string logoutText = "Log Out";
		readonly string loginText = "Log In";

		IDataService dataService;

		bool isLoggedIn = false;
		public bool IsLoggedIn
		{
			get => isLoggedIn;
			set
			{
				SetProperty(ref isLoggedIn, value);
				IsNotLoggedIn = !IsLoggedIn;
				LoginCommand?.ChangeCanExecute();
			}
		}

		bool isNotLoggedIn = true;
		public bool IsNotLoggedIn { get => isNotLoggedIn; set => SetProperty(ref isNotLoggedIn, value); }

		string loginButtonText;
		public string LoginButtonText { get => loginButtonText; set => SetProperty(ref loginButtonText, value); }

		public Command LoginCommand { get; }

		public ICommand TakePhotoCommand { get; }

		public event EventHandler<Uri> NewPhotoAdded;

		AuthenticationResult authResult;
		IIdentityService identityService;

		public PhotosListViewModel()
		{
			dataService = DependencyService.Get<IDataService>();
			Title = "All Photos";

			IsLoggedIn = false;

			LoginCommand = new Command(async () => await ExecuteLoginCommand());
			TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());

			identityService = DependencyService.Get<IIdentityService>();

			Task.Run(async () => await CheckLoginStatus());
		}

		public async Task CheckLoginStatus()
		{
			authResult = await identityService.GetCachedSignInToken();

			Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
			{
				IsLoggedIn = authResult?.User != null;

				LoginButtonText = IsLoggedIn ? logoutText : loginText;
			});
		}

		public async Task<List<Photo>> LoadPhotos()
		{
			if (IsBusy || !CrossConnectivity.Current.IsConnected)
				return new List<Photo>();

			try
			{
				IsBusy = true;
				var photos = await dataService.GetAllPhotos();

				return photos;
			}
			finally
			{
				IsBusy = false;
			}
		}

		async Task ExecuteLoginCommand()
		{
			if (IsBusy)
				return;

			if (!CrossConnectivity.Current.IsConnected)
			{
				await Application.Current.MainPage.DisplayAlert("No Internet", "Cannot Login - No Internet", "OK");
				return;
			}

			if (IsNotLoggedIn)
			{
				try
				{
					IsBusy = true;

					authResult = await identityService.Login();
				}
				finally
				{
					IsBusy = false;
				}

				if (authResult?.User == null)
				{
					IsLoggedIn = false;
					LoginButtonText = loginText;
				}
				else
				{
					IsLoggedIn = true;
					LoginButtonText = logoutText;

					Analytics.TrackEvent("Login", new Dictionary<string, string> { { "displayName", identityService.DisplayName } });
				}
			}
			else if (IsLoggedIn)
			{
				try
				{
					IsBusy = true;

					Analytics.TrackEvent("Logout", new Dictionary<string, string> { { "displayName", identityService.DisplayName } });

					identityService.Logout();
					IsLoggedIn = false;
					LoginButtonText = loginText;
				}
				finally
				{
					IsBusy = false;
				}
			}
		}

		async Task ExecuteTakePhotoCommand()
		{
			if (IsBusy)
				return;

			if (IsNotLoggedIn)
				return;

			try
			{
				IsBusy = true;

				var actions = new List<string>();

				if (CrossMedia.Current.IsTakePhotoSupported && CrossMedia.Current.IsCameraAvailable)
					actions.Add("Take Photo");

				if (CrossMedia.Current.IsPickPhotoSupported)
					actions.Add("Pick Photo");

				var result = await Application.Current.MainPage.DisplayActionSheet("Take or Pick Photo", "Cancel", null, actions.ToArray());

				MediaFile mediaFile = null;
				if (result == "Take Photo")
				{
					var options = new StoreCameraMediaOptions
					{
						PhotoSize = PhotoSize.Medium,
						DefaultCamera = CameraDevice.Rear
					};

					mediaFile = await CrossMedia.Current.TakePhotoAsync(options);

					Analytics.TrackEvent("Take Photo", new Dictionary<string, string> { { "displayName", identityService.DisplayName } });
				}
				else if (result == "Pick Photo")
				{
					mediaFile = await CrossMedia.Current.PickPhotoAsync();

					Analytics.TrackEvent("Pick Photo", new Dictionary<string, string> { { "displayName", identityService.DisplayName } });
				}
				else
				{
					Analytics.TrackEvent("Bailed on photo", new Dictionary<string, string> { { "displayName", identityService.DisplayName } });
				}

				if (mediaFile == null)
					return;

				await UploadPhoto(mediaFile);
			}
			finally
			{
				IsBusy = false;
			}
		}

		async Task UploadPhoto(MediaFile mediaFile)
		{
			UploadProgress progressUpdater = new UploadProgress();
			Uri blobUri = null;
			Uri thumbnailUri = null;

			using (var mediaStream = mediaFile.GetStream())
			{
				if (!(mediaStream is FileStream fs))
					return;

				// Create a thumbnail to go along with it!
				using (var thumbStream = new MemoryStream(await GetThumbnailBytes(fs)))
				{
					var storageService = DependencyService.Get<IStorageService>();

					thumbnailUri = await storageService.UploadBlob(thumbStream, progressUpdater);

					mediaStream.Position = 0;

					blobUri = await storageService.UploadBlob(mediaStream, progressUpdater);

					if (blobUri == null || thumbnailUri == null)
					{
						await Application.Current.MainPage.DisplayAlert("Upload Error", "There was an error uploading your photo, please try again.", "OK");
						return;
					}
				}
			}

			await SavePhoto(blobUri.AbsoluteUri, thumbnailUri.AbsoluteUri);
		}

		async Task<byte[]> GetThumbnailBytes(FileStream fileStream)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				await fileStream.CopyToAsync(memoryStream);
				var imageBytes = memoryStream.ToArray();

				var imageResizer = DependencyService.Get<IImageResizer>();
				return imageResizer.ResizeImage(imageBytes);
			}
		}

		async Task SavePhoto(string blobUrl, string thumbnailUrl)
		{
			var photo = new Photo
			{
				Comments = new List<Comment>(),
				PhotoUrl = blobUrl,
				ThumbnailUrl = thumbnailUrl,
				Tags = new List<string>(),
				UploadDate = DateTime.UtcNow,
				UserId = authResult.UniqueId,
				UpVotes = 0,
				DownVotes = 0,
				DisplayName = identityService.DisplayName
			};

			await dataService.InsertPhoto(photo);

			NewPhotoAdded?.Invoke(this, new Uri(blobUrl));
		}

	}
}
