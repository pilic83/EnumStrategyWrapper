using EnumStrategyWrapper.Attribute;

namespace EnumStrategyWrapperDemo
{
    [WrapEnum(
        StrategyProperties = new[] { "AccessLevel", "Info"}, 
        StrategyExecutions = new[] { "ShowMessage" })]
    public enum User
    {
        Standard,
        Author,
        Administrator
    }

    public class UserHelpers
    {
        [MemberProperties(
            enumName:"User",
            enumMember:"Standard",
            strategyProperty:"AccessLevel")]    
        object? UserStandardAccessLevel
        {
            get => 0;
        }
        [MemberProperties("User", "Author", "AccessLevel")]
        object? UserAuthorAccessLevel
        {
            get => 1;
        }
        [MemberProperties("User", "Administrator", "AccessLevel")]
        object? UserAdminAccessLevel
        {
            get => 2;
        }
        [MemberProperties("User", "Standard", "Info")]
        object? UserStandardInfo
        {
            get => "This is the lowest level of access and is intended for club members.";
        }
        [MemberProperties("User", "Author", "Info")]
        object? UserAuthorInfo
        {
            get => "This is an intermediate level of access and is intended for blog authors.";
        }
        [MemberProperties("User", "Administrator", "Info")]
        object? UserAdminInfo
        {
            get => "This is the highest level of access and is intended for administrators.";
        }

        [MemberExecutions(
            enumName:"User", 
            enumMember:"Standard", 
            strategyExecution:"ShowMessage")]
        object? UserStandardShowMessage(params object?[]? args)
        {
            if (args is null) return default!;
            if (args.FirstOrDefault() is null) return default!;
            string message = args.FirstOrDefault().ToString();
            Console.WriteLine(message);
            return default!;
        }
        [MemberExecutions("User", "Author", "ShowMessage")]
        object? UserAuthorShowMessage(params object?[]? args)
        {
            if (args is null) return default!;
            if (args.FirstOrDefault() is null) return default!;
            string message = args.FirstOrDefault().ToString();
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
            return default!;
        }
        [MemberExecutions("User", "Administrator", "ShowMessage")]
        object? UserAdminShowMessage(params object?[]? args)
        {
            if (args is null) return default!;
            if (args.FirstOrDefault() is null) return default!;
            string message = args.FirstOrDefault().ToString();
            var color = Console.ForegroundColor;
            Console.Write("Administrator message: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
            return default!;
        }
    }
}
