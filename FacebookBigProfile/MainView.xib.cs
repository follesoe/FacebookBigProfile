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
		
		private UIImageView profilePictureView;
		private UIImage profilePicture;
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Console.WriteLine("ViewDidLoad");
			
			profilePicture = UIImage.FromFile("ProfilePicture.jpg");
		 	profilePictureView = new UIImageView(profilePicture);
			var frame = profilePictureView.Frame;
									
			scrollView.AddSubview(profilePictureView);
			scrollView.ContentSize = profilePictureView.Frame.Size;
			scrollView.ContentInset = new UIEdgeInsets(frame.Height, frame.Width, frame.Bottom, frame.Height);
			scrollView.MaximumZoomScale = 5f;
			scrollView.MinimumZoomScale = 0.0f;
			scrollView.Bounces = false;
			scrollView.BouncesZoom = false;
			
			scrollView.IndicatorStyle = UIScrollViewIndicatorStyle.Black;
			
			
			
			scrollView.ViewForZoomingInScrollView = (sender) => {
				Console.WriteLine("ViewForZoomingInScrollView"); 
				return profilePictureView;	
			};
			
			
			scrollView.ZoomScale = 1f;
			
			UIImage overlayImage = UIImage.FromFile("FacebookOverlay.png");
			facebookOverlay.Image = overlayImage;		
		}
		
		#endregion
	}
}

