namespace MultiThreading;

public class SumResult
{
    public string MethodName { get; set; } = string.Empty;

    public long DataLength { get; set; }

    public decimal Result { get; set; }

    public long MsecElapsed { get; set; }
    
    public override string ToString()
    {
        return $"{MethodName}_{DataLength}, sum: {Result}, time (msec): {MsecElapsed}";
    }
}
