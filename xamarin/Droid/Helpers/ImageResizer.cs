using System;
using System.IO;
using Android.Graphics;
using PhotoTour.Core;
using PhotoTour.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(ImageResizer))]
namespace PhotoTour.Droid
{
	public class ImageResizer : IImageResizer
	{
		const int finalImageSize = 240;

		public byte[] ResizeImage(byte[] imageData)
		{
			// Load the bitmap
			Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

			float width = originalImage.Width;
			float height = originalImage.Height;

			if (width > height)
			{
				var ratio = height / width;

				width = finalImageSize;
				height = finalImageSize * ratio;
			}
			else
			{
				var ratio = width / height;

				height = finalImageSize;
				width = finalImageSize * ratio;
			}

			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
				return ms.ToArray();
			}
		}
	}
}
