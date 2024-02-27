using System.Diagnostics;

namespace MultiThreading;

public class Program
{

    static void Main(string[] args)
    {
        //Напишите вычисление суммы элементов массива интов:
        // Обычное
        // Параллельное(для реализации использовать Thread, например List)
        // Параллельное с помощью LINQ
        //Замерьте время выполнения для 100 000, 1 000 000 и 10 000 000
        //Укажите в таблице результаты замеров, указав:
        //  Окружение(характеристики компьютера и ОС)
        //   Время выполнения последовательного вычисления
        //   Время выполнения параллельного вычисления
        //   Время выполнения LINQ
        //Пришлите в чат с преподавателем помимо ссылки на репозиторий номера своих строк в таблице.

        Console.WriteLine("Started");

        var dataCount = new int[] { 100_000, 1_000_000, 10_000_000, 100_000_000 };

        //Задаем число тестов.
        //Будем вычислять среднее время выполнения за testCount тестов, т.к. это даст более точный результат.
        const int testCount = 20;

        //Средний результат.
        var averageResult = new AverageResult();

        //Задаем число потоков.
        int threadsCount = Tools.GetCpuCount(); //Tools.GetNumberOfCores(); //16; //8;

        for (int testNumber = 0; testNumber < testCount; testNumber++)
        {
            Console.WriteLine($"Test {testNumber + 1}");

            foreach (var count in dataCount)
            {
                var data = DataProvider.GetData(count);

                var resultSimpleSum = SimpleSum(data);
                averageResult.AddResult(resultSimpleSum);

                var resultThreadSum = ThreadSum(data, threadsCount);
                averageResult.AddResult(resultThreadSum);

                var resultPlinqSum = PlinqSum(data, threadsCount);
                averageResult.AddResult(resultPlinqSum);

                if (testNumber == 0)
                {
                    Console.WriteLine("First result:");

                    Console.WriteLine(resultSimpleSum);
                    Console.WriteLine(resultThreadSum);
                    Console.WriteLine(resultPlinqSum);

                    Console.WriteLine();
                }
            }
        }

        Console.WriteLine("Average result:");
        var results = averageResult.GetResults();
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }

    /// <summary>
    /// Простое суммирование.
    /// </summary>
    /// <param name="data">Данные.</param>
    /// <returns>Сумма.</returns>
    static decimal Sum(IList<int> data)
    {
        //Сохраняем сумму в decimal, чтобы не было арифметической перегрузки при суммировании.
        decimal result = 0;
        foreach (var item in data)
            result += item;

        return result;
    }

    static SumResult SimpleSum(IList<int> data)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var sum = Sum(data);

        stopWatch.Stop();

        var result = new SumResult
        {
            MethodName = "SimpleSum",
            DataLength = data.Count,
            Result = sum,
            MsecElapsed = (long)stopWatch.Elapsed.TotalMilliseconds
        };

        return result;
    }

    static SumResult ThreadSum(IList<int> data, int threadsCount)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        //Массив для записи результата суммирования.
        var results = new decimal[threadsCount];

        //Массив потоков.
        var threads = new List<Thread>(threadsCount);

        //Создаём threadsCount потоков.
        for (int i = 0; i < threadsCount; i++)
            threads.Add(new Thread(ThreadSumRun));

        //Запускаем threadsCount потоков.
        for (int i = 0; i < threadsCount; i++)
            threads[i].Start(new ThreadData
            {
                Data = data,
                ThreadNumber = i,
                ThreadsCount = threadsCount,
                Results = results
            });

        //Ожидаем завершения всех потоков.
        threads.ForEach(x => x.Join());

        //Получаем итоговую сумму от всех потоков.
        var sum = results.Sum();

        stopWatch.Stop();

        var result = new SumResult
        {
            MethodName = "ThreadSum",
            DataLength = data.Count,
            Result = sum,
            MsecElapsed = (long)stopWatch.Elapsed.TotalMilliseconds
        };

        return result;
    }

    /// <summary>
    /// Суммирование несколькими системными потоками.
    /// </summary>
    /// <param name="args">Объект ThreadData.</param>
    static void ThreadSumRun(object? args)
    {
        if (args is not ThreadData threadData)
            throw new ArgumentNullException(nameof(threadData));

        decimal result = 0;

        //Каждый поток считает сумму своих элементов.
        for (var i = threadData.ThreadNumber; i < threadData.Data.Count; i += threadData.ThreadsCount)
            result += threadData.Data[i];

        threadData.Results[threadData.ThreadNumber] = result;
    }

    /// <summary>
    /// Суммирование через PLINQ.
    /// </summary>
    /// <param name="data">Данные.</param>
    /// <param name="threadsCount">Число потоков.</param>
    /// <returns></returns>
    static SumResult PlinqSum(IList<int> data, int threadsCount)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var plinqData = new List<ThreadData>();

        //Массив для записи результата суммирования.
        var results = new decimal[threadsCount];

        for (int i = 0; i < threadsCount; i++)
        {
            var threadData = new ThreadData
            {
                Data = data,
                ThreadNumber = i,
                ThreadsCount = threadsCount,
                Results = results
            };

            plinqData.Add(threadData);
        }

        //Запускаем параллельный расчет.
        plinqData
            .AsParallel()
            .WithDegreeOfParallelism(threadsCount)
            .ForAll(ThreadSumRun);

        //Получаем итоговую сумму от всех потоков.
        var sum = results.Sum();

        stopWatch.Stop();

        var result = new SumResult
        {
            MethodName = "PlinqSum",
            DataLength = data.Count,
            Result = sum,
            MsecElapsed = (long)stopWatch.Elapsed.TotalMilliseconds
        };

        return result;
    }
}
