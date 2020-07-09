namespace Normal
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class PrimaryKeyAttribute : System.Attribute
    {
        public bool IsAutoIncrement { get; }
        public PrimaryKeyAttribute(bool isAutoIncrement = true)
        {
            IsAutoIncrement = isAutoIncrement;
        }
    }
}