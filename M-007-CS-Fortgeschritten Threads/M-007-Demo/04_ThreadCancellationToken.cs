namespace Multithreading
{
	internal class _04_ThreadCancellationToken
	{
		static void Main(string[] args)
		{
			// CancellationTokenSource ist der "Schalter" — nur sie kann den Abbruch auslösen
			CancellationTokenSource cts = new CancellationTokenSource();
			// CancellationToken ist das "Ticket" für den Thread — nur lesend, kein Abbruch möglich
			CancellationToken ct = cts.Token;

			// ParameterizedThreadStart erlaubt, einen object-Parameter an den Thread zu übergeben
			ParameterizedThreadStart pt = new ParameterizedThreadStart(Run);
			Thread t = new Thread(pt);
			t.Start(ct); // Token als Parameter an den Side Thread übergeben

			Thread.Sleep(2000); // Main Thread wartet 2 Sekunden
			cts.Cancel(); // Abbruchanforderung setzen — ct.IsCancellationRequested wird true
		}

		static void Run(object o)
		{
			try
			{
				// Cast von object zurück zu CancellationToken (Boxing/Unboxing)
				if (o is CancellationToken ct)
				{
					for (int i = 0; i < 100; i++)
					{
						// Kooperativer Check: Thread prüft selbst ob Abbruch angefordert wurde
						if (ct.IsCancellationRequested)
							ct.ThrowIfCancellationRequested(); // Wirft OperationCanceledException
						Console.WriteLine(i);

						Thread.Sleep(100);
					}
				}
			}
			catch (OperationCanceledException) // Exception landet hier im Side Thread, nicht im Main Thread
			{
				// Hier kann sauber aufgeräumt werden (Ressourcen freigeben etc.)
				Console.WriteLine("Thread wurde mit Token beendet");
			}
		}
	}
}
