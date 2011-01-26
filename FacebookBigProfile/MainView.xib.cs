using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

using FacebookSdk;
using Atomcraft;
using MonoTouch.CoreGraphics;
using MonoTouch.iAd;

namespace FacebookBigProfile
{
	public partial class MainView : UIViewController
	{
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
		
		private UIImageView cropSource1;
		private UIImageView cropSource2;
		private UIImageView cropSource3;
		private UIImageView cropSource4;
		private UIImageView cropSource5;
		private UIImageView cropSource6;
		
		private SizeF profilePictureSize;
		private SizeF profilePictureSmallSize;
		
		private SetProfilePictureView setProfileView;
		private LinePickerModel linePickerModel;
		
		private ATMHud hud;
		
		private bool uploadedButNotPosted = false;
		private bool lineMessageShown = false;		
		private int numberOfLines = 2;
		
		public UIColor FacebookBlue 
		{
			get { return toolbar.BackgroundColor; }
		}
		
		private void Initialize ()
		{
			View.Frame = new RectangleF(0, 20, View.Frame.Width, View.Frame.Height);
			facebookController = new FacebookController(facebook, this);		
			profilePictureSize = new SizeF(180f, 540f);
			profilePictureSmallSize = new SizeF(97f, 68f);
		}
				
		public override void ViewDidLoad ()
		{				
			base.ViewDidLoad();
			
			linePickerModel = new LinePickerModel(this);
			lineSelector.Model = linePickerModel;
			lineSelector.ShowSelectionIndicator = true;
			
			lineButton.TouchUpInside += SelectLines;
			
			// Profile Picture View may already been set when the Camera closes.
			if(profilePictureView == null)
			{	
				profilePictureView = new UIImageView();
			}						
			else 
			{	
				// Set up Scroll View again when we have a new picture from the camera.
				var size = scrollView.Frame.Size;
				scrollView.ContentSize = profilePictureView.Frame.Size;
				scrollView.ContentInset = new UIEdgeInsets(size.Height * 0.8f, size.Width * 0.8f, size.Height * 0.8f, size.Width * 0.8f);
				scrollView.ContentOffset = new PointF(0, 0);
			}
			
			scrollView.AddSubview(profilePictureView);
			scrollView.MaximumZoomScale = 5f;
			scrollView.MinimumZoomScale = 0.2f;
			scrollView.Bounces = false;
			scrollView.BouncesZoom = false;			
			scrollView.IndicatorStyle = UIScrollViewIndicatorStyle.Black;
					 							
			scrollView.ViewForZoomingInScrollView = (sender) => { return profilePictureView; };		
			
			if(profilePictureView.Image == null)
				LoadImage(UIImage.FromFile("ProfilePicture.jpg"));
			
			overlayImage = UIImage.FromFile(string.Format("FacebookOverlay{0}.png", numberOfLines));
			facebookOverlay.Image = overlayImage;	
			
			picker = new UIImagePickerController();
			picker.Delegate = new ImagePickerDelegate(this);			
						
			if(UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera)) 
			{					
				photoFromWhere = new UIActionSheet("Select an option", new ActionDel(this));
				photoFromWhere.AddButton("Take a photo");
				photoFromWhere.AddButton("Choose from library");
				photoFromWhere.AddButton("Cancel");
				photoFromWhere.CancelButtonIndex = 2;
				
				photoButton.Clicked += (o, e) => photoFromWhere.ShowFromToolbar(toolbar);
			} 
			else
			{
				photoButton.Clicked += (o, e) => GetPhotoFromLibrary();			
			}
		
			facebookButton.Clicked += (o, e) => LoginToFacebook();
						
			hud = new ATMHud();
			View.AddSubview(hud.View);
			
			AddCropHelpers();
			
			var contentIdentifiers = new NSMutableSet(); 
			contentIdentifiers.Add(new NSString("ADBannerContentSize320x50"));
			adView.RequiredContentSizeIdentifiers = contentIdentifiers;
			
