using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using System.Web;

namespace FacebookBigProfile
{
	public partial class SetProfilePictureView : UIViewController
	{
		private const string JavaScriptEvaluatorDomReadyScript = @"if (/loaded|complete/.test(document.readyState)){document.UIWebViewDocumentIsReady = true;} else {document.addEventListener('DOMContentLoaded', function(){document.UIWebViewDocumentIsReady = true;}, false);}";
		private const string JavaScriptEvaluatorDomLoadScript  = @"if (/loaded|complete/.test(document.readyState)){document.UIWebViewDocumentIsReady = true;} else {window.addEventListener('load',               function(){document.UIWebViewDocumentIsReady = true;}, false);}";
        private const string JavaScriptEvaluatorReadyStateCheckScript  = @"document.UIWebViewDocumentIsReady;";

		private string navigationTarget; 
		
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
		
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetNavigationBarHidden(false, true);
			base.ViewDidAppear (animated);
		}
		
		public override void ViewDidLoad ()
		{
			Title = "Set Profile Picture";		
			webView.LoadFinished += LoadFinished;
			base.ViewDidLoad ();
		}
		
		public void NavigateTo(string url)
		{
			Console.WriteLine("Load url: " + url);
			navigationTarget = url;
			webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
			webView.ContentScaleFactor = 1.5f;
			
			using(var alert = new UIAlertView("One more thing...", "You need to click \"Make Profile Picture\" to complete your Big Profile", null, "OK", null))
			{
				alert.Show();
			}
		}
		
		private void LoadFinished(object sender, EventArgs e)
		{
			Console.WriteLine("LoadFinished: {0}", webView.Request.Url.AbsoluteString);
			
			if(webView.Request.Url.AbsoluteString.Equals(navigationTarget))
			{
				
				ClickMakeProfilePicture();
				ScrollIntoView();
			}
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
		}
		
		private void ScrollIntoView()
		{
			Console.WriteLine("ScrollIntoView");
			string script = "window.scrollTo(240, 580);";			
			webView.EvaluateJavascript(script);		
		}
	}
}

