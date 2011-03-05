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
		
		public FacebookPhotoGrid (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public FacebookPhotoGrid (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public FacebookPhotoGrid (Facebook facebook, FacebookPicturePicker picker) : base("FacebookPhotoGrid", null)
		{			
			_facebook = facebook;
			_picker = picker;
			
			Initialize ();
		}

		private void Initialize ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Photos";
			
			var list = new FacebookPhotosTableViewController(_facebook, _picker);
			list.View.Frame = new RectangleF(0, 0, 320, 450);
			View.AddSubview(list.View);
		}
		
		public void LoadPhotos(Album album)
		{
			Console.WriteLine("Load photos from " + album.Name);
		}
	}
}