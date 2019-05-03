namespace Toadstool
{
    public class ConditionBuilder<TParent> where TParent : class, IStatementBuilder
    {
        private readonly TParent _parent;
        private string _keyword;
        private string _columnName;

        internal ConditionBuilder(TParent parent, string keyword, string columnName)
        {
            _parent = parent;
            _keyword = keyword;
            _columnName = columnName;
        }

        public TParent EqualTo(object value)
        {
            return RegisterParameter("=", value);
        }

        public TParent NotEqualTo(object value)
        {
            return RegisterParameter("!=", value);
        }

        public TParent GreaterThan(object value)
        {
            return RegisterParameter(">", value);
        }

        public TParent LessThan(object value)
        {
            return RegisterParameter("<", value);
        }

        public TParent GreaterThanOrEqualTo(object value)
        {
            return RegisterParameter(">=", value);
        }

        public TParent LessThanOrEqualTo(object value)
        {
            return RegisterParameter("<=", value);
        }

        public TParent Like(string value)
        {
            return RegisterParameter("LIKE", value);
        }

        public TParent ILike(string value)
        {
            return RegisterParameter("ILIKE", value);
        }

        public TParent IsNull()
        {
            return _parent.AddLine(_keyword, $"{_columnName} IS NULL") as TParent;
        }

        public TParent IsNotNull()
        {
            return _parent.AddLine(_keyword, $"{_columnName} IS NOT NULL") as TParent;
        }

        public ConditionBuilder<TParent> And(string condition)
        {
            _parent.AddLine(_keyword, _columnName); // this is actually a full condition;
            return new ConditionBuilder<TParent>(_parent, "AND", condition);
        }

        public ConditionBuilder<TParent> Or(string condition)
        {
            _parent.AddLine(_keyword, _columnName); // this is actually a full condition;
            return new ConditionBuilder<TParent>(_parent, "OR", condition);
        }

        public TParent End()
        {
            return _parent;
        }

        private TParent RegisterParameter(string comparator, object value)
        {
            var parameterName = _parent.RegisterParameter(value);
            var condition = $"{_columnName} {comparator} @{parameterName}";
            return _parent.AddLine(_keyword, condition) as TParent;
        }
    }
}