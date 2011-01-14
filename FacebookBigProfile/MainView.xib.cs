using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

using FacebookSdk;

namespace FacebookBigProfile
{
	public partial class MainView : UIViewController
	{
		// The IntPtr and initWithCoder constructors are required for items that need 
		// to be able to be created from a xib rather than from managed code

		public MainView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public MainView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MainView (Facebook facebook) : base("MainView", null)
		{
			this.facebook = facebook;
			Initialize ();
		}		
		
		private Facebook facebook;
		private FacebookController facebookController;
		private UIImagePickerController picker;
		private UIImageView profilePictureView;
		private UIImage overlayImage;
		private UIImage profilePicture;
		private UIActionSheet photoFromWhere;
		private LoadingView loadingView;
		
		private UIImageView cropSource1;
		private UIImageView cropSource2;
		private UIImageView cropSource3;
		private UIImageView cropSource4;
		private UIImageView cropSource5;
		private UIImageView cropSource6;
		
		private SizeF profilePictureSize;
		private SizeF profilePictureSmallSize;
		
		private void Initialize ()
		{
			View.Frame = new RectangleF(0, 20, View.Frame.Width, View.Frame.Height);
			facebookController = new FacebookController(facebook, this);		
			profilePictureSize = new SizeF(180f, 540f);
			profilePictureSmallSize = new SizeF(97f, 68f);
		}
		
		private UIImageView CreateCropSource(float x, float y, float width, float height) 
		{
			var imageView = new UIImageView();
			imageView.Frame = new RectangleF(x, y, width, height);
			return imageView;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad();
					
			profilePictureView = new UIImageView();
									
			scrollView.AddSubview(profilePictureView);
			scrollView.MaximumZoomScale = 5f;
			scrollView.MinimumZoomScale = 0.2f;
			scrollView.Bounces = false;
			scrollView.BouncesZoom = false;			
			scrollView.IndicatorStyle = UIScrollViewIndicatorStyle.Black;
					 				
			scrollView.ViewForZoomingInScrollView = (sender) => { return profilePictureView; };		
			
			LoadImage(UIImage.FromFile("ProfilePicture.jpg"));
			
			overlayImage = UIImage.FromFile("FacebookOverlay.png");
			facebookOverlay.Image = overlayImage;	
			
			picker = new UIImagePickerController();
			picker.Delegate = new ImagePickerDelegate(this);
			
						
			if(UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera)) 
			{			
				photoFromWhere = new UIActionSheet("", new ActionDel(this), "Cancel", null, "Take Photo", "Choose From Library");
				photoButton.Clicked += (o, e) => photoFromWhere.ShowFromToolbar(toolbar);
			} 
			else
			{
				photoButton.Clicked += (o, e) => GetPhotoFromLibrary();			
			}
		
			facebookButton.Clicked += (o, e) => LoginToFacebook();
			
			loadingView = new LoadingView();
			
			AddCropHelpers();
		}
		
		public void StartProgress(string title)
		{
			loadingView.Show(title);			
		}
		
		public void UpdateProgress(string title)
		{
			loadingView.UpdateTitle(title);
		}
		
		public void StopProgress()
		{
			loadingView.Hide();			
			facebookController.PostToWall();
		}
		
		public void ShowError(NSError error)
		{
			Console.WriteLine("Error: " + error.ToString());
		}
		
		public void GetPhotoFromLibrary()
		{
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			PresentModalViewController(picker, true);		
		}
		
		public void GetPhotoFromCamera()
		{
			picker.SourceType = UIImagePickerControllerSourceType.Camera;
			PresentModalViewController(picker, true);
		}
		
		public void LoginToFacebook() 
		{
			if(Reachability.RemoteHostStatus() == NetworkStatus.NotReachable)
			{
				using(var alert = new UIAlertView("No connection", "You need an Internet connection to upload your Big Profile", null, "OK", null))
				{
					alert.Show();
				}				
			}
			else 
			{
				facebookController.Login();
			}
		}

