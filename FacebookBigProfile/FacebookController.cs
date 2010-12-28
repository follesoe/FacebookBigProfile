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
		private Facebook _facebook;
		private GetUserRequestDelegate _userDelegate;
		private UploadPhotoRequestDelegate _photoDelegate;
		private FqlRequestDelegate _fqlDelegate;
		private SessionDelegate _sessionDelegate;
		
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
					GetProfile();
				}
			}
		}		
		
		public FacebookController (Facebook facebook)
		{
			_facebook = facebook;
			_photoDelegate = new UploadPhotoRequestDelegate(this);
			_userDelegate = new GetUserRequestDelegate(this);
			_sessionDelegate = new SessionDelegate(this);
			_fqlDelegate = new FqlRequestDelegate();
		}
		
		public void Login() 
		{
			Console.WriteLine(_facebook.AccessToken);
			Console.WriteLine(_facebook.ExpirationDate);
			
			if(_facebook.IsSessionValid)
			{
				IsLoggedIn = true;
			}
			else 
			{
				_facebook.Authorize(new string[]{"publish_stream" /*, "offline_access"*/}, _sessionDelegate);
			}
		}
		
		public void Logout() 
		{			
			_facebook.Logout(_sessionDelegate);
		}
		
		public void LoggedIn(string userId) 
		{
			_userId = userId;
		}
		
		public void GetProfile() 
		{
			if(!IsLoggedIn) throw new Exception("User not logged in!");
			
			_facebook.RequestWithGraphPath("me", _userDelegate);
		}
		
		public void UploadPhoto(UIImage image, string message) 
		{
			if(!IsLoggedIn) throw new Exception("User not logged in.");	
			if(string.IsNullOrEmpty(_userId)) throw new Exception("Logged in but missing user id.");
						                          		
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("picture"), image);
			parameters.Add(new NSString("message"), new NSString(message));
			_facebook.RequestWithGraphPath("/me/photos", parameters, "POST", _photoDelegate);			
		}
		
		public void TagPhoto(string photoPid)
		{
			Console.WriteLine("TagPhoto: " + photoPid);
			GetPIDforPhotoFBID(photoPid);
			return;
			
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("pid"), new NSString(photoPid));
			parameters.Add(new NSString("tag_uid"), new NSString(_userId));
			parameters.Add(new NSString("x"), new NSString("10.0"));
			parameters.Add(new NSString("y"), new NSString("10.0"));
			_facebook.RequestWithMethodName("photos.addTag", parameters, "POST", _userDelegate);	
		}
		
		private void GetPIDforPhotoFBID(string fbid)
		{			
			string fql = "SELECT pid FROM photo WHERE object_id = " + fbid;
			//fql =        "SELECT uid,name FROM user WHERE uid=4";
				
			
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("query"), new NSString(fql));	
			
			Console.WriteLine("Making FQL Request: " + fql);
			_facebook.RequestWithMethodName("fql.query", parameters, "POST", _fqlDelegate);	
		}
	}
	
	public class FqlRequestDelegate : RequestDelegateBase
	{		
		public override void HandleResult (FBRequest request, NSDictionary result)
		{
			Console.WriteLine("HandleResult for FQL");
		}
	}
}