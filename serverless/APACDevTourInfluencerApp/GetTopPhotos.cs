using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace APACDevTourInfluencerApp
{
    public static class GetTopPhotos
    {
        private static string host = Environment.GetEnvironmentVariable("Url");
        private static string databaseName = "PhotoTour";
        private static string username = Environment.GetEnvironmentVariable("user");
        private static string password = Environment.GetEnvironmentVariable("Password");
        private static MongoClient mongoClient = CreateMongoClient(host, databaseName, username, password);

        [FunctionName("GetTopPhotos")]
        public static async Task RunAsync([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, [Queue("roundup-queue", Connection = "ResultsQueue")]ICollector<string> outQueueItem, TraceWriter log)
        {
            var database = mongoClient.GetDatabase(databaseName);
            var photosCollection = database.GetCollection<object>("Photos");

            var projection = Builders<object>.Projection
           .Include("_id")
           .Include("displayName")
           .Include("upVotes")
           .Include("downVotes")
           .Include("photoUrl");

            var sort = Builders<object>.Sort.Descending("upVotes");
            var options = new FindOptions<object>
            {
                Sort = sort,
                Limit = 5,
                Projection = projection
            };

            var filterBuilder = Builders<object>.Filter;
            var filter = new BsonDocument();
            var photos = await photosCollection.FindAsync(filter, options);
            var jsonPhotos = JsonConvert.SerializeObject(photos.ToList());
            outQueueItem.Add(jsonPhotos);
        }

        private static MongoClient CreateMongoClient(string host, string databaseName, string username, string password)
        {
            MongoClientSettings settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, 10255),
                UseSsl = true,
                SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 },
                Credential = new MongoCredential(
                    "SCRAM-SHA-1",
                    new MongoInternalIdentity(databaseName, username),
                    new PasswordEvidence(password))
            };
            return new MongoClient(settings);
        }
    }
}
