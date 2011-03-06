using System;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

using FacebookSdk;

namespace FacebookBigProfile
{
    public class FacebookAlbumTableViewController : UITableViewController, IFacebookErrorProvider
    {
        static NSString CellID = new NSString ("PhotoCell");
		
		private readonly Facebook _facebook;
		private readonly FacebookPicturePicker _picker;
		private DataSource _dataSource;
		
		public FacebookAlbumTableViewController (IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public FacebookAlbumTableViewController (NSCoder coder) : base(coder)
		{			
		}
              
		public FacebookAlbumTableViewController (Facebook facebook, FacebookPicturePicker picker)
		{
			_facebook = facebook;
			_picker = picker;
		}
        
        class DataSource : UITableViewDataSource
        {
            private readonly FacebookAlbumTableViewController _tvc;
			private readonly Facebook _facebook;
			private readonly GetAlbumsRequestDelegate _requestDelegate;
			
			public List<Album> Albums { get; private set; }
            		
            public DataSource (FacebookAlbumTableViewController tableViewController, Facebook facebook)
            {	
				Albums = new List<Album>();
				
                _tvc = tableViewController;					
				_facebook = facebook;		
				
				_requestDelegate = new GetAlbumsRequestDelegate(_tvc, this);
            }
			
			public void GetAlbums()
			{
				_facebook.RequestWithGraphPath("/me/albums", _requestDelegate);
			}
			
			public void ShowAlbums(List<Album> albums)
			{
				Albums.Clear();
				Albums.AddRange(albums);				
				_tvc.GotAlbums();
			}
            
            public override int RowsInSection(UITableView tableView, int section)
            {
                return Albums.Count;			
            }
			
            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {				
                var cell = tableView.DequeueReusableCell(CellID);
                if (cell == null)
                {					
                    cell = new UITableViewCell (UITableViewCellStyle.Default, CellID);
                }
            				
                cell.TextLabel.Text = Albums[indexPath.Row].ToString();
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            
                return cell;
            }
        }
		
		class TableDelegate : UITableViewDelegate
        {
            private readonly FacebookAlbumTableViewController _tvc;
			private readonly DataSource _dataSource;

            public TableDelegate (FacebookAlbumTableViewController tableViewController, DataSource dataSource)
            {
                _tvc = tableViewController;		
				_dataSource = dataSource;
            }
            
            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {				
                _tvc.AlbumSelected(_dataSource.Albums[indexPath.Row]);
            }
        }
		
		public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
			
			_dataSource = new DataSource(this, _facebook);
			TableView.Delegate = new TableDelegate (this, _dataSource);
			TableView.DataSource = _dataSource;            
        }
		
	 	class GetAlbumsRequestDelegate : RequestDelegateBase
		{
			private readonly DataSource _dataSource;
			
			public GetAlbumsRequestDelegate(IFacebookErrorProvider controller, DataSource dataSource) : base(controller)
			{
				_dataSource = dataSource;
			}
			
			public override void HandleResult (FBRequest request, NSDictionary dict)
			{	
				try
				{
					var data = (NSArray)dict.ObjectForKey(new NSString("data"));			
							
					var albums = new List<Album>();
					for(uint i = 0; i < data.Count; ++i)
					{	
						var objDict = (NSMutableDictionary)Runtime.GetNSObject(data.ValueAt(i));
						
						var id = objDict.ObjectForKey(new NSString("id"));
						var name = objDict.ObjectForKey(new NSString("name"));
						var count = (NSNumber)objDict.ObjectForKey(new NSString("count"));
						
						albums.Add(new Album(id.ToString(), name.ToString(), count.Int32Value));
					}
					
					_dataSource.ShowAlbums(albums);
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}
		
		public void GetAlbums()
		{
			_dataSource.GetAlbums();	
		}
		
		public void GotAlbums()
		{
			TableView.ReloadData();
		}
		
		public void AlbumSelected(Album album)
		{
			_picker.AlbumSelected(album);
		}
    
		public void ErrorOccurred (NSError error)
		{
			Console.WriteLine("Error occured!");
		}
	}
}