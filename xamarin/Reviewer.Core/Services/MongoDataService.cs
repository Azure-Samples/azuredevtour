using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using MongoDB.Driver;
using PhotoTour.Services;

using Reviewer.SharedModels;
using MongoDB.Bson;

namespace PhotoTour.Core
{
	public class MongoDataService : IDataService
	{
		IMongoCollection<Photo> photosCollection;

		string dbName = "PhotoTour";
		string collectionName = "Photos";

		void Init()
		{
			if (photosCollection != null)
				return;

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
			Init();

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

		public async Task<Photo> FindPhotoByUrl(string url)
		{
			Init();

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
			Init();

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
			Init();
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
			Init();

			try
			{
				await photosCollection.InsertOneAsync(photo);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"*** ERROR: {ex.Message}");
			}
		}
	}
}
