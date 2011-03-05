using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace FacebookBigProfile
{
	public partial class FacebookPhotoGrid : UIViewController
	{
		public FacebookPhotoGrid (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public FacebookPhotoGrid (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public FacebookPhotoGrid () : base("FacebookPhotoGrid", null)
		{
			Initialize ();
		}

		private void Initialize ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "Photos";
		}
		
		public void LoadPhotos(Album album)
		{
			Console.WriteLine("Load photos from " + album.Name);
		}
	}
}