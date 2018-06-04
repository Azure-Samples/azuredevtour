using System;
using Reviewer.SharedModels;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace PhotoTour.Core
{
	public class Comment : ObservableObject
	{
		[BsonElement("_id")]
		public string _id { get; set; }

		string userId;
		[BsonElement("userId")]
		public string UserId { get => userId; set => SetProperty(ref userId, value); }

		string text;
		[BsonElement("text")]
		public string Text { get => text; set => SetProperty(ref text, value); }

	    string comment;
	    [BsonElement("comment")]
	    public string CommentText { get => comment; set => SetProperty(ref comment, value); }

        DateTime date;
		[BsonElement("date")]
		public DateTime Date { get => date; set => SetProperty(ref date, value); }

        DateTime time;
        [BsonElement("time")]
        public DateTime Time { get => time; set => SetProperty(ref time, value); }


        string displayName;
		[BsonElement("displayName")]
		public string DisplayName { get => displayName; set => SetProperty(ref displayName, value); }
	}
}
