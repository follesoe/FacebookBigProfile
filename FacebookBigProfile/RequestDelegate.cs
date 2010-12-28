using System;
using FacebookSdk;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class RequestDelegate : FBRequestDelegate
	{
		private FacebookController _facebookController;
		
		public RequestDelegate(FacebookController facebookController)
		{
			_facebookController = facebookController;
		}
		
		public override void RequestLoading(FBRequest request)
		{
			Console.WriteLine("Request Loading...");
		}
		
		public override void Request (FBRequest request, NSUrlResponse response)
		{
			
		}
		
		public override void Request (FBRequest request, NSError error)
		{
			Console.WriteLine("Error: " + error.ToString());
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
				dict = new NSDictionary(arr.ValueAt(0));
			}
			else
			{
				throw new Exception("cannot handle result in FBRequestDelegate callback");
			}
			
			if (dict.ObjectForKey(new NSString("owner")) != null)
		    {
			}
			else 
			{
				NSObject name =	dict.ObjectForKey(new NSString("name"));
				NSObject id = dict.ObjectForKey(new NSString("id"));
				Console.WriteLine("Logged in as: {0} - {1}", name, id);	
				
				_facebookController.LoggedIn(id.ToString());
			}
		}
	
		public override void Request (FBRequest request, NSData data)
		{
			
		}
	}
}