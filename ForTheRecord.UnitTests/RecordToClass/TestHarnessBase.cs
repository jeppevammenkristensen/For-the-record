namespace ForTheRecord.UnitTests.RecordToClass
{
    public abstract class TestHarnessBase<T>
    {
        public T Subject { get; protected set; }

        public TestHarnessBase()
        {
           
        }

    }
}