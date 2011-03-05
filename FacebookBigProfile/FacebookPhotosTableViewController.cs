using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FacebookSdk;

namespace FacebookBigProfile
{
	public class FacebookPhotosTableViewController : UITableViewController, IFacebookErrorProvider
	{
		static NSString CellID = new NSString ("MyOtherIdentifier");
		
		private readonly Facebook _facebook;
		private readonly FacebookPicturePicker _picker;
              
		public FacebookPhotosTableViewController (Facebook facebook, FacebookPicturePicker picker)
		{
			_facebook = facebook;
			_picker = picker;
		}
        
        class DataSource : UITableViewDataSource
        {
            private readonly FacebookPhotosTableViewController _tvc;
			private readonly Facebook _facebook;
			
			public Album Album { get; private set; }
            		
            public DataSource (FacebookPhotosTableViewController tableViewController, Facebook facebook)
            {	

                _tvc = tableViewController;	
				
				_facebook = facebook;								
				//_facebook.RequestWithGraphPath("/me/albums", new GetAlbumsRequestDelegate(_tvc, this));
            }
			
			public void ShowAlbum(Album album)
			{
				Album = album;
				_tvc.TableView.ReloadData();
			}
            
            public override int RowsInSection(UITableView tableView, int section)
            {
                return 0;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(CellID);
                if (cell == null)
                {
                    cell = new UITableViewCell (UITableViewCellStyle.Default, CellID);
                }
            				
                cell.TextLabel.Text = "TODO";
            
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
                // TODO
            }
        }
		
		public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
			
			var dataSource = new DataSource(this, _facebook);
            TableView.Delegate = new TableDelegate (this, dataSource);
            TableView.DataSource = dataSource;
        }
		
		/*
	 	class GetAlbumsRequestDelegate : RequestDelegateBase
		{
			private DataSource _dataSource;
			
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
		}*/
		
		public void ErrorOccurred (NSError error)
		{
			Console.WriteLine("Error occured!");
		}
	}
}