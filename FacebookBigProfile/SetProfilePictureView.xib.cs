using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace FacebookBigProfile
{
	public partial class SetProfilePictureView : UIViewController
	{
		public SetProfilePictureView (IntPtr handle) : base(handle)
		{
			Initialize();
		}

		[Export("initWithCoder:")]
		public SetProfilePictureView (NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public SetProfilePictureView () : base("SetProfilePictureView", null)
		{
			Initialize();
		}

		void Initialize()
		{
		}
		
		public override void ViewDidLoad ()
		{
			Title = "Set Profile Picture";		
			webView.LoadFinished += ScrollIntoView;
			base.ViewDidLoad ();
		}
		
		public void ScrollIntoView(object sender, EventArgs e)
		{
			string script = "window.scrollTo(330, 580);";						
			webView.EvaluateJavascript(script);		
		}

		public void NavigateTo(string url)
		{
			Console.WriteLine("Load url: " + url);
			webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
			webView.ContentScaleFactor = 1.5f;
			
			using(var alert = new UIAlertView("One final thing...", "Because of limitations in what apps can do on Facebook you need to click \"Make Profile Picture\" to complete your Big Profile", null, "OK", null))
			{
				alert.Show();
			}
		}
		
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetNavigationBarHidden(false, true);
			base.ViewDidAppear (animated);
		}
	}
}

