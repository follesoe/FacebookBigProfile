
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace FacebookBigProfile
{
	public partial class FacebookPhotoGrid : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

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
		
		#endregion
	}
}

