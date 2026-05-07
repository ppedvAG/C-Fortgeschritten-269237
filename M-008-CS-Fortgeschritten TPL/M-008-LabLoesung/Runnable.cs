using System.Diagnostics;

namespace TPL_Uebung;

public abstract class Runnable
{
	private bool _continue = false;

	public bool Continue
	{
		get => _continue;
		set
		{
			_continue = value;

			if (value && CurrentTask.Status == TaskStatus.Created)
				CurrentTask.Start(); //Startet beim ersten Mal

			if (value && CurrentTask.Status == TaskStatus.RanToCompletion)
				CurrentTask = new Task(Run); //Task neu erstellen wenn er schon durchgelaufen ist
		}
	}

	public Task CurrentTask { get; protected set; }

	protected private abstract void Run();
}