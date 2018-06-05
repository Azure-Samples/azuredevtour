using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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
            DependencyService.Register<IKeyVaultService, KeyVaultService>();

            MainPage = new NavigationPage(new PhotoListPage());
        }

        protected async override void OnStart()
        {
            base.OnStart();

            AppCenter.Start($"ios={APIKeys.AppCenterIOSKey};" +
                            $"android={APIKeys.AppCenterDroidKey};",
                            typeof(Analytics), typeof(Crashes));

            Analytics.SetEnabledAsync(true);
            Crashes.SetEnabledAsync(true);

            // Fire off a bunch of key vault gets
            var keyVaultService = DependencyService.Get<IKeyVaultService>();
            APIKeys.PhotosContainerName = string.IsNullOrWhiteSpace(APIKeys.PhotosContainerName) ? await keyVaultService.GetValueForKey(APIKeys.KeyValuePhotosContainerKey) : APIKeys.PhotosContainerName;
            APIKeys.StorageAccountKey = string.IsNullOrEmpty(APIKeys.StorageAccountKey) ? await keyVaultService.GetValueForKey(APIKeys.KeyVaultStorageAcctKeyKey) : APIKeys.StorageAccountKey;
            APIKeys.StorageAccountName = string.IsNullOrEmpty(APIKeys.StorageAccountName) ? await keyVaultService.GetValueForKey(APIKeys.KeyVaultStorageNameKey) : APIKeys.StorageAccountName;
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
