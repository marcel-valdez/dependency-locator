namespace Test.Locator
{
    public interface IGeneric<T>
            where T : class
    {
        T Property
        {
            get;
            set;
        }
    }
}