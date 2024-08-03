namespace PruebaDepuraciónDiscord;

public static class Logger
{
    internal static void LogMessage(string logFilePath, string message)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {message}");
                Console.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error writing to the log file: " + ex.Message);
        }
    }
}