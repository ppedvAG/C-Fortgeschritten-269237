namespace Multitasking
{
	internal class _02b_TaskBeendenMitLeertaste
	{
		static void Main(string[] args)
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			CancellationToken ct = cts.Token;

			Task t = new Task(Print, ct);
			t.Start();

			Console.WriteLine("'A' drücken um Task zu stoppen...");

			// ReadKey(intercept:true) in separatem Task:
			Task.Run(() =>
			{
				ConsoleKeyInfo key = Console.ReadKey(intercept: true);
				if (key.Key == ConsoleKey.A)
				{
					// Abbruchanforderung setzen — ct.IsCancellationRequested wird true
					cts.Cancel();
					Console.WriteLine("\n'A' gedrückt — Task wird gestoppt.");
				}
			});

			// Warten bis der Print-Task wirklich beendet ist
			try
			{
				t.Wait();
			}
			catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
			{
				// OperationCanceledException ist erwartet — Task wurde sauber abgebrochen
				Console.WriteLine("Task sauber beendet.");
			}

			Console.ReadKey();
		}

		static void Print(object token)
		{
			if (token is CancellationToken ct)
			{
				for (int i = 0; i < 100; i++)
				{
					// Kooperativer Check: Task prüft selbst ob Abbruch angefordert wurde
					ct.ThrowIfCancellationRequested(); // wirft OperationCanceledException

					Console.WriteLine($"Task {i}");

					// WaitOne(50): wartet max. 50ms, kehrt aber sofort zurück wenn Abbruch signalisiert wird
					ct.WaitHandle.WaitOne(50);
				}
			}
		}
	}
}
