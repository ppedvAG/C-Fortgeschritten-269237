public class AchievementSystem
{
    public void UnlockLevelUpAchievement(object sender, LevelUpEventArgs? args)
    {
        Console.WriteLine($"Erster Level-Up - {sender?.PlayerName}");

    }
}