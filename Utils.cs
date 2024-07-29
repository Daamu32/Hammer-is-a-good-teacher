using Newtonsoft.Json;

namespace PruebaDepuraciónDiscord;

public static class Utils
{
    internal static string GetDirectoryPathFromUser()
    {
        var directory = ReadConfig();

        if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
        {
            Console.WriteLine($"Using previously used directory: {directory}");
            Console.WriteLine("Do you want to use this directory? (Y/N):");
            label1:
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Y: break;
                case ConsoleKey.N:
                    Console.WriteLine("Please enter the directory path or enter '1' for the Downloads directory:");
                    var input = Console.ReadLine();
                    if (input == "1")
                    {
                        directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                            "Downloads");
                        Console.WriteLine($"Using Downloads directory: {directory}");
                    }
                    else
                    {
                        directory = input ?? throw new NullReferenceException("Directory was null, please check config or input");
                    }
                    WriteConfig(directory);
                    break;
                default:
                    Console.WriteLine("Incorrect input. Please press Y or N");
                    goto label1;
            }
        }
        return directory;
    }

    internal static string ReadConfig()
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

    internal static void WriteConfig(string directoryPath)
    {
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        var config = new { LastUsedDirectory = directoryPath };
        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(configPath, json);
    }

    internal static void EnsureLogFileExists(string logFilePath)
    {
        if (!File.Exists(logFilePath))
        {
            File.Create(logFilePath).Dispose();
        }
    }

    internal static void CleanLogFiles(string directoryPath, string logFilePath)
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
                Logger.LogMessage(logFilePath, $"The log file '{fileName}' has been deleted.");
                deletedFiles = true;
            }
        }

        if (!deletedFiles)
        {
            Logger.LogMessage(logFilePath, "Valid directory. No files were found that meet the required characteristics.");
        }
    }

    internal static bool ShouldDeleteFile(string fileName, string filePath)
    {
        if (fileName.Contains("RagePluginHook") || fileName.Contains("ELS") || fileName.Contains("asiloader") ||
            fileName.Contains("ScriptHookVDotNet"))
        {
            return true;
        }

        // This is for log files that are named as "message.txt" f.ex. Usually when an user just copy pastes the log content to discord. 
        if (fileName.Contains("message"))
        {
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
}