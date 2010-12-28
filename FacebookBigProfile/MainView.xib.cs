using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

using FacebookSdk;

namespace FacebookBigProfile
{
	public partial class MainView : UIViewController
	{
		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public MainView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public MainView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MainView (Facebook facebook) : base("MainView", null)
		{
			this.facebook = facebook;
			Initialize ();
		}
				
		private Facebook facebook;
		private FacebookController facebookController;
		private UIImagePickerController picker;
		private UIImageView profilePictureView;
		private UIImage overlayImage;
		private UIImage profilePicture;
		
		private UIImageView cropSource1;
		private UIImageView cropSource2;
		private UIImageView cropSource3;
		private UIImageView cropSource4;
		private UIImageView cropSource5;
		private UIImageView cropSource6;
		
		private SizeF profilePictureSize;
		private SizeF profilePictureSmallSize;
		
		private void Initialize ()
		{
			View.Frame = new RectangleF(0, 20, View.Frame.Width, View.Frame.Height);
			facebookController = new FacebookController(facebook);		
			profilePictureSize = new SizeF(180f, 540f);
			profilePictureSmallSize = new SizeF(97f, 68f);
		}
		
		private UIImageView CreateCropSource(float x, float y, float width, float height) 
		{
			var imageView = new UIImageView();
			imageView.BackgroundColor = new UIColor(255, 0, 0, 0.5f);
			imageView.Frame = new RectangleF(x, y, width, height);
			return imageView;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
					
			profilePictureView = new UIImageView();
									
			scrollView.AddSubview(profilePictureView);
			scrollView.MaximumZoomScale = 5f;
			scrollView.MinimumZoomScale = 0.2f;
			scrollView.Bounces = false;
			scrollView.BouncesZoom = false;			
			scrollView.IndicatorStyle = UIScrollViewIndicatorStyle.Black;
					 				
			scrollView.ViewForZoomingInScrollView = (sender) => {
				return profilePictureView;	
			};		
			
			LoadImage(UIImage.FromFile("ProfilePicture.jpg"));
			
			overlayImage = UIImage.FromFile("FacebookOverlay.png");
			facebookOverlay.Image = overlayImage;	
			
			picker = new UIImagePickerController();
			picker.Delegate = new ImagePickerDelegate(this);
			
			libraryButton.Clicked += (o, e) => {
				picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
				PresentModalViewController(picker, true);								
			};
			
			cameraButton.Clicked += (o, e) => {
				var cameraType = UIImagePickerControllerSourceType.Camera;
				if(UIImagePickerController.IsSourceTypeAvailable(cameraType)) {
					picker.SourceType = cameraType;
					PresentModalViewController(picker, true);
				}
			};
			
			facebookButton.Clicked += (o, e) => {
				facebookController.Login();
			};
			
			splitButton.Clicked += (o, e) => {
				SplitImage();
			};
			
			AddCropHelpers();
		}	
		
		private void AddCropHelpers() 
		{
			cropSource6 = CreateCropSource(5, 24, 80, 222);
			cropSource5 = CreateCropSource(95, 41, 43, 28);
			cropSource4 = CreateCropSource(139, 41, 43, 28);			
			cropSource3 = CreateCropSource(184, 41, 43, 28);
			cropSource2 = CreateCropSource(228, 41, 43, 28);
			cropSource1 = CreateCropSource(273, 41, 43, 28);
			
			View.AddSubview(cropSource1);
			View.AddSubview(cropSource2);
			View.AddSubview(cropSource3);
			View.AddSubview(cropSource4);
			View.AddSubview(cropSource5);
			View.AddSubview(cropSource6);
		}
		
		
		public void LoadImage(UIImage image) 
		{
			profilePicture = image;
			float zoomScale = CropHelpers.GetZoomScale(profilePicture.Size, scrollView.Frame.Size);	
			var frame = new RectangleF(0f, 0f, image.Size.Width * zoomScale, image.Size.Height * zoomScale);
			var size = scrollView.Frame.Size;
			
			profilePictureView.Frame = frame;
			profilePictureView.Image = image;			
			scrollView.ContentSize = frame.Size;
			scrollView.ContentInset = new UIEdgeInsets(size.Height * 0.8f, size.Width * 0.8f, size.Height * 0.8f, size.Width * 0.8f);
			scrollView.ContentOffset = new PointF(0, 0);
			scrollView.ZoomScale = 1.0f;
		}			
		
