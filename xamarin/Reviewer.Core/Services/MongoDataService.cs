using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using MongoDB.Driver;
using PhotoTour.Services;

using Reviewer.SharedModels;
using MongoDB.Bson;
using System.Linq;
using Microsoft.Azure.KeyVault;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using System.Security.Cryptography;

namespace PhotoTour.Core
{
    public class MongoDataService : IDataService
    {
        IMongoCollection<Photo> photosCollection;

        string dbName = "PhotoTour";
        string collectionName = "Photos";

        async Task Init()
        {
            if (photosCollection != null)
                return;

            if (string.IsNullOrWhiteSpace(APIKeys.MongoConnectionString))
            {
                await GetConnectionString();
            }

            // APIKeys.Connection string is found in the portal under the "Connection String" blade
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(APIKeys.MongoConnectionString)
            );

            settings.SslSettings =
                new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            // Initialize the client
            var mongoClient = new MongoClient(settings);

            // This will create or get the database
            var db = mongoClient.GetDatabase(dbName);

            // This will create or get the collection
            photosCollection = db.GetCollection<Photo>(collectionName);
            photosCollection = photosCollection.WithReadPreference(new ReadPreference(ReadPreferenceMode.Nearest));
        }

        async Task GetConnectionString()
        {
            var keyService = DependencyService.Get<IKeyVaultService>();

            APIKeys.MongoConnectionString = await keyService.GetValueForKey(APIKeys.KeyVaultMongoKey);
        }

        public async Task AddComment(Photo photo, Comment comment)
        {
            if (photo.Comments == null)
                photo.Comments = new List<Comment>();

            photo.Comments.Add(comment);

            await ReplacePhoto(photo);
        }

        public async Task TagPhoto(Photo photo, string tag)
        {
            if (photo.Tags == null)
                photo.Tags = new List<string>();

            photo.Tags.Add(tag);

            await ReplacePhoto(photo);
        }

        public async Task DownVote(Photo photo)
        {
            photo.DownVotes += 1;

            await ReplacePhoto(photo);
        }

        public async Task<List<Photo>> GetAllPhotos()
        {
            await Init();

            try
            {
                var allPhotos = await photosCollection
                    .Find(new BsonDocument())
                    .ToListAsync();

                return allPhotos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<Photo>> GetLatestPhotos()
        {
            await Init();

            try
            {
                var oneDayAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
                var latest = photosCollection.AsQueryable()
                                             .Where(p => p.UploadDate > oneDayAgo)
                                             .OrderByDescending(p => p.UploadDate)
                                             .ToList();

                return latest;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<Photo> FindPhotoByUrl(string url)
        {
            await Init();

            try
            {
                var thePhoto = await photosCollection.Find(p => p.PhotoUrl == url).FirstOrDefaultAsync();

                return thePhoto;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<Photo>> GetMyPhotos(string userId)
        {
            await Init();

            try
            {
                var myPhotos = await photosCollection
                    .Find(p => p.UserId == userId)
                    .ToListAsync();

                return myPhotos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task UpVote(Photo photo)
        {
            photo.UpVotes += 1;

            await ReplacePhoto(photo);
        }

        async Task ReplacePhoto(Photo photo)
        {
            await Init();
            try
            {
                await photosCollection.ReplaceOneAsync(p => p._id.Equals(photo._id), photo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR: {ex.Message}");
            }
        }

        public async Task InsertPhoto(Photo photo)
        {
            await Init();

            try
            {
                await photosCollection.InsertOneAsync(photo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR: {ex.Message}");
            }
        }

        public async Task<long> TotalPhotos()
        {
            try
            {
                await Init();

                var bannedUsers = Settings.BlockedUsers;

                var count = photosCollection.AsQueryable()
                                            .Where(p => !bannedUsers.Contains(p.UserId))
                                            .ToList()
                                            .Count();

                //var query = from p in photosCollection.AsQueryable()
                //            where p.UserId != bannedUsers.First()
                //            select p;

                //var count = query.Count();

                //var count = await photosCollection.CountAsync<Photo>(p => !bannedUsers.Any(bu => bu.Equals(p.UserId)));

                return count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** {ex.Message}");
            }
            return 0;
        }

        public async Task<List<Photo>> GetPageOfPhotos(long pageToGet, long pageSize)
        {
            await Init();

            try
            {
                var numberToSkip = (pageToGet - 1) * pageSize;

                var bannedUsers = Settings.BlockedUsers;

                //var photoList = await photosCollection.Find<Photo>(p => true)
                //.Skip((int?)numberToSkip)
                //.Limit((int?)pageSize)
                //.ToListAsync();

                var photoQuery = photosCollection.AsQueryable()
                                                 .Where(p => !bannedUsers.Contains(p.UserId))
                                                 .OrderByDescending(p => p.UploadDate)
                                                 .Skip((int)numberToSkip)
                                                 .Take((int)pageSize)
                                                 .ToList();

                return photoQuery;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
