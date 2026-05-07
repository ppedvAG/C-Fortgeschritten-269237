
//Variante 1: Action als Parameter, da keine Rückgabe, sonst Func
void ForEach<T>(IEnumerable<T> values, Action<T> action)
{
	if (values == null || action == null)
		throw new ArgumentNullException();

	foreach (T item in values) 
		action?.Invoke(item);
}

//Variante 2: Mit Delegate mehr Schreibarbeit, aber mehr Kontrolle
void ForEachWithDelegate<T>(IEnumerable<T> values, ItemDelegate<T> action)
{
	if (values == null || action == null)
		throw new ArgumentNullException();

	foreach (T item in values)
		action(item);
}

//Variante 3: Rückgabewert mit Func statt Action, hier wird eine neue Liste mit den Rückgabewerten erstellt
IEnumerable<TReturn> ForEachReturn<T, TReturn>(IEnumerable<T> values, Func<T, TReturn> func)
{
	if (values == null || func == null)
		throw new ArgumentNullException();

	List<TReturn> ret = [];
	foreach (T item in values)
	{
		TReturn r = func(item);
		ret.Add(r);
	}
	return ret;
}


List<int> zahlen = [1, 2, 3, 4, 5];
IEnumerable<string> r = ForEachReturn(zahlen, e => "hallo");

ForEach(r, e => Console.WriteLine(e));



// // ForEachWithDelegate(zahlen, e => Console.WriteLine(e));
delegate void ItemDelegate<T>(T item);
