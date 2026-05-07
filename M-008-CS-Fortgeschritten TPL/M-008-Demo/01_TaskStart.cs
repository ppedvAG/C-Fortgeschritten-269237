namespace Multitasking
{
	internal class _01_TaskStart
	{
		static void Main(string[] args)
		{
			Task t = new Task(DoSomething); //Genau wie bei Thread
			t.Start();

			Task t2 = Task.Run(DoSomething); //t.Start() automatisch dabei

			//Mehr Kontrolle über erweiterten Task-Start: 
			// z.B. TaskCreationOptions.LongRunning für CPU-intensive Tasks, die nicht im ThreadPool laufen sollen
			// Scheduler-Optionen, CancellationToken, etc.
			Task t3 = Task.Factory.StartNew(DoSomething); //Genau gleicher Code wie Task.Run

			// Warten bis die Tasks fertig sind (ähnlich wie t.Join() bei Threads)
			t2.Wait(); //t.Join in Tasks (warten bis Task fertig ist)

			for (int i = 0; i < 100; i++)
				Console.WriteLine($"Main Thread {i}");

			Console.ReadKey();
		}

		static void DoSomething()
		{
			for (int i = 0; i < 100; i++)
				Console.WriteLine($"Task {i}");
		}
	}
}