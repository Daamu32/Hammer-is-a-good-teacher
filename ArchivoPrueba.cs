using System;
using System.IO;
using Newtonsoft.Json;

namespace LogFileCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            string directoryPath = GetDirectoryPathFromUser();

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("The specified directory does not exist.");
                return;
            }

            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),"OneDrive", "Escritorio", "test.log");

            EnsureLogFileExists(logFilePath);

            CleanLogFiles(directoryPath, logFilePath);
        }

        static string GetDirectoryPathFromUser()
        {
            string lastUsedDirectory = ReadConfig();

            if (!string.IsNullOrEmpty(lastUsedDirectory) && Directory.Exists(lastUsedDirectory))
            {
                Console.WriteLine($"Using previously used directory: {lastUsedDirectory}");
                Console.WriteLine("Do you want to use this directory? (Y/N):");
                Label 1:
                switch(Console.ReadKey()){
                    case ConsoleKey.Y: 
                        return lastUsedDirectory;
                    case ConsoleKey.N:
                        Console.WriteLine("Please enter the directory path or enter '1' for the Downloads directory:");
                        string input = Console.ReadLine();

                        string directoryPath;
                        if (input == "1")
                        {
                            directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                            Console.WriteLine($"Using Downloads directory: {directoryPath}");
                        }
                        else
                        {
                            directoryPath = input;
                        }

                        WriteConfig(directoryPath);
                        return directoryPath;
                    default:
                        Console.WriteLine("Incorrect input. Please press Y or N");
                        goto Label1;
                        break;
                }
            }
        }

        static string ReadConfig()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                dynamic config = JsonConvert.DeserializeObject(json);
                return config.LastUsedDirectory;
            }
            return string.Empty;
        }

        static void WriteConfig(string directoryPath)
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var config = new { LastUsedDirectory = directoryPath };
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, json);
        }

        static void EnsureLogFileExists(string logFilePath)
        {
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose();
            }
        }

        static void CleanLogFiles(string directoryPath, string logFilePath)
        {
            string[] logFiles = Directory.GetFiles(directoryPath, "*.log");
            string[] txtFiles = Directory.GetFiles(directoryPath, "*.txt");

            string[] allFiles = new string[logFiles.Length + txtFiles.Length];
            logFiles.CopyTo(allFiles, 0);
            txtFiles.CopyTo(allFiles, logFiles.Length);

            bool deletedFiles = false;

            foreach (var file in allFiles)
            {
                string fileName = Path.GetFileName(file);
                if (ShouldDeleteFile(fileName, file))
                {
                    File.Delete(file);
                    Console.WriteLine($"The log file '{fileName}' has been deleted.");
                    LogMessage(logFilePath, $"The log file '{fileName}' has been deleted.");
                    deletedFiles = true;
                }
            }

            if (!deletedFiles)
            {
                Console.WriteLine("Valid directory. No files were found that meet the required characteristics.");
                LogMessage(logFilePath, "Valid directory. No files were found that meet the required characteristics.");
            }
        }

        static bool ShouldDeleteFile(string fileName, string filePath)
        {
            if (fileName.Contains("RagePluginHook") || fileName.Contains("ELS") || fileName.Contains("asiloader") || fileName.Contains("ScriptHookVDotNet"))
            {
                return true;
            }
            // This is for log files that are named as "message.txt" f.ex. Usually when an user just copy pastes the log content to discord. 
            if(fileName.Contains("message")){
                // This logic here is only needed for the "message.txt" file.
                using (StreamReader reader = new StreamReader(filePath))
                {
                string firstLine = reader.ReadLine();
                if (firstLine != null && firstLine.Contains("Started new log on"))
                {
                    return true;
                }
            }
            }
            

            return false;
        }

        static void LogMessage(string logFilePath, string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to the log file: " + ex.Message);
            }
        }
    }
}