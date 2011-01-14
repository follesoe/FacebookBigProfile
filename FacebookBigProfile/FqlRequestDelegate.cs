using System;
using FacebookSdk;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class FqlRequestDelegate : RequestDelegateBase
	{	
		private readonly FacebookController _controller;
		
		public FqlRequestDelegate(FacebookController controller)
		{
			_controller = controller;
		}
		
		public override void HandleResult (FBRequest request, NSDictionary dict)
		{
			if (dict.ObjectForKey(new NSString("owner")) != null)
		    {
			}
			else 
			{
				NSObject id = dict.ObjectForKey(new NSString("pid"));
				_controller.TagPhoto(id.ToString()); 	
			}
		}
	}
}

