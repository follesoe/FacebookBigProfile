using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using System.Web;
using Atomcraft;
using System.Threading;

namespace FacebookBigProfile
{
	public partial class SetProfilePictureView : UIViewController
	{
		private const string JavaScriptEvaluatorDomReadyScript = @"if (/loaded|complete/.test(document.readyState)){document.UIWebViewDocumentIsReady = true;} else {document.addEventListener('DOMContentLoaded', function(){document.UIWebViewDocumentIsReady = true;}, false);}";
		private const string JavaScriptEvaluatorDomLoadScript  = @"if (/loaded|complete/.test(document.readyState)){document.UIWebViewDocumentIsReady = true;} else {window.addEventListener('load',               function(){document.UIWebViewDocumentIsReady = true;}, false);}";
        private const string JavaScriptEvaluatorReadyStateCheckScript  = @"document.UIWebViewDocumentIsReady;";

		private string navigationTarget; 
		private bool hudVisible;
		private bool profileUpdated;
		private ATMHud hud;
		
		public SetProfilePictureView (IntPtr handle) : base(handle)
		{
			Initialize();
		}

		[Export("initWithCoder:")]
		public SetProfilePictureView (NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		private UINavigationController _parentNavigationController;

		public SetProfilePictureView (UINavigationController parentNavigationController) : base("SetProfilePictureView", null)
		{
			Initialize();
			_parentNavigationController = parentNavigationController;
		}

		void Initialize()
		{
		}
		
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetNavigationBarHidden(false, true);
			profileUpdated = false;
			base.ViewDidAppear (animated);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Set Profile Picture";		
			webView.LoadStarted += LoadStarted;
			webView.LoadFinished += LoadFinished;
			webView.ScalesPageToFit = true;
			
			hud = new ATMHud();		
			hud.Center = new System.Drawing.PointF(View.Frame.Width / 2f, 66f);
			View.AddSubview(hud.View);
		}
		
		public override void ViewWillDisappear (bool animated)
		{			
			profileUpdated = false;
			HideProgress();
			base.ViewWillDisappear (animated);
		}
		
		public void NavigateTo(string url)
		{		
			ShowProgress();
			Console.WriteLine("Load url: " + url);
			navigationTarget = url;
			webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
		}
		
		private void LoadStarted(object sender, EventArgs e)
		{
			ShowProgress();
			Console.WriteLine("LoadStarted: {0}", webView.Request.Url.AbsoluteString);
		}
					
		private void LoadFinished(object sender, EventArgs e)
		{
			Console.WriteLine("LoadFinished: {0}", webView.Request.Url.AbsoluteString);
			
			if(webView.Request.Url.AbsoluteString.StartsWith("http://www.facebook.com/profile"))
			{
				profileUpdated = true;
				HideProgress();
				_parentNavigationController.PopViewControllerAnimated(true);
				return;
			}
			
			if(webView.Request.Url.AbsoluteString.Contains("login"))
			{
				HideProgress();
				using(var alert = new UIAlertView("One last thing...", "You need to log in to set your profile picture", null, "OK", null))
				{
					alert.Show();
				}
			}
			
			if(webView.Request.Url.AbsoluteString.Equals(navigationTarget))
			{			
				
				var thread = new Thread(() => {
					Thread.Sleep(1500);
					InvokeOnMainThread(() => ClickMakeProfilePicture());
					Thread.Sleep(2500);
					BeginInvokeOnMainThread(() => ClickOkay());
				});
				
				thread.Start();
			}
		}
		
		private void ShowProgress()
		{
			if(!hudVisible && !profileUpdated)
			{
				hudVisible = true;
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				hud.Center = new System.Drawing.PointF(View.Frame.Width / 2f, 66f);
				hud.SetCaption("Setting profile picture...");			
				hud.SetActivity(true);
				hud.Show();				
				hud.Update();
				hud.HideAfter(50.0);
			}
		}
		
		private void HideProgress()
		{
			hudVisible = false;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			hud.SetActivity(false);
			hud.Hide();
		}
		
		private void ClickMakeProfilePicture()
		{			
			string html = HttpUtility.HtmlDecode(webView.EvaluateJavascript("document.documentElement.outerHTML"));
			int setPhotoScriptStartIndex = html.IndexOf("Bootloader.loadComponents(\"photo-to-profile\"");
			int setPhotoScriptEndIndex = html.IndexOf("return false;", setPhotoScriptStartIndex);			
			int length = setPhotoScriptEndIndex - setPhotoScriptStartIndex;
			
			if(setPhotoScriptStartIndex > 0 && setPhotoScriptEndIndex > 0)
			{
				string script = html.Substring(setPhotoScriptStartIndex, length);			
				webView.EvaluateJavascript(script);
			}
			else 
			{
				HideProgress();
			}
		}
		
		private void ClickOkay()
		{
			webView.EvaluateJavascript("document.getElementsByName('ok')[0].click();");
		}
	}
}

