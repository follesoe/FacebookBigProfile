using System;
using FacebookSdk;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class UploadPhotoRequestDelegate : RequestDelegateBase
	{
		private readonly FacebookController _controller;
		
		public UploadPhotoRequestDelegate(FacebookController controller)
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
				NSObject id = dict.ObjectForKey(new NSString("id"));
				_controller.TagPhoto(id.ToString()); 	
			}
		}
	}
}