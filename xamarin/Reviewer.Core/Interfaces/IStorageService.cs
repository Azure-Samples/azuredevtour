using System;
using System.IO;
using System.Threading.Tasks;

namespace PhotoTour.Core
{
	public interface IStorageService
	{
		Task<Uri> UploadBlob(Stream blobContent, UploadProgress progressUpdater);
	}
}
