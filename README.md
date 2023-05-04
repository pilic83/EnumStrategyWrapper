# EnumStrategyWrapper
An enumeration strategy wrapper that allows to avoid changing the switch block when you add a new member to an enum.
All you need to do is decorate the enum with the WrapEnum attribute class and define StrategyProperties or StrategyExecutions (or both) as an array of strings containing the property names of properties that have different values for different enum members. Ditto for StrategyExecutions - an array of strings containing method names of methods that have different implementations for different enum members.<br />
```
[WrapEnum(StrategyProperties = new[] { "AccessLevel", "Info"}, StrategyExecutions = new[] { "ShowMessage" })]
public enum User
{
   Standard,
   Author,
   Administrator
}
```
AccessLevel and Info property have different values for different members of User, and ShowMessage has different implementation for different members of User.<br />
Added property and methods we have to add in some Helper class and again decorate added properties and added methods with atribute classes MemberProperties or MemberExecutions. Both have three arguments - enumName, enumMember which is name of enum and name of member of that enum and finaly strategyProperty or strategyExecutions depends of atribute class whitch is name of properti or method from StrategyProperties or StrategyExecutions from WrapEnum. That is how we link properties or method in helpers class with specifications of enum members. <br />
```
[MemberProperties("User", "Author", "AccessLevel")]
object? UserAuthorAccessLevel
{
   get => 1;
}
```
"User" is enum name, "Author" is member name in User enum and "AccessLevel" is added property for User from StrategyProperties array. Now we linked UserAuthorAccessLevel property with AccessLevel property. In other case AccessLevel will return default value.<br />
Pay attention that added property is always object?.<br />
```
[MemberExecutions("User", "Standard", "ShowMessage")]
object? UserStandardShowMessage(params object?[]? args)
{
   if (args is null) return default!;
   if (args.FirstOrDefault() is null) return default!;
   string message = args.FirstOrDefault().ToString();
   Console.WriteLine(message);
   return default!;
}
```
Same as previous - "User" is enum name, "Author" is member name in User enum and "ShowMessage" is added method for User from StrategyExecutions array. Now we linked UserStandardShowMessage method with ShowMessage method. In other case ShowMessage will do nothing and return default value.<br />
Pay attention that added method is always returns object? and has parameter as params object?[]? args .<br />
<br />
Finally after that source generator will do everything for us. We just have to use generated classes.<br />
```
var authorUser = UserStrategy.Author;
userAccessLevel = authorUser.AccessLevel is null ? 0 : (int)authorUser.AccessLevel;
authorUser.ShowMessage(authorUser.Info);
authorUser.ShowMessage($"Author user acess level is {userAccessLevel}.");
```
UserStrategy class wil be generated with source code generator and it contains readonly properties that map enum members, and that properties are classes derived from UserStrategy and they contain added properties and methods that we defined with attributes.
