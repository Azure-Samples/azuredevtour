using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using MongoDB.Bson;
using MongoDB.Driver;
using static Newtonsoft.Json.JsonConvert;

namespace APACDevTourInfluencerApp
{
    public static class GetTopPhotos
    {
        private static string host = Environment.GetEnvironmentVariable("MongoHostname");
        private static string databaseName = "PhotoTour";
        private static string username = Environment.GetEnvironmentVariable("MongoUsername");
        private static string password = Environment.GetEnvironmentVariable("MongoPassword");
        private static MongoClient mongoClient = CreateMongoClient(host, databaseName, username, password);

        [FunctionName("GetTopPhotos")]
        public static async Task RunAsync(
            [TimerTrigger("0 0 0 * * *", RunOnStartup = true)]TimerInfo myTimer, 
            [Queue("roundup-queue", Connection = "ResultsQueue")]IAsyncCollector<string> outQueueItem, 
            TraceWriter log)
        {
            var database = mongoClient.GetDatabase(databaseName);
            var photosCollection = database.GetCollection<Photo>("Photos");

            var oneDayAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));

            var topPhotosToday = photosCollection.AsQueryable()
                .Where(p => p.uploadDate >= oneDayAgo)
                .Select(p => new
                {
                    p.displayName,
                    p.uploadDate,
                    p.upVotes,
                    p.downVotes,
                    p.photoUrl
                })
                .OrderByDescending(p => p.upVotes)
                .Take(5)
                .ToList();

            await outQueueItem.AddAsync(SerializeObject(topPhotosToday));
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
