namespace Toadstool
{
    public class ConditionBuilder
    {
        private readonly SelectBuilder _parent;
        private string _keyword;
        private string _columnName;

        internal ConditionBuilder(SelectBuilder parent, string keyword, string columnName)
        {
            _parent = parent;
            _keyword = keyword;
            _columnName = columnName;
        }

        public SelectBuilder EqualTo(object value)
        {
            return RegisterParameter("=", value);
        }

        public SelectBuilder NotEqualTo(object value)
        {
            return RegisterParameter("!=", value);
        }

        public SelectBuilder GreaterThan(object value)
        {
            return RegisterParameter(">", value);
        }

        public SelectBuilder LessThan(object value)
        {
            return RegisterParameter("<", value);
        }

        public SelectBuilder GreaterThanOrEqualTo(object value)
        {
            return RegisterParameter(">=", value);
        }

        public SelectBuilder LessThanOrEqualTo(object value)
        {
            return RegisterParameter("<=", value);
        }

        public SelectBuilder Like(string value)
        {
            return RegisterParameter("LIKE", value);
        }

        public SelectBuilder ILike(string value)
        {
            return RegisterParameter("ILIKE", value);
        }

        public SelectBuilder IsNull()
        {
            return _parent.AddLine(_keyword, $"{_columnName} IS NULL");
        }

        public SelectBuilder IsNotNull()
        {
            return _parent.AddLine(_keyword, $"{_columnName} IS NOT NULL");
        }

        public ConditionBuilder And(string condition)
        {
            _parent.AddLine(_keyword, _columnName); // this is actually a full condition;
            return _parent.And(condition);
        }

        public ConditionBuilder Or(string columnName)
        {
            _parent.AddLine(_keyword, _columnName); // this is actually a full condition;
            return new ConditionBuilder(_parent, "OR", columnName);
        }

        public SelectBuilder End()
        {
            return _parent;
        }

        private SelectBuilder RegisterParameter(string comparator, object value)
        {
            var parameterName = _parent.RegisterParameter(value);
            var condition = $"{_columnName} {comparator} @{parameterName}";
            return _parent.AddLine(_keyword, condition);
        }
    }
}