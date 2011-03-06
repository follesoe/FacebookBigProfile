using System;
using System.Collections.Generic;

namespace FacebookBigProfile
{
	public class Album
	{
		public int Count { get; private set; }
		public string Name { get; private set; }
		public string Id { get; private set; }
		public List<Photo> Photos { get; private set; }
		
		public int NumberOfGroups
		{
			get 
			{
				return Convert.ToInt32(Math.Ceiling((double)Photos.Count / 4f));
			}
		}	
		
		public PhotoGroup GetGroup(int rowIndex)
		{
			int startIndex = rowIndex * 4;
			int endIndex = Math.Min(Photos.Count - startIndex, 4);			
			
			Console.WriteLine("{0} til {1}", startIndex, endIndex);
			
			var photos = Photos.GetRange(startIndex, endIndex);
			return new PhotoGroup(photos.ToArray());
		}
		
		public Album(string id, string name, int count)
		{
			Id = id;
			Name = name;
			Count = count;
			Photos = new List<Photo>();
		}
		
		public void AddPhotos(List<Photo> photos)
		{
			Photos.AddRange(photos);
		}
		
		public override string ToString ()
		{
			return string.Format ("{0} ({1})", Name, Count);
		}
	}
	
	public class Photo
	{
		public string Id { get; private set; }
		public string Source { get; private set; }
		public string SmallSource { get; private set; }
		
		public Photo(string id, string source, string smallSource)
		{
			Id = id;
			Source = source;
			SmallSource = smallSource;
		}
	}
	
	public class PhotoGroup
	{
		private Photo[] _photos;
					
		public Photo GetPhoto(int i)
		{
			if(_photos.Length > i)
				return _photos[i];
			else return null;
		}
		
		public int Length 
		{
			get { return _photos.Length; }
		}
		
		public PhotoGroup(params Photo[] photos)
		{
			_photos = photos;
		}
	}
}