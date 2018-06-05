using System;
using Reviewer.SharedModels;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace PhotoTour.Core
{
	[BsonIgnoreExtraElements]
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

		DateTime date;
		[BsonElement("date")]
		public DateTime Date { get => date; set => SetProperty(ref date, value); }

		string displayName;
		[BsonElement("displayName")]
		public string DisplayName { get => displayName; set => SetProperty(ref displayName, value); }
	}
}
