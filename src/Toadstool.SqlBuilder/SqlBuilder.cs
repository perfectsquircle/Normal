namespace Toadstool
{
    public static class SqlBuilder
    {
        public static SelectBuilder Select(params string[] selectList)
        {
            return new SelectBuilder(selectList);
        }

        public static InsertBuilder InsertInto(string tableName, params string[] columnNames)
        {
            return new InsertBuilder(tableName, columnNames);
        }
    }
}