using System;
using System.Drawing;
using CoreGraphics;
using PhotoTour.Core;
using UIKit;
using Xamarin.Forms;
using PhotoTour.iOS;

[assembly: Dependency(typeof(ImageResizer))]
namespace PhotoTour.iOS
{
	public class ImageResizer : IImageResizer
	{
		const int finalImageSize = 240;

		public byte[] ResizeImage(byte[] imageData)//, float width, float height)
		{
			UIImage originalImage = ImageFromByteArray(imageData);
			UIImageOrientation orientation = originalImage.Orientation;

			float width = originalImage.CGImage.Width;
			float height = originalImage.CGImage.Height;

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

			//create a 24bit RGB image
			using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
												 (int)width, (int)height, 8,
												 4 * (int)width, CGColorSpace.CreateDeviceRGB(),
												 CGImageAlphaInfo.PremultipliedFirst))
			{
				RectangleF imageRect = new RectangleF(0, 0, width, height);

				// draw the image
				context.DrawImage(imageRect, originalImage.CGImage);

				UIImage resizedImage = UIImage.FromImage(context.ToImage(), 0, orientation);

				// save the image as a jpeg
				return resizedImage.AsJPEG().ToArray();
			}
		}

		UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			UIImage image;
			try
			{
				image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}
	}
}
