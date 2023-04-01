namespace EnumStrategyWrapper.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class MemberExecutionsAttribute : System.Attribute
    {
        public string EnumName;
        public string EnumMember;
        public string StrategyExecution;
        public MemberExecutionsAttribute(string enumName, string enumMember, string strategyExecution)
        {
            EnumName = enumName;
            EnumMember = enumMember;
            StrategyExecution = strategyExecution;
        }
    }
}
