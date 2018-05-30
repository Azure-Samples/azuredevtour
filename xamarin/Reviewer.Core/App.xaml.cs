using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using Reviewer.Core;
using PhotoTour.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PhotoTour.Core
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MonkeyCache.FileStore.Barrel.ApplicationId = "phototour";

			DependencyService.Register<IStorageService, StorageService>();
			DependencyService.Register<IDataService, MongoDataService>();


			MainPage = new NavigationPage(new PhotoListPage());
		}

		protected override void OnStart()
		{
			base.OnStart();

			AppCenter.Start($"ios={APIKeys.AppCenterIOSKey};" +
							$"android={APIKeys.AppCenterDroidKey};",
							typeof(Analytics), typeof(Crashes));

			Analytics.SetEnabledAsync(true);
		}

		protected override void OnSleep()
		{
			base.OnSleep();
		}

		protected override void OnResume()
		{
			base.OnResume();
		}
	}
}
