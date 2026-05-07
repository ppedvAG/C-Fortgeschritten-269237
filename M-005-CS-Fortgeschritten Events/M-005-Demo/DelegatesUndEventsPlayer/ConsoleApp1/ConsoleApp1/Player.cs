public class Player
{
    public string Name { get; }

    public event EventHandler<LevelUpEventArgs?>? LevelUp;

    public Player(string name)
    {
        Name = name;
    }

    public void AddXp(int xp)
    {
        Console.WriteLine($"{Name} bekommt {xp} XP.");

        if (xp >= 100)
        {
            try
            {
                LevelUp?.Invoke(this, new LevelUpEventArgs(Name));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Auslösen des Level-Up-Events: {ex.Message}");
            }


        }
    }
}

public class LevelUpEventArgs : EventArgs
{
    public string? PlayerName { get; }
    public LevelUpEventArgs(string? playerName)
    {
        PlayerName = playerName;
    }
}