namespace MultiThreading;

public static class Tools
{
    /// <summary>
    /// Метод возвращает количество физических ядер.
    /// </summary>
    /// <returns>Количество физических ядер.</returns>
    public static int GetCoreCount()
    {
        int coreCount = 0;
        foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
        {
            coreCount += int.Parse(item["NumberOfCores"].ToString());
        }
        return coreCount;
    }

    /// <summary>
    /// Метод возвращает количество логических процессоров.
    /// </summary>
    /// <returns>Количество логических процессоров.</returns>
    public static int GetCpuCount()
    {
        return Environment.ProcessorCount;
    }
}
