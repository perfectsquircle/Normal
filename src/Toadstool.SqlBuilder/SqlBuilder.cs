namespace Toadstool
{
    public static class SqlBuilder
    {
        public static SelectBuilder Select(params string[] selectList)
        {
            return new SelectBuilder(string.Join(", ", selectList));
        }
    }
}