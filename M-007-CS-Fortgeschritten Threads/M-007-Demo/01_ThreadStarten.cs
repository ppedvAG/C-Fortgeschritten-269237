namespace Multithreading
{
	internal class _01_ThreadStarten
	{
		static void Main(string[] args)
		{
			Thread t = new Thread(Run); //Funktionszeiger
			t.Start(); //Thread parallel starten

			for (int i = 0; i < 10; i++)
				Console.WriteLine($"Main Thread: {i}");

			t.Join(); //Threads zusammenführen (wieder sequentiell)

		}

		static void Run()
		{
			for (int i = 0; i < 10; i++)
				Console.WriteLine($"Side Thread: {i}");
		}
	}
}