using System;
using FacebookSdk;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class FqlRequestDelegate : RequestDelegateBase
	{	
		public bool IsWallPhoto { get; set; }
		
		private readonly FacebookController _controller;
		
		public FqlRequestDelegate(FacebookController controller) : base(controller)
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
				if(IsWallPhoto)
				{
					_controller.TagPhoto(id.ToString());
				}
				else 
				{
					_controller.MakeProfilePhoto(id.ToString());
				}
			}
		}
	}
}

