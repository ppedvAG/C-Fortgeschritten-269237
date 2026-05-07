using static Multithreading._06_ThreadMitCallback;

namespace Multithreading
{
	public class _06_ThreadMitCallback
	{
		// Delegate-Typ für die Callback-Methode — wird aufgerufen wenn der Thread fertig ist
		public delegate void CallbackDelegate(ReturnObject r);

		// Hier wird das Ergebnis vom Side Thread gespeichert (über den Callback befüllt)
		private static ReturnObject Obj;

		static void Main(string[] args)
		{
			// ThreadWithReturn bekommt die Callback-Methode "Result" übergeben
			// Sie wird vom Side Thread aufgerufen, sobald er sein Ergebnis bereit hat
			ThreadWithReturn twr = new ThreadWithReturn(new CallbackDelegate(Result));
			Thread t = new Thread(twr.ThreadReturn);
			t.Start();
			t.Join(); // Warten bis der Side Thread den Callback aufgerufen hat

			// Obj ist jetzt befüllt und kann hier verwendet werden
			Console.WriteLine($"Ergebnis: {Obj.Text}, {Obj.Zahl}");
		}

		// Callback-Methode: wird vom Side Thread aufgerufen und läuft in dessen Kontext
		static void Result(ReturnObject o)
		{
			Obj = o; // Ergebnis in der Klassenvariable speichern
		}
	}

	// Hilfsklasse, die den Thread-Code und den Callback kapselt
	public class ThreadWithReturn
	{
		private CallbackDelegate callback; // gespeicherter Verweis auf die Callback-Methode

		public ThreadWithReturn(CallbackDelegate callback)
		{
			this.callback = callback; // Callback beim Erstellen injizieren
		}

		// Diese Methode läuft im Side Thread
		public void ThreadReturn()
		{
			// Ergebnisobjekt erstellen und befüllen
			ReturnObject o = new ReturnObject();
			o.Text = "Test";
			o.Zahl = 5;

			// Callback aufrufen — übergibt das Ergebnis zurück an den Aufrufer
			callback(o);
		}
	}

	// Einfaches Datenübertragungsobjekt (DTO) für das Ergebnis des Threads
	public class ReturnObject
	{
		public string Text { get; set; }

		public int Zahl { get; set; }
	}
}
