namespace KontoSimulator
{
    internal class Program
    {
		static void Main(string[] args)
		{
			Thread t = null;
			List<Thread> threads = new List<Thread>();

			for (int i = 0; i < 500; i++) 
			{
				t = new Thread(KontoUpdate);
				threads.Add(t);
				t.Start();
			}

			foreach (Thread thread in threads)
				thread.Join();

			// Erwarteter Wert: 500 * 10000 = 5.000.000
			// Ohne Lock kann TransactionCount kleiner sein (Race Condition)
			Console.WriteLine($"Fertig. Transaktionen: {Konto.TransactionCount} (erwartet: {500 * 10000})");
		}

		static void KontoUpdate() //Random Einzahlungen und Auszahlungen ausführen
		{
			Random random = new Random();
			for (int i = 0; i < 10000; i++)
			{
				int betrag = random.Next(0, 1000);

				if (random.Next() % 2 == 0)
					Konto.Einzahlen(betrag);
				else
					Konto.Auszahlen(betrag);
			}
		}

		public static class Konto
		{
			public static int Kontostand { get; set; } = 0;
			public static int TransactionCount { get; set; } = 0;

			public static object LockFlag = new object(); // ein Lock für beide Operationen — verhindert gleichzeitigen Zugriff auf Kontostand

			public static void Einzahlen(int betrag)
			{
				try
				{
					//Variablen werden gesperrt wenn Thread drauf zugreifen möchte
					//Threads die auf gelockte Blöcke zugreifen wollen müssen warten
					// lock (LockFlag)
					// {
					// 	Kontostand += betrag;
					// 	TransactionCount++;
					// 	Console.WriteLine($"Kontostand: {Kontostand}");
					// }
											Kontostand += betrag;
						TransactionCount++;
						Console.WriteLine($"Kontostand: {Kontostand}");
				}
				catch (Exception) //Wenn 2 Threads zum genau gleichen Zeitpunkt zugreifen wollen
				{
					Console.WriteLine("Deadlock");
				}
			}

			public static void Auszahlen(int betrag)
			{
				try
				{
					// lock (LockFlag)
					// {
					// 	Kontostand -= betrag;
					// 	TransactionCount++;
					// 	Console.WriteLine($"Kontostand: {Kontostand}");
					// }
											Kontostand -= betrag;
						TransactionCount++;
						Console.WriteLine($"Kontostand: {Kontostand}");
				}
				catch (Exception)
				{
					Console.WriteLine("Deadlock");
				}
			}
		}
	}
}