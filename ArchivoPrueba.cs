namespace PruebaDepuraciónDiscord
{ 
    static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            string directoryPath = Utils.GetDirectoryPathFromUser();

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("The specified directory does not exist.");
                return;
            }

            string logFilePath = Directory.GetCurrentDirectory() + "\\RemoveLogs.log";
            Utils.EnsureLogFileExists(logFilePath);

            Utils.CleanLogFiles(directoryPath, logFilePath);
        }
    }
}