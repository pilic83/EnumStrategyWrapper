using EnumStrategyWrapper.Attribute;

namespace EnumStrategyWrapperDemo
{
    [WrapEnum(
        StrategyProperties = new[] {"Bonus", "Award"})]
    public enum Level
    {
        Easy,
        Medium,
        Hard,
        Ultimate
    }
    public class LevelHelpers
    {
        [MemberProperties(
            enumName: "Level", 
            enumMember: "Hard", 
            strategyProperty: "Bonus")]
        object? LevelHardBonus
        {
            get => 10;
        }

        [MemberProperties("Level", "Ultimate","Bonus")]
        object? LevelUltimateBonus
        {
            get => 20;
        }

        [MemberProperties("Level", "Medium", "Award")]
        object? LevelMediumAward
        {
            get => "Chocolate";
        }
        [MemberProperties(
            enumName: "Level", 
            enumMember: "Hard", 
            strategyProperty: "Award")]
        object? LevelHardAward
        {
            get => "Dinner in restaurant";
        }

        [MemberProperties("Level", "Ultimate", "Award")]
        object? LevelUltimateAward
        {
            get => "Medal and dinner in reastaurant";
        }
    }
}
