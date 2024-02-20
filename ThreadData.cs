namespace MultiThreading;

public class ThreadData
{
    public IList<int> Data { get; set; } = new List<int>();

    public int ThreadNumber { get; set; }

    public int ThreadsCount { get; set; }

    public decimal[] Results { get; set; }
}
