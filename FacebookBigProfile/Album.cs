using System;

namespace FacebookBigProfile
{
	public class Album
	{
		public int Count { get; private set; }
		public string Name { get; private set; }
		public string Id { get; private set; }
		
		public Album(string id, string name, int count)
		{
			Id = id;
			Name = name;
			Count = count;
		}
		
		public override string ToString ()
		{
			return string.Format ("{0} ({1})", Name, Count);
		}
	}
}