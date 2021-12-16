namespace Integral.Application.Common.Persistence
{
    public class ModifiedProperty
    {
        public ModifiedProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}
