namespace Multitasking
{
	internal class _04_TaskExceptions
	{
		static void Main(string[] args)
		{
			Task t1, t2, t3, t4;
			try
			{
				t1 = Task.Run(Exception1);
				t2 = Task.Run(Exception2);
				t3 = Task.Run(Exception3);
				t4 = Task.Run(KeineException);

				Task.WaitAny(t1, t2, t3, t4);

				// Diese Blöcke werden NIE erreicht — WaitAll wirft AggregateException
				// und springt direkt in catch wenn mind. ein Task gefailed ist
				if (t1.IsFaulted) { Console.WriteLine("t1 hat Exception geworfen"); }

				if (t2.IsCanceled) { Console.WriteLine("t2 wurde abgebrochen"); }

				if (t3.IsCompleted) { Console.WriteLine("t3 ist fertig (egal ob Fehler)"); }

				if (t4.IsCompletedSuccessfully) { Console.WriteLine("t4 war erfolgreich"); }
			}
			catch (AggregateException ex) // Sammelt alle Exceptions aller Tasks
			{
				foreach (Exception e in ex.InnerExceptions) // jede einzelne Exception ausgeben
					Console.WriteLine(e.Message);
			}

			// Main Thread läuft nach dem catch normal weiter
			Console.WriteLine("Exception handling abgeschlossen — Main Thread läuft weiter");
		}

		public static void Exception1()
		{
			Thread.Sleep(1000);
			throw new DivideByZeroException();
		}

		public static void Exception2()
		{
			Thread.Sleep(2000);
			throw new StackOverflowException();
		}

		public static void Exception3()
		{
			Thread.Sleep(3000);
			throw new OutOfMemoryException();
		}

		public static void KeineException()
		{
			Console.WriteLine("Alles OK");
		}
	}
}
