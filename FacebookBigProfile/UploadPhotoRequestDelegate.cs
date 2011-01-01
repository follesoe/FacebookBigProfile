using System;
using FacebookSdk;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class UploadPhotoRequestDelegate : RequestDelegateBase
	{
		private readonly FacebookController _controller;
		
		public bool AutoTag { get; set; }
		
		public UploadPhotoRequestDelegate(FacebookController controller) : base(controller)
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
				if(AutoTag) 
				{
					NSObject id = dict.ObjectForKey(new NSString("id"));
					_controller.GetPIDforPhotoFBID(id.ToString()); 	
				}
				else 
				{
					_controller.StartUpload();
				}
			}
		}
	}
}