			adView.Hidden = true;
			adView.AdLoaded += (o, e) => {
				//adView.Hidden = false;
				Console.WriteLine("AdLoaded");
			};
			
			adView.FailedToReceiveAd += (object o, AdErrorEventArgs e) => {
				adView.Hidden = true;
				Console.WriteLine("FailedToReceiveAd: " + e.Error.ToString());
			};
		}
		
		public override void ViewWillAppear (bool animated)
		{
			NavigationController.SetNavigationBarHidden(true, true);					
			base.ViewDidAppear (animated);
		}
		
		public override void ViewDidAppear (bool animated)
		{
			if(uploadedButNotPosted) 
			{
				uploadedButNotPosted = false;
				PostToWall();											
			}
			base.ViewDidAppear (animated);
		}
		
		public void PostToWall()
		{
			facebookController.PostToWall();
		}
		
		private void SelectLines(object sender, EventArgs e)
		{
			if(lineSelector.Hidden)
			{			   
				lineButton.SetTitle("Done", UIControlState.Normal);
				
				adView.Frame = new RectangleF(0, adView.Frame.Y - lineSelector.Frame.Height, 320, 50);				
				lineSelector.Hidden = false;
				helpLabel.Hidden = true;
				if(lineMessageShown) return;				
				using(var alert = new UIAlertView("Number of Lines", "Choose the number of lines of profile text that appears beneath your name on your Facebook profile. This helps to accurately crop the profile photos.", null, "OK", null))
				{
					lineMessageShown = true;
					alert.Show();
				}
			}
			else
			{
				LinesSelected(linePickerModel.CurrentText, linePickerModel.CurrentRow);
			}
		}
		
		public void LinesSelected(string text, int lines)
		{
			numberOfLines = lines;			
			lineButton.SetTitle(text, UIControlState.Normal);
			lineSelector.Hidden = true;
			adView.Frame = new RectangleF(0, adView.Frame.Y + lineSelector.Frame.Height, 320, 50);
			UpdateCropHelpers();
		}
		
