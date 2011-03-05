using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FacebookSdk;
using System.Drawing;

namespace FacebookBigProfile
{
	public partial class FacebookPhotoGrid : UIViewController
	{
		private readonly Facebook _facebook;
		private readonly FacebookPicturePicker _picker;
		private FacebookPhotosTableViewController _photos;
		
		public FacebookPhotoGrid(Facebook facebook, FacebookPicturePicker picker) : base("FacebookPhotoGrid", null)
		{			
			_facebook = facebook;
			_picker = picker;
		}		
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			
			Console.WriteLine("PhotoGrid DidLoad");
			
			_photos = new FacebookPhotosTableViewController(_facebook, _picker);
			_photos.View.Frame = new RectangleF(0, 0, 320, 450);
			View.AddSubview(_photos.View);				
		}
		
		public void LoadPhotos(Album album)
		{	
			_photos.LoadFromAlbum(album);
		}
	}
}