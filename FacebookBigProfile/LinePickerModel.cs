using System;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace FacebookBigProfile
{
	public class LinePickerModel : UIPickerViewModel
	{
		private readonly List<string> _options;
		private readonly MainView _mainView;
		public int CurrentRow;
		
		public string CurrentText
		{
			get { return _options[CurrentRow]; }
		}
		
		public LinePickerModel(MainView mainView)
		{
			_mainView = mainView;
			_options = new List<string>();
			_options.Add("No lines");
			_options.Add("One line");
			_options.Add("Two lines (most common)");
			_options.Add("Three lines");
			_options.Add("Four lines");
			_options.Add("Five lines");
		}
		
		public override int GetComponentCount (UIPickerView picker)
		{
			return 1;
		}
		
		public override int GetRowsInComponent (UIPickerView picker, int component)
		{
			return _options.Count;
		}
		
		public override string GetTitle (UIPickerView picker, int row, int component)
		{
			return _options[row];
		}
		
		public override void Selected (UIPickerView picker, int row, int component)
		{
			CurrentRow = row;
		}		
	}
}

