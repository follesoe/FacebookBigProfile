using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FacebookSdk;

namespace FacebookBigProfile
{
	public partial class FacebookPicturePicker : UIViewController
	{		
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

