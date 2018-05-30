using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading;
using System.Diagnostics;
using PhotoTour.Core;
using Microsoft.WindowsAzure.Storage;

namespace PhotoTour.Services
{
	public class StorageService : IStorageService
	{
		public async Task<Uri> UploadBlob(Stream blobContent, UploadProgress progressUpdater)
		{
			Uri blobAddress = null;

			try
			{
				var container = new CloudBlobContainer(new Uri(GetBlobContainerUriAndSAS()));

				var blockBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}.jpg");

				await blockBlob.UploadFromStreamAsync(blobContent, null, null, null, progressUpdater, new CancellationToken());

				blobAddress = blockBlob.Uri;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"*** ERROR: {ex.Message}");

				return null;
			}

			return blobAddress;
		}

		#region Helpers

		string GetBlobContainerUriAndSAS()
		{
			string connectionString = $"DefaultEndpointsProtocol=https;AccountName={APIKeys.StorageAccountName};AccountKey={APIKeys.StorageAccountKey}";
			var csa = CloudStorageAccount.Parse(connectionString);

			var blobClient = csa.CreateCloudBlobClient();

			var container = blobClient.GetContainerReference(APIKeys.PhotosContainerName);
			var sasConstraints = new SharedAccessBlobPolicy
			{
				SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1),
				SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5),
				Permissions = SharedAccessBlobPermissions.Write
			};

			var containerToken = container.GetSharedAccessSignature(sasConstraints);

			return $"{container.Uri}{containerToken}";
		}

		#endregion
	}
}
