namespace MultiThreading;

public static class DataProvider
{
    private static readonly Random R = new Random();

    public static IList<int> GetData(int count)
    {
        var result = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            result.Add(R.Next(100));
        }

        return result;
    }
}
