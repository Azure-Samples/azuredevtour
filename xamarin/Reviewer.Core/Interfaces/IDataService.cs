using System;
using Reviewer.SharedModels;
using System.Collections.Generic;
using System.Threading.Tasks;

using PhotoTour.Core;

namespace PhotoTour.Services
{
	public interface IDataService
	{
		Task<List<Photo>> GetAllPhotos();

		Task<List<Photo>> GetMyPhotos(string userId);

		Task<Photo> FindPhotoByUrl(string url);

		Task UpVote(Photo photo);

		Task DownVote(Photo photo);

		Task AddComment(Photo photo, Comment comment);

		Task InsertPhoto(Photo photo);

		Task TagPhoto(Photo photo, string tag);
	}
}
