using System;
using System.Drawing;

namespace FacebookBigProfile
{
	public class CropHelpers
	{
		public static float GetZoomScale(SizeF originalSize, SizeF targetSize) 
		{
			return targetSize.Width / originalSize.Width;			
		}	
		
		public static RectangleF CalculateScaledCropSource(SizeF pictureSize, RectangleF cropSource, PointF scrollOffset, float zoomScale) 
		{
			if(zoomScale == 0f) throw new DivideByZeroException("You can not scale something to 0.");
			
			return new RectangleF((cropSource.X + scrollOffset.X) / zoomScale, 
			                      (cropSource.Y + scrollOffset.Y) / zoomScale,
			                      cropSource.Width / zoomScale,
			                      cropSource.Height / zoomScale);			
		}
	}
}

