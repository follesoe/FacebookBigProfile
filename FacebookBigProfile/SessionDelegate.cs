using System;
using FacebookSdk;

namespace FacebookBigProfile
{
	public class SessionDelegate : FBSessionDelegate
	{
		private FacebookController _facebookController;
		
		public SessionDelegate(FacebookController facebookController)
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