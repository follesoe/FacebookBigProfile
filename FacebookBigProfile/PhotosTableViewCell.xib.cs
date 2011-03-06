using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading;

namespace FacebookBigProfile
{
	public partial class PhotosTableViewCell : UIViewController
	{
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
		
		public UITableViewCell Cell
		{
			get { return cell; }
		}
		
		public void ShowPhotos(PhotoGroup group)
		{
			_group = group;
			LoadPhoto(0, img1);
			LoadPhoto(1, img2);
			LoadPhoto(2, img3);
			LoadPhoto(3, img4);				
		}
		
		public void LoadPhoto(int number, UIImageView img)
		{
			var photo = _group.GetPhoto(number);
				
			if(photo == null)
			{
				img.Alpha = 0;
				img.UserInteractionEnabled = false;
			}
			else
			{
				img.Alpha = 1;
				img.UserInteractionEnabled = true;
				
				ImageLoader.GetImage(new GetImageRequest(photo.Id, number, new Uri(photo.SmallSource, UriKind.Absolute)), UpdatePhoto);
			}
		}
		
		public void UpdatePhoto(UpdateImage update)
		{
			BeginInvokeOnMainThread(() => {
				if(update.Number == 0) img1.Image = update.Image;
				if(update.Number == 1) img2.Image = update.Image;
				if(update.Number == 2) img3.Image = update.Image;
				if(update.Number == 3) img4.Image = update.Image;
			});
		}

		private void Initialize ()
		{
		}
	}
	
	public class ImageLoader
	{
		static string _cacheDir = string.Empty;
		static ConcurrentDictionary<GetImageRequest, Action<UpdateImage>> _requests;
		static Processor<GetImageRequest> _queue;
		
		static ImageLoader()
		{
			string baseDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "..");
			_cacheDir = Path.Combine (baseDir, "tmp/");
			_queue = new Processor<GetImageRequest>(Download);
			_requests = new ConcurrentDictionary<GetImageRequest, Action<UpdateImage>>();
		}
		
		public static void GetImage(GetImageRequest request, Action<UpdateImage> callback)
		{			
			Console.WriteLine("Get image {0}", request);
			if(_requests.TryAdd(request, callback))
			{
				Console.WriteLine("Queued {0}", request);
				_queue.Queue(request);
			}
		}		

		static void Download (GetImageRequest request)
		{
			var buffer = new byte [4*1024];

			try 
			{
				Action<UpdateImage> callback;
				if(_requests.TryGetValue(request, out callback))
				{
					string filetype = request.Uri.Segments.Last().Split('.').Last();					                                                 
					string target = _cacheDir + request.Id + "." + filetype;
					
					if(!File.Exists(target))
					{
						UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
						using (var file = new FileStream (target, FileMode.Create, FileAccess.Write, FileShare.Read)) 
						{
							var req = WebRequest.Create (request.Uri) as HttpWebRequest;
		
			                using (var resp = req.GetResponse()) 
							using (var s = resp.GetResponseStream()) 
							{
								int n;
								while ((n = s.Read (buffer, 0, buffer.Length)) > 0)
								{
									file.Write (buffer, 0, n);
			                    }
			                }
						}
						UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					}
					
					callback(new UpdateImage(request.Number, request.Uri, UIImage.FromFileUncached(target)));
				}		
			} 
			catch (Exception ex) 
			{
				Console.WriteLine ("Problem with {0} {1}", request.Uri, ex);
			}			
		}
	}
	
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

