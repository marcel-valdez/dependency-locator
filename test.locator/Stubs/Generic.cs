
namespace Test.Locator
{
    public class Generic<T> : BaseGeneric<T>
            where T : class, new()
    {
        public Generic()
            : base(new T())
        {
        }

        public Generic(T arg)
            : base(arg)
        {
        }

        public override T Property
        {
            get;

            set;
        }
    }
}
