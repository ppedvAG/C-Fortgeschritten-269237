var player = new Player("Arjan");

var achievements = new AchievementSystem();
var sound = new SoundSystem();

player.LevelUp += achievements.UnlockLevelUpAchievement;



player.AddXp(50);


player.AddXp(120);

player.LevelUp -= achievements.UnlockLevelUpAchievement;