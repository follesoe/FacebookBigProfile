using System;
using FacebookSdk;

namespace FacebookBigProfile
{
	public class FacebookController
	{
		private Facebook _facebook;
		private RequestDelegate _requestDelegate;
		private SessionDelegate _sessionDelegate;
		
		private bool _isLoggedIn; 
		
		public bool IsLoggedIn {
			get { return _isLoggedIn; }
			set {
				_isLoggedIn = value;
				if(_isLoggedIn) 
					GetProfile();
			}
		}
		
		public FacebookController (Facebook facebook)
		{
			_facebook = facebook;
			_requestDelegate = new RequestDelegate(this);
			_sessionDelegate = new SessionDelegate(this);
		}
		
		public void Login() {
			_facebook.Authorize(new string[]{"read_stream", "offline_access"}, _sessionDelegate);
		}
		
		public void Logout() {
			_facebook.Logout(_sessionDelegate);
		}
		
		public void GetProfile() {
			if(!IsLoggedIn) throw new Exception("User not logged in!");
			
			_facebook.RequestWithGraphPath("me", _requestDelegate);
		}
		
		public void ShowName(string name) {
			Console.WriteLine("Logged in as: " + name);
		}
	}
}