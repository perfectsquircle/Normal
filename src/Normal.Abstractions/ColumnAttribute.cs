namespace Normal
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ColumnAttribute : System.Attribute
    {
        public ColumnAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}