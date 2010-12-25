using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace FacebookBigProfile
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			
			window.MakeKeyAndVisible ();
			
			UIImage image = UIImage.FromFile("ProfilePicture.jpg");
			UIImageView imageView = new UIImageView(image);
			
						
			scrollView.AddSubview(imageView);
			scrollView.ContentSize = imageView.Frame.Size;
			scrollView.ContentInset = new UIEdgeInsets(imageView.Frame.Height, imageView.Frame.Width, imageView.Frame.Bottom, imageView.Frame.Height);
			scrollView.MaximumZoomScale = 5f;
			scrollView.MinimumZoomScale = 0.0f;
			scrollView.Bounces = false;
			scrollView.BouncesZoom = false;
			
			scrollView.IndicatorStyle = UIScrollViewIndicatorStyle.Black;
			scrollView.ViewForZoomingInScrollView = (sender) => {
				return imageView;	
			};
			
			scrollView.ZoomScale = 1f;
			
			UIImage overlayImage = UIImage.FromFile("FacebookOverlay.png");
			facebookOverlay.Image = overlayImage;
			
			return true;
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

