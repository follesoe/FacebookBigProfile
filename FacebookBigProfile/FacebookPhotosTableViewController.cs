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
		static NSString CellID = new NSString ("MyOtherIdentifier");
		
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
			
			public Album Album { get; private set; }
            		
            public DataSource (FacebookPhotosTableViewController tableViewController, Facebook facebook)
            {	
                _tvc = tableViewController;					
				_facebook = facebook;
				_requestDelegate = new GetPhotosRequestDelegate(_tvc, this);
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
				return Album.Photos.Count;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(CellID);
                if (cell == null)
                {
                    cell = new UITableViewCell (UITableViewCellStyle.Default, CellID);
                }
            				
                cell.TextLabel.Text = Album.Photos[indexPath.Row].Id;
            
                return cell;
            }
        }
		
		class TableDelegate : UITableViewDelegate
        {
            private readonly FacebookPhotosTableViewController _tvc;
			private readonly DataSource _dataSource;

            public TableDelegate (FacebookPhotosTableViewController tableViewController, DataSource dataSource)
            {
                _tvc = tableViewController;		
				_dataSource = dataSource;
            }
            
            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {				
                Console.WriteLine(_dataSource.Album.Photos[indexPath.Row].Id + " selected..");
            }
        }
		
		public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
			Title = "Photos";
			
			_dataSource = new DataSource(this, _facebook);
            TableView.Delegate = new TableDelegate (this, _dataSource);
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
							var imgDict = (NSMutableDictionary)Runtime.GetNSObject(data.ValueAt(j));
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