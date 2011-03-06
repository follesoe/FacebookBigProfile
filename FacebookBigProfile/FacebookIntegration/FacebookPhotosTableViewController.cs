using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FacebookSdk;
using System.Collections.Generic;
using MonoTouch.ObjCRuntime;

namespace FacebookBigProfile
{
	public class FacebookPhotosTableViewController : UITableViewController, IFacebookErrorProvider
	{
		static NSString CellID = new NSString ("PhotoCell");
		
		private readonly Facebook _facebook;
		private readonly FacebookPicturePicker _picker;
		private DataSource _dataSource;		
              
		public FacebookPhotosTableViewController (Facebook facebook, FacebookPicturePicker picker)
		{
			_facebook = facebook;
			_picker = picker;
		}
        
        class DataSource : UITableViewDataSource
        {
            private readonly FacebookPhotosTableViewController _tvc;
			private readonly Facebook _facebook;
			private readonly GetPhotosRequestDelegate _requestDelegate;
			private readonly Dictionary<int, PhotosTableViewCell> _cellControllers;
			private readonly Random _random;
			
			public Album Album { get; private set; }
            		
            public DataSource (FacebookPhotosTableViewController tableViewController, Facebook facebook)
            {	
                _tvc = tableViewController;					
				_facebook = facebook;
				_cellControllers = new Dictionary<int, PhotosTableViewCell>();
				_requestDelegate = new GetPhotosRequestDelegate(_tvc, this);
				_random = new Random(Environment.TickCount);
            }
			
			public void LoadFromAlbum(Album album)
			{
				Console.WriteLine("Load photos from " + album.Name);
				
				Album = album;											
				_facebook.RequestWithGraphPath(string.Format("/{0}/photos", Album.Id), _requestDelegate);
			}
			
			public void AddPhotos(List<Photo> photos)
			{
				Album.AddPhotos(photos);
				_tvc.TableView.ReloadData();
			}		
            
            public override int RowsInSection(UITableView tableView, int section)
            {
                if(Album == null) return 0;
				return Album.NumberOfGroups;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
				PhotosTableViewCell cellController = null;
				
                var cell = tableView.DequeueReusableCell(CellID);
                if (cell == null)
                {
					cellController = new PhotosTableViewCell();
					cellController.ImageSelected += (img) => _tvc.PhotoSelected(img);
                    cell = cellController.Cell;
					cell.Tag = Environment.TickCount + _random.Next(0, 1000);
					_cellControllers.Add(cell.Tag, cellController);					
                }
				else
				{
					cellController = _cellControllers[cell.Tag];
				}
				
				cellController.ShowPhotos(Album.GetGroup(indexPath.Row));
            				            
                return cell;
            }
        }
		
		class TableDelegate : UITableViewDelegate
        {
            public TableDelegate ()
            {
            }
			
			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				return 81f;
			}
            
            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {				
            }
        }
		
		public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
			Title = "Photos";
			
			_dataSource = new DataSource(this, _facebook);
            TableView.Delegate = new TableDelegate();
            TableView.DataSource = _dataSource;
        }
		
	 	class GetPhotosRequestDelegate : RequestDelegateBase
		{
			private DataSource _dataSource;
			
			public GetPhotosRequestDelegate(IFacebookErrorProvider controller, DataSource dataSource) : base(controller)
			{
				_dataSource = dataSource;
			}
			
			public override void HandleResult (FBRequest request, NSDictionary dict)
			{	
				try
				{
					var data = (NSArray)dict.ObjectForKey(new NSString("data"));
							
					var photos = new List<Photo>();
					for(uint i = 0; i < data.Count; ++i)
					{	
						var objDict = (NSMutableDictionary)Runtime.GetNSObject(data.ValueAt(i));
						
						var id = objDict.ObjectForKey(new NSString("id")).ToString();
						var source = objDict.ObjectForKey(new NSString("source")).ToString();
						
						string smallesSource = string.Empty;						
						uint smallestSize = uint.MaxValue;	
						
					 	var photoArray = (NSArray)objDict.ObjectForKey(new NSString("images"));
						for(uint j = 0; j < photoArray.Count; ++j)
						{		
							var imgDict = (NSMutableDictionary)Runtime.GetNSObject(photoArray.ValueAt(j));
							var size = (NSNumber)imgDict.ObjectForKey(new NSString("height"));							
							if(smallestSize > size.UInt32Value)
							{
								smallestSize = size.UInt32Value;
								smallesSource = imgDict.ObjectForKey(new NSString("source")).ToString();							
							}
						}
						photos.Add(new Photo(id, source, smallesSource));
					}
					
					_dataSource.AddPhotos(photos);
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}
		
		public void PhotoSelected(string url)
		{
			_picker.PhotoSelected(url);
		}
		
		public void LoadFromAlbum(Album album)
		{
			Title = album.Name;
			_dataSource.LoadFromAlbum(album);	
		}
		
		public void ErrorOccurred (NSError error)
		{
			Console.WriteLine("Error occured!");
		}
	}
}