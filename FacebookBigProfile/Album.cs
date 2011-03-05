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
}