		public void StartProgress(string title)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			
			hud.SetCaption(title);
			hud.SetActivity(true);
			hud.Show();
			hud.Update();
		}
		
		public void UpdateProgress(string title)
		{
			hud.SetCaption(title);
			hud.Update();
		}
		
		public void StopProgress()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			hud.SetActivity(false);
			hud.Hide();
			uploadedButNotPosted = true;
		}
		
		public void ShowError(NSError error)
		{
			Console.WriteLine("Error: " + error.ToString());
		}
		
		public void GetPhotoFromLibrary()
		{
			helpLabel.Hidden = true;
			picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			PresentModalViewController(picker, true);		
		}
		
		public void GetPhotoFromCamera()
		{
			helpLabel.Hidden = true;
			picker.SourceType = UIImagePickerControllerSourceType.Camera;
			PresentModalViewController(picker, true);
		}
		
		public void SetProfilePicture(string url)
		{
			if(setProfileView == null) setProfileView = new SetProfilePictureView();
			NavigationController.PushViewController(setProfileView, true);
			setProfileView.NavigateTo(url);
		}
		
		public void LoginToFacebook() 
		{	
			//SetProfilePicture("http://www.facebook.com/photo.php?fbid=159589650757706&m2w");
			//return;
			
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
			cropSource6 = CreateCropSource(5, 50, 80, 242);
			cropSource5 = CreateCropSource(94, 82, 43, 30);
			cropSource4 = CreateCropSource(138, 82, 44, 30);			
			cropSource3 = CreateCropSource(183, 82, 43, 30);
			cropSource2 = CreateCropSource(227, 82, 44, 30);
			cropSource1 = CreateCropSource(272, 82, 43, 30);
			
			View.AddSubview(cropSource1);
			View.AddSubview(cropSource2);
			View.AddSubview(cropSource3);
			View.AddSubview(cropSource4);
			View.AddSubview(cropSource5);
			View.AddSubview(cropSource6);		
		}	
		
		private void UpdateCropHelpers()
		{
			float topSmall = 72f + (float)numberOfLines*5.0f;	
						
			UpdateCropSource(cropSource5, topSmall);
			UpdateCropSource(cropSource4, topSmall);
			UpdateCropSource(cropSource3, topSmall);
			UpdateCropSource(cropSource2, topSmall);
			UpdateCropSource(cropSource1, topSmall);
			
			overlayImage = UIImage.FromFile(string.Format("FacebookOverlay{0}.png", numberOfLines));
			facebookOverlay.Image = overlayImage;			
		}
		
		private void UpdateCropSource(UIImageView image, float newTop)
		{
			var oldFrame = image.Frame;
			image.Frame = new RectangleF(oldFrame.X, newTop, oldFrame.Width, oldFrame.Height);
		}
		
		private UIImageView CreateCropSource(float x, float y, float width, float height) 
		{
			var imageView = new UIImageView();
			imageView.Frame = new RectangleF(x, y, width, height);
			return imageView;
		}
			
		public void LoadImage(UIImage image) 
		{		
			image = image.Scale(GetScaledSize(image.Size));
			scrollView.ZoomScale = 1.0f;
			
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
		
		private SizeF GetScaledSize(SizeF orgSize)
		{
			float scale = 0.0f;
			float maxWidth = 1600;
			float maxHeight = 1600;

		    if (orgSize.Width > maxWidth || orgSize.Height > maxHeight)
		    {
		        if (maxWidth/orgSize.Width < maxHeight/orgSize.Height)
		        {
		            scale = maxWidth/orgSize.Width;
		        }
		        else
		        {
		            scale = maxHeight/orgSize.Height;
		        }
				return new SizeF(orgSize.Width*scale, orgSize.Height*scale);
		    }
			
			return orgSize;
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
			// Start context.
			UIGraphics.BeginImageContext(section.Size);
			var ctx = UIGraphics.GetCurrentContext();
			
			// Clear the image with red.
			ctx.SetRGBFillColor(255, 255, 255, 255);
			ctx.FillRect(new RectangleF(new PointF(0, 0), section.Size));
												
			// Setting transform to flip the image.
			var transform = new MonoTouch.CoreGraphics.CGAffineTransform(1, 0, 0, -1, 0, section.Height);
			ctx.ConcatCTM(transform);	
						
			// Drawing the image.
			var drawSource = CreateDrawRectangle(image, section);
			ctx.DrawImage(drawSource, image.CGImage.WithImageInRect(section));			
			
			// Extracting the image and ending.
			var croppedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			
			return croppedImage;
	    }
		
		private RectangleF CreateDrawRectangle(UIImage image, RectangleF section)
		{
			float toWide = image.Size.Width - (section.Left + section.Width);
			float toTall = image.Size.Height - (section.Top + section.Height);
							
			float width = section.Width;
			float height = section.Height;
			float left = section.Left;
			float top = section.Top;
			
			if(toWide < 0) width = section.Width - Math.Abs(toWide);
			if(toTall < 0) height = section.Height - Math.Abs(toTall);
			
			if(section.Top < 0)
			{
				height = section.Height - Math.Abs(section.Top);
				top = 0;
			}
			
			if(section.Left < 0)
			{
				width = section.Width - Math.Abs(section.Left);
				left = 0;
			}
						
			top = section.Top <= 0 ? 0 : section.Height - height;
			left = section.Left >= 0 ? 0 : section.Width - width;
						
			return new RectangleF((float)Math.Ceiling(left), (float)Math.Ceiling(top), (float)Math.Ceiling(width), (float)Math.Ceiling(height));
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
				if(buttonIndex == 0) _mainView.GetPhotoFromCamera();
				else if(buttonIndex == 1) _mainView.GetPhotoFromLibrary();
			}
		}
	}
}