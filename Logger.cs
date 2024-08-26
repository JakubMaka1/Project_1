using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
namespace API_Program;


    public class Logger
    {
        private static readonly string dateFormat = "dd.MM.yyyy HH:mm:ss";

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
                    WriteEmailLog($"Ostatnie wyłaczenie się programu: {lastLine}");
                }
                else
                {
                    WriteEmailLog("Nie znaleziono linii zawierających słowo.");
                }
        }

        public static void CleanOldLogs()
        {
            string logPath = "C:/Temp/EmailLog.txt"; 
            // Odczytanie wszystkich linii z pliku logów
            string[] logLines = File.ReadAllLines(logPath);

            // Obliczanie daty granicznej (1 dzień temu)
            DateTime thresholdDate = DateTime.Now.AddDays(-1);

            // Filtracja linii, które mają datę nowszą niż 1 dzień
            var filteredLogLines = logLines.Where(line =>
                {
                // Wyodrębnienie daty i godziny z początku linii
                string dateString = line.Substring(0, 10); // `dd.MM.yyyy`
                string timeString = line.Substring(11, 8); // `HH:mm:ss`
                string dateTimeString = $"{dateString} {timeString}";

                if (DateTime.TryParseExact(dateTimeString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime logDate))
                {
                    return logDate >= thresholdDate;
                }
                return false;
                }).ToArray(); // Konwersja do tablicy

            // Zapisanie przefiltrowanych linii do pliku logów
            File.WriteAllLines(logPath, filteredLogLines);
        }

}
            