		public void SplitImage() 
		{
			/*
			Console.WriteLine();
			Console.WriteLine("Split the image...");
			Console.WriteLine();
			Console.WriteLine("Picture Size:\t\t\t" + profilePicture.Size);			
			Console.WriteLine("Picture View Size:\t\t" + profilePictureView.Frame.Size);
			Console.WriteLine("Scroll Offset:\t\t\t" + scrollView.ContentOffset);
			*/
	
			float zoomScale = CropHelpers.GetZoomScale(profilePicture.Size, scrollView.Frame.Size);	
			float currentZoomScale = scrollView.ZoomScale * zoomScale;
			
			/*
			Console.WriteLine("ScrollView ZoomScale:\t" + scrollView.ZoomScale);
			Console.WriteLine("Calculated ZoomScale:\t" + zoomScale);
			Console.WriteLine("Actuall ZoomScale:\t\t" + currentZoomScale);
			*/
			
			/*
			var imageCrop1 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource1.Frame, scrollView.ContentOffset, currentZoomScale); 
			var imageCrop2 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource2.Frame, scrollView.ContentOffset, currentZoomScale); 
			var imageCrop3 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource3.Frame, scrollView.ContentOffset, currentZoomScale); 
			var imageCrop4 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource4.Frame, scrollView.ContentOffset, currentZoomScale); 
			var imageCrop5 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource5.Frame, scrollView.ContentOffset, currentZoomScale); 			
			*/
			var imageCrop6 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource6.Frame, scrollView.ContentOffset, currentZoomScale); 

			/*
			Console.WriteLine();
			Console.WriteLine("Image 1 crop: " + imageCrop1);
			Console.WriteLine("Image 2 crop: " + imageCrop2);
			Console.WriteLine("Image 3 crop: " + imageCrop3);
			Console.WriteLine("Image 4 crop: " + imageCrop4);
			Console.WriteLine("Image 5 crop: " + imageCrop5);
			Console.WriteLine("Image 6 crop: " + imageCrop6);
			*/
				
			/*
			var cropped1 = Crop(profilePicture, imageCrop1).Scale(profilePictureSmallSize);
			var cropped2 = Crop(profilePicture, imageCrop2).Scale(profilePictureSmallSize);
			var cropped3 = Crop(profilePicture, imageCrop3).Scale(profilePictureSmallSize);
			var cropped4 = Crop(profilePicture, imageCrop4).Scale(profilePictureSmallSize);
			var cropped5 = Crop(profilePicture, imageCrop5).Scale(profilePictureSmallSize);
			*/
			var cropped6 = Crop(profilePicture, imageCrop6).Scale(profilePictureSize);
			facebookController.UploadImage(cropped6, "Part 6 of my Big Profile Picture.");
			
			/*
			cropped6.SaveToPhotosAlbum(delegate(UIImage image, NSError error) {
				Console.WriteLine("Saved to album!");
			});*/
			
		}
		
		public UIImage Crop(UIImage image, RectangleF section)
	    {			
			UIGraphics.BeginImageContext(section.Size);			
			var context = UIGraphics.GetCurrentContext();
			
			context.ClipToRect(new RectangleF(0, 0, section.Width + 1, section.Height + 1));
		
			var transform = new MonoTouch.CoreGraphics.CGAffineTransform(1, 0, 0, -1, 0, section.Height);
			context.ConcatCTM(transform);			
			
			var drawRectangle = new RectangleF(-section.X, -section.Y, image.Size.Width, image.Size.Height);
			context.DrawImage(drawRectangle, image.CGImage);			
			
			var croppedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return croppedImage;
	    }
	}
}






