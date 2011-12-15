namespace Test.Locator
{
    public class ConcreteStubDependency : IStubDependency
    {
        public ConcreteStubDependency(string data)
        {
            this.Data = data;
        }

        public ConcreteStubDependency()
        {
        }

        public ConcreteStubDependency(string data, int moredata, string postfix)
        {
            this.Data = data + moredata + postfix;
        }

        public string Data
        {
            get;
            set;
        }
    }
}