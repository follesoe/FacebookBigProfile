using System;
using FacebookSdk;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Json;
using System.Collections.Generic;

namespace FacebookBigProfile
{
	public interface IFacebookErrorProvider
	{
		void ErrorOccurred(NSError error);	
	}
	
	public class FacebookController : IFacebookErrorProvider
	{
		public class QueuedUpload 
		{
			public UIImage Image { get; set; }
			public string Message { get; set; }
			public bool AutoTag { get; set; }
		}		
		
		private readonly Facebook _facebook;
		private readonly FacebookLoginController _loginController;
		private readonly MainView _mainView;
		private readonly UploadPhotoRequestDelegate _photoDelegate;
		private readonly FqlRequestDelegate _fqlDelegate;
		private readonly UploadNextRequestDelegate _uploadNextDelegate;
		
		private const string ProgresString = "Uploading image {0} of 6 of your Big Profile...";
		
		private Queue<QueuedUpload> _queuedUploads;			
		
		public FacebookController(Facebook facebook, FacebookLoginController loginController, MainView mainView)
		{
			_facebook = facebook;
			_mainView = mainView;
			_loginController = loginController;
			_photoDelegate = new UploadPhotoRequestDelegate(this);
			_fqlDelegate = new FqlRequestDelegate(this);
			_uploadNextDelegate = new UploadNextRequestDelegate(this);
			_queuedUploads = new Queue<QueuedUpload>();
		}				
		
		public void ErrorOccurred(NSError error)
		{
			_mainView.StopProgress();
			_mainView.ShowError(error);
		}
		
		public void QueueForUpload(UIImage image, string message, bool autoTag) 
		{
			_queuedUploads.Enqueue(new QueuedUpload { Image = image, Message = message, AutoTag = autoTag });			
		}
		
		public void StartUpload()
		{
			if(_queuedUploads.Count > 0)
			{
				_mainView.UpdateProgress(string.Format(ProgresString, (6 - _queuedUploads.Count) + 1));
				var upload = _queuedUploads.Dequeue();
				UploadPhoto(upload.Image, upload.Message, upload.AutoTag);				
			}
			else 
			{
				_mainView.StopProgress();
			}
		}
		
		private void UploadPhoto(UIImage image, string message, bool autoTag) 
		{			
			if(!_loginController.IsLoggedIn) throw new Exception("User not logged in.");	
			if(string.IsNullOrEmpty(_loginController.UserId)) throw new Exception("Logged in but missing user id.");
		
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("picture"), image);
			parameters.Add(new NSString("message"), new NSString(message));
			
			_photoDelegate.AutoTag = autoTag;
			_facebook.RequestWithGraphPath("/me/photos", parameters, "POST", _photoDelegate);			
		}		
		
		public void GetPIDforPhotoFBID(string fbid, bool isWallPhoto)
		{			
			string fql = "SELECT pid FROM photo WHERE object_id = " + fbid;
			
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("query"), new NSString(fql));	
			
			_fqlDelegate.IsWallPhoto = isWallPhoto;
			if(isWallPhoto)
			{
				_facebook.RequestWithMethodName("fql.query", parameters, "POST", _fqlDelegate);	
			}
			else 
			{
				SetProfilePicture(fbid);
			}
		}
		
		public void TagPhoto(string photoPid)
		{
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("pid"), new NSString(photoPid));
			parameters.Add(new NSString("tag_uid"), new NSString(_loginController.UserId));
			parameters.Add(new NSString("x"), new NSString("10.0"));
			parameters.Add(new NSString("y"), new NSString("10.0"));
			_facebook.RequestWithMethodName("photos.addTag", parameters, "POST", _uploadNextDelegate);	
		}
		
		public void SetProfilePicture(string fbid)
		{
			string profilePictureUrl = string.Format("http://www.facebook.com/photo.php?fbid={0}&m2w", fbid);
			_mainView.StopProgress();
			_mainView.SetProfilePicture(profilePictureUrl);
			
			/*
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("owner_id"), new NSString(_userId));
			parameters.Add(new NSString("photo_id"), new NSString(photoPid));
			parameters.Add(new NSString("x"), new NSString("0"));
			parameters.Add(new NSString("y"), new NSString("0"));
			parameters.Add(new NSString("height"), new NSString("540"));
			parameters.Add(new NSString("width"), new NSString("180"));
			
			_facebook.RequestWithMethodName("facebook.photos.cropProfilePic", parameters, "POST", _uploadNextDelegate);
			*/						
		}
		
		public void PostToWall()
		{			
			var actionLinks = new JsonArray();
			
			var learnMore = new JsonObject();
			learnMore.Add("text", "Learn more about Big Profile");
			learnMore.Add("href", "http://myapp.no/BigProfile");
			
			var appStore = new JsonObject();
			appStore.Add("text", "Visit App Store");
			appStore.Add("href", "http://myapp.no/BigProfileAppStore");
			
			//actionLinks.Add(learnMore);
			actionLinks.Add(appStore);
			
			var attachment = new JsonObject();
			attachment.Add("name", "Big Profile");
			attachment.Add("caption", "Check out my new profile picture");
			attachment.Add("description", "Make your profile stand out with a big profile picture stretched across the new Facebook design. Available in App Store!");
			attachment.Add("href", "http://myapp.no/BigProfile");
									
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("user_message_prompt"), new NSString("Tell your friends"));
			parameters.Add(new NSString("attachment"), new NSString(attachment.ToString()));     
			parameters.Add(new NSString("action_links"), new NSString(actionLinks.ToString()));
			
			_facebook.Dialog("stream.publish", parameters, facebookDialogDelegate);
		}
		
		private FacebookDialogDelegate facebookDialogDelegate = new FacebookDialogDelegate();
	}
	
	public class FacebookDialogDelegate : FBDialogDelegate
	{
		public override void DialogDidComplete (FBDialog dialog)
		{
		}		
		
		public override void DialogCompleteWithUrl (NSUrl url)
		{
		}		
		
		public override void DialogDidNotCompleteWithUrl (NSUrl url)
		{
		}	
		
		public override void DialogDidNotComplete (FBDialog dialog)
		{
		}		
		
		public override void Dialog (FBDialog dialog, NSError error)
		{
			Console.WriteLine("Error: " + error.ToString());
		}		
		
		public override bool Dialog (FBDialog dialog, NSUrl url)
		{
			return true;
		}
	}
	
	public class UploadNextRequestDelegate : RequestDelegateBase
	{
		private readonly FacebookController _controller;
		
		public UploadNextRequestDelegate(FacebookController controller) : base(controller)
		{
			_controller = controller;
		}
		
		public override void HandleResult (FBRequest request, NSDictionary result)
		{
			_controller.StartUpload();
		}
	}
}