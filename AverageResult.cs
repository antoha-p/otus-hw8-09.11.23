namespace MultiThreading;

public class AverageResult
{
    private readonly Dictionary<string, SumResult> _dict = new();
    private readonly Dictionary<string, int> _counter = new();
    private readonly Dictionary<string, long> _timeAdded = new();

    public void AddResult(SumResult result)
    {
        var key = $"{result.MethodName}_{result.DataLength}";

        if (!_dict.ContainsKey(key))
        {
            _dict.Add(key, result);
            _counter.Add(key, 1);
            _timeAdded.Add(key, DateTime.UtcNow.Ticks);
        }
        else
        {
            var oldResult = _dict[key];
            var oldCounter = _counter[key];
            oldResult.MsecElapsed = (oldResult.MsecElapsed * oldCounter + result.MsecElapsed) / (oldCounter + 1);
            _counter[key]++;
        }
    }

    public IList<SumResult> GetResults()
    {
        return _dict.Values.OrderBy(x => _timeAdded[$"{x.MethodName}_{x.DataLength}"]).ToList();
    }
}
