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
		private FacebookPhotoGrid _photoGrid;
		
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
							
		public void AlbumSelected(Album album)
		{
			if(_photoGrid == null)
				_photoGrid = new FacebookPhotoGrid();
			
			_photoGrid.LoadPhotos(album);
			NavigationController.PushViewController(_photoGrid, true);
		}
	}
}