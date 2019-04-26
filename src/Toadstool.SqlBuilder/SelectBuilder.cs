namespace Toadstool
{
    public class SelectBuilder
    {
        private readonly string _selectList;
        private string _from;

        internal SelectBuilder(string selectList)
        {
            this._selectList = selectList ?? throw new System.ArgumentNullException(nameof(selectList));
        }

        public SelectBuilder From(string from)
        {
            _from = from ?? throw new System.ArgumentNullException(nameof(from));
            return this;
        }

        public SelectBuilder
    }
}