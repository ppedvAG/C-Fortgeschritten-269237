namespace Multitasking
{
	internal class _06_TaskWhenAll
	{
		static void Main(string[] args)
		{
			// - Liste mit 1000 Tasks erstellen
			// - jeder Task berechnet das Quadrat von i und gibt int zurück
			List<Task<int>> tasks = new();
			for (int i = 0; i < 1000; i++)
				tasks.Add(Task.Run(() =>
				{
					return Square(i);
				}));

			// - Task.WhenAll wartet bis ALLE Tasks fertig sind
			// - gibt Task<int[]> zurück — Ergebnisse aller Tasks als Array
			// - nicht-blockierend (im Gegensatz zu Task.WaitAll)
			Task<int[]> ergebnisse = Task.WhenAll(tasks);

			// - .Result blockiert den Main Thread bis WhenAll fertig ist
			// - ergebnis[i] = Quadrat von i
			int[] ergebnis = ergebnisse.Result;

			// Diese Zeile kommt erst NACH allen Tasks — .Result hat blockiert
			// Ohne .Result würde sie sofort kommen, noch bevor Tasks fertig sind
			Console.WriteLine($"Alle Tasks fertig. Ergebnisse: {ergebnis.Length}");
		}

		static int Square(int x)
		{
			return x * x;
		}
	}
}
