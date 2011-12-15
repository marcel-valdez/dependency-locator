namespace Test.Locator
{
    public class ConstructorableStub : IConstructorableStub
    {
        public ConstructorableStub(IStubDependency dependency)
        {
            this.Data = dependency.Data;
        }

        public ConstructorableStub(IStubDependency dependency, IConstructorableStub another)
        {
            this.Data = dependency.Data;
            this.Data += another.Data;
        }

        public string Data
        {
            get;
            set;
        }
    }
}