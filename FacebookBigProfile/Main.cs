
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
			
			var button = UIButton.FromType(UIButtonType.RoundedRect);
			var frame = new RectangleF(35f, 30f, 100f, 100f);
			
			button.Frame = frame;
			button.SetTitle("My coded button", UIControlState.Normal);
			
			button.TouchUpInside += (sender, e) => {
				button.SetTitle("Clicked!", UIControlState.Normal);
			};
			
			window.AddSubview(button);
			
			return true;
		}
		
		partial void buttonPressed (UIButton sender)
		{
			myButton.SetTitle("Clicked!", UIControlState.Normal);
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

