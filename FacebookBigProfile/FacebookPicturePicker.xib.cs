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
		
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetNavigationBarHidden(false, true);			
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Facebook Albums";
			
			var list = new FacebookAlbumTableViewController(_facebook, this);
			list.View.Frame = new RectangleF(0, 0, 320, 450);
			View.AddSubview(list.View);
		}
	
				
		/*
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);		
			
			cancelButton.Clicked += (o, e) => {
				_parentNavigationController.PopViewControllerAnimated(true);				
			};
			
			_albums = new FacebookAlbumTableViewController(_facebook, this);
			_albums.View.Bounds = new RectangleF(0, 0, 320, View.Frame.Height - 44);
			_albums.View.Frame = new RectangleF(0, 44, 320, _albums.View.Bounds.Height);					
		}*/
		
		
		public void AlbumSelected(Album album)
		{
			var pictures = new FacebookPhotoGrid();	
			NavigationController.PushViewController(pictures, true);
		}
	}
}