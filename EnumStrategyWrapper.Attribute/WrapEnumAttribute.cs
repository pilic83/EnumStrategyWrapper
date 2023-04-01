namespace EnumStrategyWrapper.Attribute
{
    [AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public sealed class WrapEnumAttribute : System.Attribute
    {
        public string[]? StrategyExecutions;
        public string[]? StrategyProperties;
        public WrapEnumAttribute() 
        {
        }
    }
}