namespace Multithreading
{
	internal class _03_ThreadBeenden
	{
		static void Main(string[] args)
		{
			try
			{
				Thread t = new Thread(Run);
				t.Start(); // Side Thread startet und läuft Run() parallel

				// Main Thread wartet 3 Sekunden, bevor er den Side Thread unterbricht
				Thread.Sleep(3000);

				// Interrupt() setzt ein Flag im Side Thread.
				// Die ThreadInterruptedException wird dort geworfen, sobald
				// der Side Thread den nächsten blockierenden Aufruf erreicht (z.B. Thread.Sleep).
				t.Interrupt();

				// t.Abort() war die alte Methode, einen Thread sofort zu beenden.
				// Sie ist seit .NET 5 deprecated/nicht mehr unterstützt, weil sie
				// den Thread an einer beliebigen Stelle abbricht und keinen sauberen Zustand garantiert.
				//t.Abort();
			}
			catch (ThreadInterruptedException)
			{
				// Dieser catch-Block wird NICHT ausgelöst, weil t.Interrupt()
				// die Exception im Side Thread wirft, nicht im Main Thread.
			}
		}

		static void Run()
		{
			try
			{
				// 100 Iterationen à 100ms = max. 10 Sekunden Laufzeit
				for (int i = 0; i < 100; i++)
				{
					Console.WriteLine(i);
					// Thread.Sleep() ist ein blockierender Aufruf.
					// Genau hier wird die ThreadInterruptedException geworfen,
					// wenn t.Interrupt() vom Main Thread aufgerufen wurde.
					Thread.Sleep(100);
				}
			}
			catch (ThreadInterruptedException)
			{
				// Der Side Thread fängt die Exception selbst und kann hier
				// aufräumen (z.B. Ressourcen freigeben) bevor er endet.
				Console.WriteLine("Thread wurde interrupted");
			}
		}
	}
}
