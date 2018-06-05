using System;
using PhotoTour.Core;
using System.Threading.Tasks;
using PhotoTour.Services;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Identity.Client;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Analytics;
using Plugin.Connectivity;

namespace Reviewer.Core
{
    public class PhotoViewerViewModel : BaseViewModel
    {
        IDataService dataService;
        IIdentityService identityService;
        AuthenticationResult authenticationResult = null;

        string photoUrl;
        public string PhotoUrl { get => photoUrl; set => SetProperty(ref photoUrl, value); }

        bool isLoggedIn;
        public bool IsLoggedIn { get => isLoggedIn; set => SetProperty(ref isLoggedIn, value); }

        bool addingComment;
        public bool AddingComment { get => addingComment; set => SetProperty(ref addingComment, value); }

        ObservableCollection<Comment> observableComments;
        public ObservableCollection<Comment> ObservableComments { get => observableComments; set => SetProperty(ref observableComments, value); }

        string newComment;
        public string NewComment { get => newComment; set => SetProperty(ref newComment, value); }

        bool saveCommentEnabled;
        public bool SaveCommentEnabled { get => saveCommentEnabled; set => SetProperty(ref saveCommentEnabled, value); }

        bool cancelCommentEnabled;
        public bool CancelCommentEnabled { get => cancelCommentEnabled; set => SetProperty(ref cancelCommentEnabled, value); }

        public ICommand UpVoteCommand { get; }
        public ICommand DownVoteCommand { get; }
        public ICommand AddCommentCommand { get; }
        public ICommand CancelCommentCommand { get; }
        public ICommand SaveCommentCommand { get; }

        Photo photo;
        public Photo Photo { get => photo; set => SetProperty(ref photo, value); }

        public PhotoViewerViewModel(string url)
        {
            IsLoggedIn = false;
            PhotoUrl = url;
            AddingComment = false;

            SaveCommentEnabled = false;
            CancelCommentEnabled = false;

            dataService = DependencyService.Get<IDataService>();
            identityService = DependencyService.Get<IIdentityService>();

            UpVoteCommand = new Command(async () => await ExecuteUpVote());
            DownVoteCommand = new Command(async () => await ExecuteDownVote());
            AddCommentCommand = new Command(() =>
            {
                NewComment = "";
                AddingComment = true;
                SaveCommentEnabled = true;
                CancelCommentEnabled = true;
            });
            CancelCommentCommand = new Command(() =>
            {
                AddingComment = false;
                SaveCommentEnabled = false;
                CancelCommentEnabled = false;
            });
            SaveCommentCommand = new Command(async () => await ExecuteSaveComment());

            CheckLoginStatus();
            LoadPhotoInfo();
        }

        async void LoadPhotoInfo()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "Cannot Get Photo Details - No Internet", "OK");
                return;
            }

            Photo = await ((MongoDataService)dataService).FindPhotoByUrl(PhotoUrl);

            ObservableComments = new ObservableCollection<Comment>();
            foreach (var item in Photo.Comments)
            {
                ObservableComments.Add(item);
            }
        }

        async void CheckLoginStatus()
        {
            authenticationResult = await identityService.GetCachedSignInToken();

            IsLoggedIn = authenticationResult?.User != null;
        }

        async Task ExecuteUpVote()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (!CrossConnectivity.Current.IsConnected)
                {
                    await Application.Current.MainPage.DisplayAlert("No Internet", "Cannot Save Vote - No Internet", "OK");

                    return;
                }


                await dataService.UpVote(Photo);

                Analytics.TrackEvent("Up Vote", new Dictionary<string, string> { { "displayName", identityService.DisplayName } });
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteDownVote()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (!CrossConnectivity.Current.IsConnected)
                {
                    await Application.Current.MainPage.DisplayAlert("No Internet", "Cannot Save Vote - No Internet", "OK");

                    return;
                }

                await dataService.DownVote(Photo);

                Analytics.TrackEvent("Up Vote", new Dictionary<string, string> { { "displayName", identityService.DisplayName } });
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteSaveComment()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SaveCommentEnabled = false;
                CancelCommentEnabled = false;

                if (!CrossConnectivity.Current.IsConnected)
                {
                    await Application.Current.MainPage.DisplayAlert("No Internet", "Cannot Save Comment - No Internet", "OK");

                    return;
                }

                var comment = new Comment
                {
                    _id = Guid.NewGuid().ToString(),
                    Date = DateTime.UtcNow,
                    DisplayName = identityService.DisplayName,
                    Text = NewComment,
                    UserId = authenticationResult?.UniqueId
                };

                await dataService.AddComment(Photo, comment);

                Analytics.TrackEvent("Up Vote", new Dictionary<string, string> {
                    { "displayName", comment.DisplayName },
                    { "comment", comment.Text }
                });

                ObservableComments.Add(comment);
            }
            finally
            {
                IsBusy = false;
                AddingComment = false;
            }
        }

        public async Task ReportImage()
        {
            var message = new ReportImageMessage();
            MessagingCenter.Send(message, ReportImageMessage.Message);

            await Task.CompletedTask;

            return;
        }

        public void BlockUser()
        {
            var blocked = Settings.BlockedUsers;
            blocked.Add(Photo.UserId);

            Settings.BlockedUsers = blocked;

            var message = new BlockUserMessage();
            MessagingCenter.Send(message, BlockUserMessage.Message);
        }
    }
}
