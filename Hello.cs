using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Scheduler
{
	public delegate IEnumerator Coroutine();
	private List<Tuple<double,Coroutine>> _scheduledTasks = new List<Tuple<double,Coroutine>>();
	
	public void Schedule(double time, Coroutine action)
	{
		_scheduledTasks.Add(new Tuple<double,Coroutine>(time, action));
	}

	public void Run()
	{
		while(_scheduledTasks.Any())
		{
			var sortedTasks = _scheduledTasks.OrderBy(t => t.Item1);
			var nextTask = sortedTasks.First();
			_scheduledTasks.RemoveAt(_scheduledTasks.IndexOf(nextTask));
			var time = nextTask.Item1;
			var timeToExecute = DateTime.Now.AddSeconds(time);
			while(DateTime.Now < timeToExecute) System.Threading.Thread.Sleep(0);
			var coroutine = nextTask.Item2();
			if(coroutine.MoveNext())
			{
				Schedule((double)coroutine.Current, () => coroutine);
			}
		}
	}
}

public class Hello
{
	public static void Main()
	{
		var scheduler = new Scheduler();

		scheduler.Schedule((double)0.0, SomeTask);
		scheduler.Schedule((double)0.25, OtherTask);

		scheduler.Run();
	}
	
	public static IEnumerator SomeTask()
	{
		Console.WriteLine("Hello");
		yield return 0.25;
		Console.WriteLine("World");
		yield return 0.25;
		Console.WriteLine("Print this last");
	}
	
	public static IEnumerator OtherTask()
	{
		Console.WriteLine("Tony And");
		yield break;
	}
}