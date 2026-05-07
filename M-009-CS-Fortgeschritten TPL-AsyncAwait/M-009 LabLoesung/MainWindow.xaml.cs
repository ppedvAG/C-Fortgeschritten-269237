using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TaskTest
{
	public partial class MainWindow : Window
	{
		private CancellationTokenSource _cts;

		public MainWindow() => InitializeComponent();

		private async void SaveSplitJson(object sender, EventArgs e)
		{
			_cts = new CancellationTokenSource(); //Neues Token für jeden Start
			using StreamReader sr = new StreamReader("history.city.list.min.json");
			JsonDocument jd = await JsonDocument.ParseAsync(sr.BaseStream);
			//StreamReader erstellen und BaseStream davon als Stream-Parameter hier benutzen
			//Json File einlesen mit await um UI-Freeze zu verhindern

			if (Directory.Exists("Lab"))
				Directory.Delete("Lab", true); //Übung zurücksetzen
			Directory.CreateDirectory("Lab");

			ConcurrentDictionary<string, List<JsonElement>> cities = new();
			foreach (JsonElement je in jd.RootElement.EnumerateArray()) //Json Datei iterieren
			{
				string countryCode = je.GetProperty("city").GetProperty("country").GetString(); //Auf den CountryCode zugreifen (AT, DE, IT, ...)
				if (!cities.ContainsKey(countryCode))
					cities.TryAdd(countryCode, new());
				cities[countryCode].Add(je);
			}

			//Dictionary<string, List<JsonElement>> linqCities = jd.RootElement.EnumerateArray()
			//	.GroupBy(je => je.GetProperty("city").GetProperty("country").GetString())
			//	.ToDictionary(k => k.Key, v => v.ToList());

			//Ohne Parallel, parallelisierung per Hand
			Progress.Maximum = cities.Count;

			try
			{
				//Parallel 1:
				//List<Task> writeTasks = new();
				//foreach (KeyValuePair<string, List<JsonElement>> kv in cities) //Json Files schreiben
				//	writeTasks.Add(WriteFileAndUpdateUI(kv.Key, JsonListToJson(kv.Value), _cts.Token));
				//await Task.WhenAll(writeTasks);

				//Sequenziell:
				foreach (KeyValuePair<string, List<JsonElement>> kv in cities)
					await WriteFileAndUpdateUI(kv.Key, JsonListToJson(kv.Value), _cts.Token);
			}
			catch (OperationCanceledException)
			{
				// Wird geworfen wenn _cts.Cancel() aufgerufen wird – kein Fehler, sondern gewolltes Abbrechen
				WriteText("Abgebrochen.");
				Progress.Value = 0;
			}

			//Parallel For each mit MaxDegreeOfParallelism:
			 Parallel.ForEach(
				cities, 
				new ParallelOptions { MaxDegreeOfParallelism = 4 }, async 
				(kv, ct) => await WriteFileAndUpdateUI(kv.Key, JsonListToJson(kv.Value),
                _cts.Token));

        }
		
		private async Task WriteFileAndUpdateUI(string path, string content, CancellationToken ct)
		{
			await File.WriteAllTextAsync(Path.Combine("Lab", $"{path}.json"), content, ct);
			// Nach dem await läuft der Code auf einem Thread-Pool-Thread weiter.
			// UI-Elemente (ProgressBar, TextBox) dürfen in WPF nur vom UI-Thread
			// angefasst werden – daher Dispatcher.Invoke, um zurück auf den UI-Thread zu wechseln.
			Dispatcher.Invoke(() =>
			{
				WriteText($"{path} in die Datei geschrieben");
				Progress.Value++;
			});
		}

		private void Cancel(object sender, EventArgs e)
		{
			_cts?.Cancel(); //CancellationToken signalisieren -> alle laufenden Tasks brechen ab
		}

		/// <summary>
		/// Verwende diese Methode, um einen Text in der TextBox anzuzeigen.
		/// </summary>
		/// <param name="text"></param>
		private void WriteText(string text)
		{
			Dispatcher.Invoke(() =>
			{
				Output.Text += text;
				Output.Text += Environment.NewLine;
				Scroll.ScrollToEnd();
			});
		}

		//Es gibt keine Methode um aus einer Liste von JsonElements ein JsonArray zu generieren
		private string JsonListToJson(List<JsonElement> jsons)
		{
			return jsons.Aggregate(new StringBuilder("[\n"), (sb, je) =>
				sb.Append('\t')
				  .Append(je.GetRawText())
				  .Append(",\n"))
				  .ToString()
				  .TrimEnd(',', '\n') + "\n]";
		}
	}
}
