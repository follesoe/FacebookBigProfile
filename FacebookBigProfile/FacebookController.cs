using System;
using FacebookSdk;

namespace FacebookBigProfile
{
	public class FacebookController
	{
		private Facebook _facebook;
		private RequestDelegate _requestDelegate;
		private SessionDelegate _sessionDelegate;
		
		public bool IsLoggedIn { get; set; }
		
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
	}
}