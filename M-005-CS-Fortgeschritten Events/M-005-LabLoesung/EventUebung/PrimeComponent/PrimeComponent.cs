public class PrimeComponent
{
    public event EventHandler<int>? Prime;
    public event EventHandler<int>? Prime100;
    public event EventHandler<(int, int)>? NotPrime;

    public void StartProcess()
    {
        Prime?.Invoke(this, 2);
        for (int i = 3, counter = 0; ; i += 2)
        {
            if (CheckPrime(i))
            {
                counter++;
                if (counter % 100 != 0)
                    Prime?.Invoke(this, i);
                else
                    Prime100?.Invoke(this, i);
            }
            Thread.Sleep(50);
        }
    }

    public bool CheckPrime(int num)
    {
        if (num % 2 == 0)
        {
            NotPrime?.Invoke(this, (num, 2));
            return false;
        }

        for (int i = 3; i <= num / 2; i += 2) //Nur bis zu Hälfte gehen, da größere Zahlen nicht teilbar sein können
        {
            if (num % i == 0)
            {
                NotPrime?.Invoke(this, (num, i));
                return false;
            }
        }
        return true;
    }
}