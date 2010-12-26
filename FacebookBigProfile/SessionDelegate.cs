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
			Console.WriteLine("FbDidLogin");
			_facebookController.IsLoggedIn = true;
		}
		
		public override void FbDidLogout()
		{
			Console.WriteLine("FbDidLogout");
			_facebookController.IsLoggedIn = false;
		}
		
		public override void FbDidNotLogin(bool cancelled)
		{
			Console.WriteLine("FB Did Not Login");
			_facebookController.IsLoggedIn = false;
		}
	}
}