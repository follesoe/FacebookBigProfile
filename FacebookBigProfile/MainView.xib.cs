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
			Initialize (null);
		}

		[Export("initWithCoder:")]
		public MainView (NSCoder coder) : base(coder)
		{
			Initialize (null);
		}

		public MainView (Facebook facebook) : base("MainView", null)
		{
			_facebook = facebook;
			Initialize (null);
		}
				
		private Facebook _facebook;
		private FacebookController facebookController;
		private UIImagePickerController picker;
		private UIImageView profilePictureView;
		private UIImage overlayImage;
		private UIImage profilePicture;
		
		private void Initialize (Facebook facebook)
		{
			facebookController = new FacebookController(_facebook);
			View.Frame = new RectangleF(0, 20, View.Frame.Width, View.Frame.Height);				
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
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}
		
		public void LoadImage(UIImage image) {
			profilePicture = image;
			float zoomScale = GetZoomScale(profilePicture.Size, scrollView.Frame.Size);	
			var frame = new RectangleF(0f, 0f, image.Size.Width * zoomScale, image.Size.Height * zoomScale);
			var size = scrollView.Frame.Size;
			
			profilePictureView.Frame = frame;
			profilePictureView.Image = image;			
			scrollView.ContentSize = frame.Size;
			scrollView.ContentInset = new UIEdgeInsets(size.Height * 0.8f, size.Width * 0.8f, size.Height * 0.8f, size.Width * 0.8f);
			scrollView.ContentOffset = new PointF(0, 0);
			scrollView.ZoomScale = 1.0f;
		}
		
		private float GetZoomScale(SizeF originalSize, SizeF targetSize) {
			return targetSize.Width / originalSize.Width;			
		}		
		
		public void SplitImage() {
			Console.WriteLine("Split the image...");
			
			Console.WriteLine("Picture Size:\t" + profilePicture.Size);
			Console.WriteLine("Picture View Size:\t" + profilePictureView.Frame.Size);
			Console.WriteLine("Scroll Offset:\t" + scrollView.ContentOffset);
			
	
			float zoomScale = GetZoomScale(profilePicture.Size, scrollView.Frame.Size);	
			Console.WriteLine("ZoomScale:\t" + scrollView.ZoomScale);
			Console.WriteLine("Calculated ZoomScale:\t" + zoomScale);
			Console.WriteLine("Actuall ZoomScale:\t" + scrollView.ZoomScale * zoomScale);
			//var frame6 = new RectangleF(8f, 55f, 163f, 486f);
		}
	}
}






