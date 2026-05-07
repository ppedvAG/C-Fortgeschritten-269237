namespace Multitasking
{
	internal class _05_TaskContinueWith
	{
		static void Main(string[] args)
		{
			Task<int> t = Task.Run(() =>
			{
				return 0;
			});

			// - läuft immer nach t | - task.Result = Ergebnis von t | - nicht-blockierend
			t.ContinueWith(task => Folgetask(task.Result)); 
			// - nur wenn t Exception geworfen hat
			t.ContinueWith(task => Fehlertask(), TaskContinuationOptions); 
			// - nur wenn t erfolgreich war
			t.ContinueWith(task => Erfolgstask(), TaskContinuationOptions.OnlyOnRanToCompletion);

			// Main Thread wird NICHT blockiert — ContinueWith registriert nur Callbacks
			// Alles unterhalb hier läuft sofort weiter, noch bevor t fertig ist
			Console.WriteLine("Main Thread läuft weiter");
		}

		static void Folgetask(int x)
		{

		}

		static void Fehlertask()
		{

		}

		static void Erfolgstask()
		{

		}
	}
}
