namespace Multitasking
{
	internal class _03_TaskMitReturn
	{
		static void Main(string[] args)
		{
			Task<int> t = Task.Run(SumI);
			//Console.WriteLine(t.Result); //.Result blockt den Main Thread (t.Wait())
			int start = 5;
			Task<int> t2 = Task.Run((start) => //Funktion als Lambda Expression
			{
				start = 10; //Lambda Expression kann auf lokale Variablen zugreifen (Closure)
				int summe = start;
				for (int i = 0; i < 1000; i++)
					summe += i;
				Thread.Sleep(500);
				return summe;
			});

			Task.WaitAll(t, t2); //Warte auf alle angegebenen Tasks
			Task.WaitAny(t, t2); //Warte auf einen Task, return Wert gibt den Index des Tasks zurück
		}

		static int SumI()
		{
			int summe = 0;
			for (int i = 0; i < 1000; i++)
				summe += i;
			Thread.Sleep(500);
			return summe;
		}
	}
}
