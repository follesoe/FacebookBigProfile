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
		private readonly Facebook _facebook;
		private readonly MainView _mainView;
		private FacebookAlbumTableViewController _tableView;
		
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
			NavigationController.SetNavigationBarHidden(false, true);			
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Console.WriteLine("PicturePicker DidLoad");
			
			Title = "Facebook Albums";
			
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
			if(_photoGrid == null)
			{
				_photoGrid = new FacebookPhotoGrid(_facebook, this);
			}
						
			NavigationController.PushViewController(_photoGrid, true);
			_photoGrid.LoadPhotos(album);
		}
		
		public void StartProgress(string message)
		{
			_mainView.StartProgress(message);
		}		
		
		public void StopProgress()
		{
			_mainView.StopProgress();
		}
	}
}