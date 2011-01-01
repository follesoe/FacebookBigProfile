using System;
using FacebookSdk;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public abstract class RequestDelegateBase : FBRequestDelegate
	{				
		private FacebookController _controller;
		
		public RequestDelegateBase(FacebookController controller)
		{
			_controller = controller;
		}
		
		public abstract void HandleResult(FBRequest request, NSDictionary result);
		
		public override void RequestLoading (FBRequest request)
		{
		}
				
		public override void Request (FBRequest request, NSUrlResponse response)
		{
		}		
		
		public override void Request (FBRequest request, NSError error)
		{
			_controller.ErrorOccurred(error);			
		}
				
		public override void Request (FBRequest request, NSObject result)
		{
			NSDictionary dict;
			
			if(result is NSDictionary)
			{	
				dict = result as NSDictionary;
			}
			else if(result is NSArray)
			{
				var arr = (NSArray)result;
				
				if(arr.Count > 0)
					dict = new NSDictionary(arr.ValueAt(0));
				else 
					dict = new NSDictionary();
			}
			else
			{
				throw new Exception("cannot handle result in FBRequestDelegate callback");
			}
			
			HandleResult(request, dict);
		}	
		
		public override void Request (FBRequest request, NSData data)
		{
		}
	}
}

