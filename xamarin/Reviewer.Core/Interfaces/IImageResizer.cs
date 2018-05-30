using System;
using System.Threading.Tasks;

namespace PhotoTour.Core
{
	public interface IImageResizer
	{
		byte[] ResizeImage(byte[] imageData);//, float width, float height);
	}
}
