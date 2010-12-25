using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace FacebookBigProfile
{
	public partial class MainView : UIViewController
	{
		#region Constructors

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

		public MainView () : base("MainView", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			View.Frame = new RectangleF(0, 20, View.Frame.Width, View.Frame.Height);				
		}
		
		private UIImagePickerController picker;
		private UIImageView profilePictureView;
		private UIImage overlayImage;
		
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
		}
		
		#endregion
		
		public void LoadImage(UIImage image) {
			var frame = new RectangleF(0, 0, image.Size.Width, image.Size.Height);
			profilePictureView.Image = image;
			profilePictureView.Frame = frame;
			scrollView.ContentSize = frame.Size;
			scrollView.ContentInset = new UIEdgeInsets(frame.Height, frame.Width, frame.Height, frame.Width);			
		}
	}
}

