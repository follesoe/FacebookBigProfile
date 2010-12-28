using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

using FacebookSdk;

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
		const string kAppId = "180991745253598"; //"188864954461169";
		private Facebook _facebook;
		
		private MainView mainView;
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{			
			_facebook = new FacebookSdk.Facebook(kAppId);
			
			window.MakeKeyAndVisible();	
					
			mainView = new MainView(_facebook);
			window.AddSubview(mainView.View);
			
			return true;
		}
		
		public override void HandleOpenURL (UIApplication application, NSUrl url)
		{
			_facebook.HandleOpenUrl(url);
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}