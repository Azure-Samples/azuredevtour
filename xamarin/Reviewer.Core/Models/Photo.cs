using System;
using Reviewer.SharedModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;
using System.Runtime.CompilerServices;

namespace PhotoTour.Core
{
	public class Photo : ObservableObject
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }

		string photoUrl;
		[BsonElement("photoUrl")]
		public string PhotoUrl { get => photoUrl; set => SetProperty(ref photoUrl, value); }

		string thumbnailUrl;
		[BsonElement("thumbnailUrl")]
		public string ThumbnailUrl { get => thumbnailUrl; set => SetProperty(ref thumbnailUrl, value); }

		string userId;
		[BsonElement("userId")]
		public string UserId { get => userId; set => SetProperty(ref userId, value); }

		List<Comment> comments;
		[BsonElement("comments")]
		public List<Comment> Comments { get => comments; set => SetProperty(ref comments, value); }

	    //Comment comment;
	    //[BsonElement("comment")]
	    //public Comment Comment { get => comment; set => SetProperty(ref comment, value); }


        int upVotes;
		[BsonElement("upVotes")]
		public int UpVotes { get => upVotes; set => SetProperty(ref upVotes, value); }

		int downVotes;
		[BsonElement("downVotes")]
		public int DownVotes { get => downVotes; set => SetProperty(ref downVotes, value); }

		DateTime uploadDate;
		[BsonElement("uploadDate")]
		public DateTime UploadDate { get => uploadDate; set => SetProperty(ref uploadDate, value); }

		List<string> tags;
		[BsonElement("tags")]
		public List<string> Tags { get => tags; set => SetProperty(ref tags, value); }

		string displayName;
		[BsonElement("displayName")]
		public string DisplayName { get => displayName; set => SetProperty(ref displayName, value); }

	    string caption;
	    [BsonElement("caption")]
	    public string Caption { get => caption; set => SetProperty(ref caption, value); }


        [BsonElement("votes")]
        [BsonRepresentation(BsonType.Int64, AllowTruncation = true)]
		public int Votes { get; set; }
	}
}
