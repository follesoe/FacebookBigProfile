using System;
using FacebookSdk;

namespace FacebookBigProfile
{
	public class SessionDelegate : FBSessionDelegate
	{
		private FacebookLoginController _facebookController;
		
		public SessionDelegate(FacebookLoginController facebookController)
		{
			_facebookController = facebookController;
		}
			
		public override void FbDidLogin()
		{
			_facebookController.IsLoggedIn = true;
		}
		
		public override void FbDidLogout()
		{
			_facebookController.IsLoggedIn = false;
		}
		
		public override void FbDidNotLogin(bool cancelled)
		{
			_facebookController.IsLoggedIn = false;
		}
	}
}