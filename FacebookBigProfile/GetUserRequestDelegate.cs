using System;
using FacebookSdk;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class GetUserRequestDelegate : RequestDelegateBase
	{
		private FacebookController _facebookController;
		
		public GetUserRequestDelegate(FacebookController facebookController) : base(facebookController)
		{
			_facebookController = facebookController;
		}
		
		public override void HandleResult (FBRequest request, NSDictionary dict)
		{
			if (dict.ObjectForKey(new NSString("owner")) != null)
		    {
			}
			else 
			{
				NSObject id = dict.ObjectForKey(new NSString("id"));				
				_facebookController.LoggedIn(id.ToString());
			}
		}
	}
}