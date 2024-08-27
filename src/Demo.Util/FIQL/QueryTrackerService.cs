namespace Demo.Util.FIQL
{
    public class QueryTrackerService
    {
        public int QueryCount { get; private set; }

        public void Increment()
        {
            QueryCount++;
        }

        public void Reset()
        {
            QueryCount = 0;
        }
    }
}