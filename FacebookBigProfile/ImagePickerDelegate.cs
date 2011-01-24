using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class ImagePickerDelegate : UIImagePickerControllerDelegate
	{
		private MainView _controller;
		
		public ImagePickerDelegate (MainView controller)
		{
			_controller = controller;	
		}
		
		public override void Canceled (UIImagePickerController picker)
		{
			_controller.DismissModalViewControllerAnimated(true);
		}
		
		public override void FinishedPickingMedia (UIImagePickerController picker, MonoTouch.Foundation.NSDictionary info)
		{
			var originalImage = new NSString("UIImagePickerControllerOriginalImage");
			var image = (UIImage)info[originalImage];
			
			_controller.LoadImage(image);
			_controller.DismissModalViewControllerAnimated(true);
		}
	}
}

