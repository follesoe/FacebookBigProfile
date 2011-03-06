using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace FacebookBigProfile
{
	public partial class PhotosTableViewCell : UIViewController
	{		
		public event ImageSelected ImageSelected;
		
		private PhotoGroup _group;
		
		public PhotosTableViewCell (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public PhotosTableViewCell (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public PhotosTableViewCell ()
		{
			MonoTouch.Foundation.NSBundle.MainBundle.LoadNib ("PhotosTableViewCell", this, null);
			Initialize ();
		}
		
		private void Initialize ()
		{			
			btn1.TouchUpInside += (o, e) => RaiseImageSelected(0);
			btn2.TouchUpInside += (o, e) => RaiseImageSelected(1);
			btn3.TouchUpInside += (o, e) => RaiseImageSelected(2);
			btn4.TouchUpInside += (o, e) => RaiseImageSelected(3);
		}
		
		public UITableViewCell Cell
		{
			get { return cell; }
		}		
		
		public void RaiseImageSelected(int number)
		{
			if(_group != null && ImageSelected != null)
			{
				ImageSelected(_group.GetPhoto(number).Source);
			}
		}
		
		public void ShowPhotos(PhotoGroup group)
		{
			_group = group;
			LoadPhoto(0, btn1);
			LoadPhoto(1, btn2);
			LoadPhoto(2, btn3);
			LoadPhoto(3, btn4);				
		}
		
		public void LoadPhoto(int number, UIButton btn)
		{
			var photo = _group.GetPhoto(number);
				
			if(photo == null)
			{
				btn.Alpha = 0;
				btn.Enabled = false;
			}
			else
			{
				btn.Alpha = 1;
				btn.Enabled = true;
				
				ImageLoader.GetImage(new GetImageRequest(photo.Id, number, new Uri(photo.SmallSource, UriKind.Absolute)), UpdatePhoto);
			}
		}
		
		public void UpdatePhoto(UpdateImage update)
		{
			BeginInvokeOnMainThread(() => {
				if(update.Number == 0) btn1.SetBackgroundImage(update.Image, UIControlState.Normal);				
				if(update.Number == 1) btn2.SetBackgroundImage(update.Image, UIControlState.Normal);
				if(update.Number == 2) btn3.SetBackgroundImage(update.Image, UIControlState.Normal);
				if(update.Number == 3) btn4.SetBackgroundImage(update.Image, UIControlState.Normal);
			});
		}
	}	
	
	public delegate void ImageSelected(string url);
	
	public class GetImageRequest 
	{
		public string Id { get; set; }
		public int Number { get; set; }
		public Uri Uri { get; set; }
		
		public GetImageRequest(string id, int number, Uri uri)
		{
			Id = id;
			Number = number;
			Uri = uri;
		}
	}
	
	public class UpdateImage
	{
		public int Number { get; set; }
		public Uri Uri { get; set; }
		public UIImage Image { get; set; }
						
		public UpdateImage(int number, Uri uri, UIImage image)			
		{
			Number = number;
			Uri = uri;
			Image = image;
		}
	}
}

