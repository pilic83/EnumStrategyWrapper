using EnumStrategyWrapperDemo;

var standardUser = UserStrategy.Standard;
int userAccessLevel = standardUser.AccessLevel is null ? 0 : (int)standardUser.AccessLevel;
standardUser.ShowMessage(standardUser.Info);
standardUser.ShowMessage($"Standard user acess level is {userAccessLevel}.");

var authorUser = UserStrategy.Author;
userAccessLevel = authorUser.AccessLevel is null ? 0 : (int)authorUser.AccessLevel;
authorUser.ShowMessage(authorUser.Info);
authorUser.ShowMessage($"Author user acess level is {userAccessLevel}.");

var adminUser = UserStrategy.Administrator;
userAccessLevel = adminUser.AccessLevel is null ? 0 : (int)adminUser.AccessLevel;
adminUser.ShowMessage(adminUser.Info);
adminUser.ShowMessage($"Administrator user acess level is {userAccessLevel}.");

var contestantEasy = LevelStrategy.Easy;
int bonus = contestantEasy.Bonus is null ? 0 : (int)contestantEasy.Bonus;
string award = contestantEasy.Award is null ? "none" : contestantEasy.Award.ToString();
Console.WriteLine($"Easy mode - Bonus: {bonus}$, Award: {award}");

var contestantMedium = LevelStrategy.Medium;
bonus = contestantMedium.Bonus is null ? 0 : (int)contestantMedium.Bonus;
award = contestantMedium.Award is null ? "none" : contestantMedium.Award.ToString();
Console.WriteLine($"Medium mode - Bonus: {bonus}$, Award: {award}");

var contestantHard =  LevelStrategy.Hard;
bonus = contestantHard.Bonus is null ? 0 : (int)contestantHard.Bonus;
award = contestantHard.Award is null ? "none" : contestantHard.Award.ToString();
Console.WriteLine($"Hard mode - Bonus: {bonus}$, Award: {award}");

var contestantUltimate = LevelStrategy.Ultimate;
bonus = contestantUltimate.Bonus is null ? 0 : (int)contestantUltimate.Bonus;
award = contestantUltimate.Award is null ? "none" : contestantUltimate.Award.ToString();
Console.WriteLine($"Ultimate mode - Bonus: {bonus}$, Award: {award}");

Console.ReadLine();