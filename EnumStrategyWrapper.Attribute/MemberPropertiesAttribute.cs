using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumStrategyWrapper.Attribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MemberPropertiesAttribute : System.Attribute
    {
        public string EnumName;
        public string EnumMember;
        public string StrategyProperty;
        public MemberPropertiesAttribute(string enumName, string enumMember, string strategyProperty )
        {
            EnumName = enumName;
            EnumMember = enumMember;
            StrategyProperty = strategyProperty;
        }
    }
}
