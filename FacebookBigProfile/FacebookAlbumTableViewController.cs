using System;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FacebookSdk;
using MonoTouch.ObjCRuntime;

namespace FacebookBigProfile
{
    public class FacebookAlbumTableViewController : UITableViewController, IFacebookErrorProvider
    {
        static NSString CellID = new NSString ("MyIdentifier");
		
		private readonly Facebook _facebook;
              
		public FacebookAlbumTableViewController (Facebook facebook)
		{
			_facebook = facebook;
		}
        
        // The data source for our TableView
        class DataSource : UITableViewDataSource
        {
            private readonly FacebookAlbumTableViewController _tvc;
			private readonly Facebook _facebook;
            		
            public DataSource (FacebookAlbumTableViewController tableViewController, Facebook facebook)
            {			
                _tvc = tableViewController;	
				_facebook = facebook;
				
				_facebook.RequestWithGraphPath("/me/albums", new GetAlbumsRequestDelegate(_tvc));
            }
            
            public override int RowsInSection (UITableView tableView, int section)
            {
                return 1;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell (CellID);
                if (cell == null)
                {
                    cell = new UITableViewCell (UITableViewCellStyle.Default, CellID);
                }
            
                // Customize the cell here...
				cell.TextLabel.Text = "Hello World!";
            
                return cell;
            }
        }
    
        // This class receives notifications that happen on the UITableView
        class TableDelegate : UITableViewDelegate
        {
            private readonly FacebookAlbumTableViewController _tvc;

            public TableDelegate (FacebookAlbumTableViewController tableViewController)
            {
                _tvc = tableViewController;
            }
            
            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
                Console.WriteLine ("Row selected {0}", indexPath.Row);
            }
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            TableView.Delegate = new TableDelegate (this);
            TableView.DataSource = new DataSource (this, _facebook);
        }
    
		public void ErrorOccurred (NSError error)
		{
			Console.WriteLine("Error occured!");
		}
	}
	
	public class GetAlbumsRequestDelegate : RequestDelegateBase
	{
		public GetAlbumsRequestDelegate(IFacebookErrorProvider controller) : base(controller)
		{
		}
		
		public override void HandleResult (FBRequest request, NSDictionary dict)
		{			
			var data = (NSArray)dict.ObjectForKey(new NSString("data"));			
							
			for(uint i = 0; i < 6; ++i)
			{	
				var objDict = (NSMutableDictionary)Runtime.GetNSObject(data.ValueAt(i));
				
				var id = objDict.ObjectForKey(new NSString("id"));
				var name = objDict.ObjectForKey(new NSString("name"));
				var count = objDict.ObjectForKey(new NSString("count"));
				
				Console.WriteLine("{0} {1} ({2})", id, name, count);
			}
		}
	}
}