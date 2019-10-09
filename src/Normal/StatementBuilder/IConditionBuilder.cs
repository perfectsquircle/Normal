namespace Normal
{
    public interface IConditionBuilder<TParent>
    {
        TParent EqualTo(object value);
        TParent NotEqualTo(object value);
        TParent GreaterThan(object value);
        TParent LessThan(object value);
        TParent GreaterThanOrEqualTo(object value);
        TParent LessThanOrEqualTo(object value);
        TParent Like(string value);
        TParent ILike(string value);
        TParent IsNull();
        TParent IsNotNull();
        IConditionBuilder<TParent> And(string condition);
        IConditionBuilder<TParent> Or(string condition);
        TParent End();
    }
}