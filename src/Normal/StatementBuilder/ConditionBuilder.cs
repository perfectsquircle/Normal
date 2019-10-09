namespace Normal
{
    internal class ConditionBuilder<TParent> : IConditionBuilder<TParent>
        where TParent : IStatementBuilder
    {
        private readonly StatementBuilder _parent;
        private readonly string _keyword;
        private readonly string _columnName;

        public ConditionBuilder(StatementBuilder parent, string keyword, string columnName)
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
            return (TParent)_parent.AddLine(_keyword, $"{_columnName} IS NULL");
        }

        public TParent IsNotNull()
        {
            return (TParent)_parent.AddLine(_keyword, $"{_columnName} IS NOT NULL");
        }

        public IConditionBuilder<TParent> And(string condition)
        {
            _parent.AddLine(_keyword, _columnName); // this is actually a full condition;
            return new ConditionBuilder<TParent>(_parent, "AND", condition);
        }

        public IConditionBuilder<TParent> Or(string condition)
        {
            _parent.AddLine(_keyword, _columnName); // this is actually a full condition;
            return new ConditionBuilder<TParent>(_parent, "OR", condition);
        }

        public TParent End()
        {
            return (TParent)(IStatementBuilder)_parent;
        }

        private TParent RegisterParameter(string comparator, object value)
        {
            var parameterName = _parent.RegisterParameter(value);
            var condition = $"{_columnName} {comparator} @{parameterName}";
            return (TParent)_parent.AddLine(_keyword, condition);
        }
    }
}