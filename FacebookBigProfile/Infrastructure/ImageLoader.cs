using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Concurrent;
using MonoTouch.UIKit;

namespace FacebookBigProfile
{
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
			if(_requests.TryAdd(request, callback))
			{
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
}

