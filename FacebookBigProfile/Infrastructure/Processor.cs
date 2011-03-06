using System;
using System.Threading;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace FacebookBigProfile
{
	public class Processor<T> 
	{
	
	    private Action<T> action;
	    private Queue<T> queue = new Queue<T>();
	
	    public Processor(Action<T> action) 
		{
	        this.action = action;
	        new Thread(new ThreadStart(ThreadProc)).Start();
	    }
	
	    public void Queue(T data) 
		{
	        lock (queue) 
			{
	            queue.Enqueue(data);
	            Monitor.Pulse(queue); 
	        }	
	    }
		
	    private void ThreadProc() 
		{	
			NSAutoreleasePool pool = new NSAutoreleasePool();
	        Monitor.Enter(queue);
	        Queue<T> copy;
	
	        while (true) 
			{
	
	            if (queue.Count == 0) 
				{
	                Monitor.Wait(queue);
	            }
	
	            copy = new Queue<T>(queue);
	            queue.Clear();
	            Monitor.Exit(queue);
		            
				foreach (var item in copy) 
				{
	                action(item); 
	            }
	
	            Monitor.Enter(queue); 
	        }
	    }
	}
}