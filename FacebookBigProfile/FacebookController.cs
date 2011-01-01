using System;
using FacebookSdk;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Json;
using System.Collections.Generic;

namespace FacebookBigProfile
{
	public class FacebookController
	{
		public class QueuedUpload 
		{
			public UIImage Image { get; set; }
			public string Message { get; set; }
			public bool AutoTag { get; set; }
		}		
		
		private readonly Facebook _facebook;
		private readonly MainView _mainView;
		private readonly GetUserRequestDelegate _userDelegate;
		private readonly UploadPhotoRequestDelegate _photoDelegate;
		private readonly FqlRequestDelegate _fqlDelegate;
		private readonly SessionDelegate _sessionDelegate;
		private readonly UploadNextRequestDelegate _uploadNextDelegate;
		
		private const string ProgresString = "Uploading part {0} of 6 of your awesome profile picture...";
		
		private Queue<QueuedUpload> _queuedUploads;
		
		private string _userId;
		private bool _isLoggedIn; 
		
		public bool IsLoggedIn 
		{
			get { return _isLoggedIn; }
			set 
			{
				_isLoggedIn = value;
				if(_isLoggedIn) 
				{
					_mainView.StartProgress(string.Format(ProgresString, 1));			
					GetProfile();
				}
			}
		}		
		
		public FacebookController(Facebook facebook, MainView mainView)
		{
			_facebook = facebook;
			_mainView = mainView;
			_photoDelegate = new UploadPhotoRequestDelegate(this);
			_userDelegate = new GetUserRequestDelegate(this);
			_sessionDelegate = new SessionDelegate(this);
			_fqlDelegate = new FqlRequestDelegate(this);
			_uploadNextDelegate = new UploadNextRequestDelegate(this);
			_queuedUploads = new Queue<QueuedUpload>();
		}
		
		public void Login() 
		{
			if(_facebook.IsSessionValid)
			{
				IsLoggedIn = true;
			}
			else 
			{
				_facebook.Authorize(new string[]{"publish_stream", "read_stream", "user_photos", "offline_access"}, _sessionDelegate);
			}
		}
		
		public void Logout() 
		{			
			_facebook.Logout(_sessionDelegate);
		}
		
		public void LoggedIn(string userId) 
		{
			_userId = userId;
			_mainView.SplitImage();
		}
		
		public void ErrorOccurred(NSError error)
		{
			_mainView.StopProgress();
			_mainView.ShowError(error);
		}
		
		public void GetProfile() 
		{
			if(!IsLoggedIn) throw new Exception("User not logged in!");
			
			_facebook.RequestWithGraphPath("me", _userDelegate);
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
			if(!IsLoggedIn) throw new Exception("User not logged in.");	
			if(string.IsNullOrEmpty(_userId)) throw new Exception("Logged in but missing user id.");
		
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("picture"), image);
			parameters.Add(new NSString("message"), new NSString(message));
			
			_photoDelegate.AutoTag = autoTag;
			_facebook.RequestWithGraphPath("/me/photos", parameters, "POST", _photoDelegate);			
		}		
		
		public void GetPIDforPhotoFBID(string fbid)
		{			
			string fql = "SELECT pid FROM photo WHERE object_id = " + fbid;
			
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("query"), new NSString(fql));	
			
			_facebook.RequestWithMethodName("fql.query", parameters, "POST", _fqlDelegate);	
		}
		
		public void TagPhoto(string photoPid)
		{
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("pid"), new NSString(photoPid));
			parameters.Add(new NSString("tag_uid"), new NSString(_userId));
			parameters.Add(new NSString("x"), new NSString("10.0"));
			parameters.Add(new NSString("y"), new NSString("10.0"));
			_facebook.RequestWithMethodName("photos.addTag", parameters, "POST", _uploadNextDelegate);	
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