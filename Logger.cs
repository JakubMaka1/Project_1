using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using PROJECT_1;


    public static class Logger
    {

        public static void WriteEmailLog(string message)
        {
            string logPath = "C:/Temp/EmailLog.txt";

            using(StreamWriter writer = new StreamWriter(logPath, true, Encoding.UTF8))
            {
                writer.WriteLine($"{DateTime.Now} :{message}");
            }
        }
        public static void WriteSystemLog(string message)
        {
            string logPath = "C:/Temp/SystemLog.txt";

            using(StreamWriter writer = new StreamWriter(logPath,true , Encoding.UTF8))
            {
                writer.WriteLine($"{DateTime.Now} :{message}");
            }
        }

        public static void ReadSystemLog(string word)
        {
            string logPath = "C:/Temp/SystemLog.txt";

           var lines = File.ReadAllLines(logPath);

                // Filtracja linii zawierających jakieś słowo 
               
                var filteredLines = lines.Where(line => line.Contains(word, StringComparison.OrdinalIgnoreCase));

                // Wyświetlenie wyników w konsoli
                Console.WriteLine($"Linie zawierające słowo: ${word}");
                foreach (var line in filteredLines)
                {
                    Console.WriteLine(line);
                }
        }

        public static void ReadLastSystemLog(string word)
        {
            string logPath = "C:/Temp/SystemLog.txt";

           var lines = File.ReadAllLines(logPath);

                // Filtracja linii zawierających jakieś słowo 
               
                var lastLine = lines.Where(line => line.Contains(word, StringComparison.OrdinalIgnoreCase)).LastOrDefault();
                if (lastLine != null)
                {
                    Logger.WriteEmailLog($"Ostatnie wyłaczenie się programu: {lastLine}");
                }
                else
                {
                    Logger.WriteEmailLog("Nie znaleziono linii zawierających słowo.");
                }
        }
    }
