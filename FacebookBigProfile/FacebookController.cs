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
		private RequestDelegate _requestDelegate;
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
			_requestDelegate = new RequestDelegate(this);
			_sessionDelegate = new SessionDelegate(this);
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
				_facebook.Authorize(new string[]{"publish_stream", "offline_access"}, _sessionDelegate);
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
			
			_facebook.RequestWithGraphPath("me", _requestDelegate);
		}
		
		public void UploadImage(UIImage image, string message) 
		{
			if(!IsLoggedIn) throw new Exception("User not logged in.");	
			if(string.IsNullOrEmpty(_userId)) throw new Exception("Logged in but missing user id.");
			
			var tag = new JsonObject();
			tag.Add("id", _userId);
			tag.Add("x", "10");
			tag.Add("y", "10");
			
			var tags = new JsonObject();
			tags.Add("data", new JsonArray(tag));
			                          		
			var parameters = new NSMutableDictionary();
			parameters.Add(new NSString("picture"), image);
			parameters.Add(new NSString("message"), new NSString(message));
			parameters.Add(new NSString("tags"), new NSString(tags.ToString()));
			_facebook.RequestWithGraphPath("/me/photos", parameters, "POST", _requestDelegate);		
		}
	}
}