namespace PruebaDepuraciónDiscord
{ 
    static class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // Check if an argument was passed to delete download files
            if (args.Length > 0 && args[0] == "deleteDownloads")
            {
                string DownloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                string DownloadlogFilePath = Directory.GetCurrentDirectory() + "\\RemoveLogs.log";
                Console.WriteLine($"Using Downloads directory: {DownloadDirectory}");
                Utils.EnsureLogFileExists(DownloadlogFilePath);
                Utils.CleanLogFiles(DownloadDirectory, DownloadlogFilePath);
                return;
            }


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