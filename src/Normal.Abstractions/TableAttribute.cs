namespace Normal
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class TableAttribute : System.Attribute
    {
        public TableAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}