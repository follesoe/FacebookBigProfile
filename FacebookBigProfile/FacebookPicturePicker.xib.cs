
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using FacebookSdk;

namespace FacebookBigProfile
{
	public partial class FacebookPicturePicker : UIViewController
	{		
		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public FacebookPicturePicker (IntPtr handle) : base(handle)
		{
			Initialize();
		}

		[Export("initWithCoder:")]
		public FacebookPicturePicker (NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		private readonly UINavigationController _parentNavigationController;
		private readonly Facebook _facebook;

		public FacebookPicturePicker(UINavigationController parentNavigationController, Facebook facebook) : base("FacebookPicturePicker", null)
		{
			Initialize ();
			_parentNavigationController = parentNavigationController;
			_facebook = facebook;
		}
		
		private void Initialize()
		{
		}
		
		private FacebookAlbumTableViewController _albums;
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			
			cancelButton.Clicked += (o, e) => {
				_parentNavigationController.PopViewControllerAnimated(true);				
			};
			
			_albums = new FacebookAlbumTableViewController(_facebook);
			_albums.View.Bounds = new RectangleF(0, 0, 320, View.Frame.Height - 44);
			_albums.View.Frame = new RectangleF(0, 44, 320, _albums.View.Bounds.Height);
			View.AddSubview(_albums.View);			
		}
	}
}

