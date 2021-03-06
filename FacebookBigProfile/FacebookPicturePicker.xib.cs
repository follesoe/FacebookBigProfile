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
		private readonly Facebook _facebook;
		private readonly MainView _mainView;
		private FacebookAlbumTableViewController _tableView;
		private FacebookPhotosTableViewController _photos;		
		
		public FacebookPicturePicker (IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public FacebookPicturePicker (NSCoder coder) : base(coder)
		{			
		}	

		public FacebookPicturePicker(Facebook facebook, MainView mainView) : base("FacebookPicturePicker", null)
		{
			_facebook = facebook;
			_mainView = mainView;
		}
		
		public override void ViewWillAppear (bool animated)
		{
			Title = "Facebook Albums";
			NavigationController.SetNavigationBarHidden(false, true);			
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
												
			_tableView = new FacebookAlbumTableViewController(_facebook, this);				
			_tableView.View.Frame = new RectangleF(0, 0, 320, 450);
			View.AddSubview(_tableView.View);			
		}	
		
		public override void ViewDidAppear (bool animated)
		{
			_tableView.GetAlbums();
		}
									
		public void AlbumSelected(Album album)
		{
			if(_photos == null)
			{
				_photos = new FacebookPhotosTableViewController(_facebook, this);
				_photos.View.Frame = new RectangleF(0, 0, 320, 450);
			}
			_photos.LoadFromAlbum(album);
			
			Title = "Albums";
			NavigationController.PushViewController(_photos, true);		
		}
		
		public void PhotoSelected(string url)
		{						
			NavigationController.PopToRootViewController(true);
			_mainView.StartDownloadImage(url);
		}		
	}
}