using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace FacebookBigProfile
{
	public partial class SetProfilePictureView : UIViewController
	{
		public SetProfilePictureView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public SetProfilePictureView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public SetProfilePictureView () : base("SetProfilePictureView", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			Title = "Set Profile Picture";
			base.ViewDidLoad ();
		}
		
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetNavigationBarHidden(false, true);
			base.ViewDidAppear (animated);
		}
	}
}