		private void AddCropHelpers() 
		{
			cropSource6 = CreateCropSource(5, 25, 80, 242);
			cropSource5 = CreateCropSource(94, 57, 43, 30);
			cropSource4 = CreateCropSource(138, 57, 44, 30);			
			cropSource3 = CreateCropSource(183, 57, 43, 30);
			cropSource2 = CreateCropSource(227, 57, 44, 30);
			cropSource1 = CreateCropSource(272, 57, 43, 30);
			
			View.AddSubview(cropSource1);
			View.AddSubview(cropSource2);
			View.AddSubview(cropSource3);
			View.AddSubview(cropSource4);
			View.AddSubview(cropSource5);
			View.AddSubview(cropSource6);
		}		
		
		public void LoadImage(UIImage image) 
		{						
			profilePicture = image;
			float zoomScale = CropHelpers.GetZoomScale(profilePicture.Size, scrollView.Frame.Size);	
			var frame = new RectangleF(0f, 0f, image.Size.Width * zoomScale, image.Size.Height * zoomScale);
			var size = scrollView.Frame.Size;
						
			profilePictureView.Frame = frame;
			profilePictureView.Image = image;			
			scrollView.ContentSize = frame.Size;
			scrollView.ContentInset = new UIEdgeInsets(size.Height * 0.8f, size.Width * 0.8f, size.Height * 0.8f, size.Width * 0.8f);
			scrollView.ContentOffset = new PointF(0, 0);
		}			
		
		public void SplitImage() 
		{		
			float zoomScale = CropHelpers.GetZoomScale(profilePicture.Size, scrollView.Frame.Size);	
			float currentZoomScale = scrollView.ZoomScale * zoomScale;
			
			var imageCrop1 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource1.Frame, scrollView.ContentOffset, currentZoomScale); 
			var imageCrop2 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource2.Frame, scrollView.ContentOffset, currentZoomScale); 
			var imageCrop3 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource3.Frame, scrollView.ContentOffset, currentZoomScale); 
			var imageCrop4 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource4.Frame, scrollView.ContentOffset, currentZoomScale); 
			var imageCrop5 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource5.Frame, scrollView.ContentOffset, currentZoomScale); 			
			var imageCrop6 = CropHelpers.CalculateScaledCropSource(profilePicture.Size, cropSource6.Frame, scrollView.ContentOffset, currentZoomScale); 
			
			var cropped1 = Crop(profilePicture, imageCrop1).Scale(profilePictureSmallSize);
			var cropped2 = Crop(profilePicture, imageCrop2).Scale(profilePictureSmallSize);
			var cropped3 = Crop(profilePicture, imageCrop3).Scale(profilePictureSmallSize);
			var cropped4 = Crop(profilePicture, imageCrop4).Scale(profilePictureSmallSize);
			var cropped5 = Crop(profilePicture, imageCrop5).Scale(profilePictureSmallSize);
			var cropped6 = Crop(profilePicture, imageCrop6).Scale(profilePictureSize);
			
			facebookController.QueueForUpload(cropped1, "Part 1 of my Big Profile Picture.", true);
			facebookController.QueueForUpload(cropped2, "Part 2 of my Big Profile Picture.", true);
			facebookController.QueueForUpload(cropped3, "Part 3 of my Big Profile Picture.", true);
			facebookController.QueueForUpload(cropped4, "Part 4 of my Big Profile Picture.", true);
			facebookController.QueueForUpload(cropped5, "Part 5 of my Big Profile Picture.", true);
			facebookController.QueueForUpload(cropped6, "Part 6 of my Big Profile Picture.", false);	
			facebookController.StartUpload();		
		}
		
		public UIImage Crop(UIImage image, RectangleF section)
	    {			
			UIGraphics.BeginImageContext(section.Size);			
			var context = UIGraphics.GetCurrentContext();
			
			var transform = new MonoTouch.CoreGraphics.CGAffineTransform(1, 0, 0, -1, 0, section.Height);
			context.ConcatCTM(transform);	

			context.DrawImage(new RectangleF(0, 0, section.Width, section.Height), 
			                  image.CGImage.WithImageInRect(section));
			
			var croppedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return croppedImage;
	    }
		
		public class ActionDel : UIActionSheetDelegate
		{
			private readonly MainView _mainView;
			
			public ActionDel(MainView mainView)
			{
				_mainView = mainView;
			}
			
			public override void Clicked (UIActionSheet actionSheet, int buttonIndex)
			{
				if(buttonIndex == 1) _mainView.GetPhotoFromCamera();
				else if(buttonIndex == 2) _mainView.GetPhotoFromLibrary();
			}
		}
	}
}






