using System;
using FacebookSdk;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class FacebookLoginController : IFacebookErrorProvider
	{
		private readonly MainView _mainView;
		private readonly Facebook _facebook;
		private readonly SessionDelegate _sessionDelegate;
		private readonly GetUserRequestDelegate _userDelegate;
		
		public event EventHandler UserIsLoggedIn; 
				
		public FacebookLoginController (Facebook facebook, MainView mainView)
		{
			_facebook = facebook;
			_mainView = mainView;
			_sessionDelegate = new SessionDelegate(this);
			_userDelegate = new GetUserRequestDelegate(this);			
		}
						
		public string UserId 
		{
			get; private set;
		}		
		
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
		
		public void ErrorOccurred(NSError error)
		{
			_mainView.StopProgress();
			_mainView.ShowError(error);
		}
		
		public void Login() 
		{
			if(_facebook.IsSessionValid)
			{
				IsLoggedIn = true;
			}
			else 
			{
				_facebook.Authorize(new string[]{"publish_stream", "read_stream", "user_photos"}, _sessionDelegate);
			}
		}
		
		public void Logout() 
		{			
			_facebook.Logout(_sessionDelegate);
		}
		
		public void LoggedIn(string userId) 
		{
			UserId = userId;
			if(UserIsLoggedIn != null)
				UserIsLoggedIn(this, new EventArgs());			
		}
		
		public void GetProfile() 
		{
			if(!IsLoggedIn) throw new Exception("User not logged in!");
			
			_facebook.RequestWithGraphPath("me", _userDelegate);
		}
	}
}