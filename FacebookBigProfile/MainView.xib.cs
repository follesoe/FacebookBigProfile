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
			scrollView.ZoomScale = 1f;
			scrollView.Bounces = false;
			scrollView.BouncesZoom = false;			
			scrollView.IndicatorStyle = UIScrollViewIndicatorStyle.Black;

		 	LoadImage(UIImage.FromFile("ProfilePicture.jpg"));
						
			scrollView.ViewForZoomingInScrollView = (sender) => {
				return profilePictureView;	
			};					
			
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
		}
		
		public void LoadImage(UIImage image) {
			var frame = new RectangleF(0, 0, image.Size.Width, image.Size.Height);
			profilePictureView.Image = image;
			profilePictureView.Frame = frame;
			scrollView.ContentSize = frame.Size;
			scrollView.ContentInset = new UIEdgeInsets(frame.Height, frame.Width, frame.Height, frame.Width);			
		}
	}
}