using System;
using System.Drawing;
using NUnit.Framework;
using FacebookBigProfile;

namespace FacebookBigProfileTests
{
	[TestFixture]
	public class Image_slicing_tests
	{
		[Test]
		public void Can_cut_an_image_which_is_scaled_down()
		{
			scrollOffset = new PointF(0, 0);
			pictureSize = new SizeF(473, 500);
			zoomScale = 0.6765327f;
			
			cropSource = new RectangleF(10, 10, 100, 150);			
			expectedCrop = new RectangleF(14.7812515196974f, 14.7812515196974f, 147.812515196974f, 221.718772795461f);			
			actualCrop = CropHelpers.CalculateScaledCropSource(pictureSize, cropSource, scrollOffset, zoomScale);
			
			Assert_Rectangle(expectedCrop, actualCrop, 0.01f);
		}
		
		[Test]
		public void Can_cut_an_image_which_have_parts_scrolled_off_screen()
		{
			pictureSize = new SizeF(473, 500);
			scrollOffset = new PointF(15, 15);
			zoomScale = 0.6765327f;
			
			cropSource = new RectangleF(10, 10, 100, 150);			
			expectedCrop = new RectangleF(36.9531287992436f, 36.9531287992436f, 147.812515196974f, 221.718772795461f);			
			actualCrop = CropHelpers.CalculateScaledCropSource(pictureSize, cropSource, scrollOffset, zoomScale);
			
			Assert_Rectangle(expectedCrop, actualCrop, 0.01f);
		}
		
		[Test]
		public void Can_cut_an_image_which_is_scaled_up() 
		{
			scrollOffset = new PointF(0, 0);
			pictureSize = new SizeF(266, 189);
			zoomScale = 1.203007f;
			
			cropSource = new RectangleF(10, 10, 100, 150);			
			expectedCrop = new RectangleF(8.31250358476717f, 8.31250358476717f, 83.1250358476717f, 124.687553771508f);			
			actualCrop = CropHelpers.CalculateScaledCropSource(pictureSize, cropSource, scrollOffset, zoomScale);
			
			Assert_Rectangle(expectedCrop, actualCrop, 0.01f);
		}
		
		[Test]
		public void Do_not_cut_outside_image_if_image_isnt_wide_enough()
		{
			scrollOffset = new PointF(20, 0);
			pictureSize = new SizeF(640, 480);
			cropSource = new RectangleF(580, 0, 60, 60);
			expectedCrop = new RectangleF(600, 0, 40, 60);
			actualCrop = CropHelpers.CalculateScaledCropSource(pictureSize, cropSource, scrollOffset, 1.0f);
			
			Assert_Rectangle(expectedCrop, actualCrop, 0.1f);
		}
		
		[Test, ExpectedException(typeof(DivideByZeroException))]
		public void Throws_an_exception_if_zoom_scale_is_0()
		{
			scrollOffset = new PointF(0, 0);
			pictureSize = new SizeF(1, 1);			
			cropSource = new RectangleF(1, 1, 1, 1);			
			expectedCrop = new RectangleF(1, 1, 1, 1);			
			actualCrop = CropHelpers.CalculateScaledCropSource(pictureSize, cropSource, scrollOffset, 0.0f);		
		}	
		
		private void Assert_Rectangle(RectangleF expected, RectangleF actual, float delta) 
		{
			Assert.AreEqual(expected.X, actual.X, delta);
			Assert.AreEqual(expected.Y, actual.Y, delta);
			Assert.AreEqual(expected.Width, actual.Width, delta);
			Assert.AreEqual(expected.Height, actual.Height, delta);
		}
		
		[TestFixtureSetUp]
		public void Setup() 
		{

		}
				
		private RectangleF cropSource;
		private RectangleF expectedCrop;
		private RectangleF actualCrop;
		private SizeF pictureSize;
		private PointF scrollOffset;
		private float zoomScale;
	}
}