using System.Reflection;

namespace EnumStrategyWrapper.Enumeration
{
    public abstract class Enumeration<T> : IEquatable<Enumeration<T>> where T : Enumeration<T>
    {
        public int Value { get; protected init; }
        public string Name { get; protected init; } = String.Empty;
        private static readonly Dictionary<int, T> Enumerations = CreateEnumerations();

        private static Dictionary<int, T> CreateEnumerations()
        {
            Type enumerationType = typeof(T);
            var filterFields = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
            var fields = enumerationType.GetFields(filterFields)
                .Where(fieldInfo => enumerationType.IsAssignableFrom(fieldInfo.FieldType))
                .Select(fieldInfo => (T)fieldInfo.GetValue(default)!);
            return fields.ToDictionary(key => key.Value);
        }

        protected Enumeration(int value, string name) 
        {
            Value = value;
            Name = name;
        }
        public bool Equals(Enumeration<T>? other)
        {
            if (other is null)
                return false;
            if (GetType() != other.GetType())
                return false;
            return Value == other.Value;
        }
        public override bool Equals(object? other)
        {
            if (other is null)
                return false;
            if (other is not Enumeration<T>)
                return false;
            var otherValue = other as Enumeration<T>;
            return Equals(otherValue);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
        }
        public static T? FromValue(int value)
        { 
            return Enumerations.TryGetValue(value, out T? result) ? result : null; 
        }
        public static T? FromName(string name)
        { 
            return Enumerations.Values.SingleOrDefault(x => x.Name == name); 
        }
    }
}
