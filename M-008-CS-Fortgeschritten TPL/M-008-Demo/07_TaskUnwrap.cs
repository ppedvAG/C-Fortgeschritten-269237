namespace Multitasking
{
	internal class _07_TaskUnwrap
	{
		static void Main(string[] args)
		{
			// Problem: ContinueWith gibt Task<T> zurück.
			// Wenn der Folge-Task selbst auch einen Task startet, entsteht Task<Task<T>> —
			// eine doppelte Verschachtelung, bei der man zweimal warten müsste.
			Task<Task<int>> verschachtelt = null;
			// Unwrap() "flacht" Task<Task<int>> zu Task<int> ab — nur noch eine Warteebene
			Task<int> einzeln = verschachtelt.Unwrap();

			// Schritt 1: Daten asynchron laden (gibt byte[] zurück)
			Task<byte[]> data = Task.Run(GetData);

			// Schritt 2: Wenn GetData fertig ist, starte Compute in einem neuen Task.
			// ContinueWith gibt Task<T> zurück — da Compute selbst ein Task.Run ist,
			// entsteht Task<Task<byte>> (verschachtelt)
			Task<Task<byte>> schritt2 = data.ContinueWith(t => Task.Run(() => Compute(t.Result)));

			// Unwrap() reduziert Task<Task<byte>> → Task<byte>
			// .Result blockiert den Main Thread bis das Endergebnis da ist
			byte b = schritt2.Unwrap().Result;

			Console.WriteLine($"Finales Ergebnis: {b:x}");
		}

		private static byte[] GetData()
		{
			Random rand = new Random();
			byte[] bytes = new byte[64];
			rand.NextBytes(bytes);
			return bytes;
		}

		static byte Compute(byte[] data)
		{
			byte final = 0;
			foreach (byte item in data)
			{
				final ^= item;
				Console.WriteLine("{0:x}", final);
			}
			Console.WriteLine("Done computing");
			return final;
		}
	}
}
