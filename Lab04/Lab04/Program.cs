using System;
using System.IO;

namespace LogAnalyzer
{
    // 1. Создаем класс для хранения одной записи лога (как требуют тесты)
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = "";
        public string Category { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            string logPath = "event_server.log";
            if (!File.Exists(logPath))
            {
                Console.WriteLine("Файл лога не найден!");
                return;
            }

            string[] lines = File.ReadAllLines(logPath);

            // Парсим логи в объекты
            LogEntry[] entries = ParseLog(lines);

            // Пример работы: выведем статус сервера
            Console.WriteLine(GetServerStatus(entries));
        }

        // 2. Метод парсинга массива строк в массив объектов LogEntry
        public static LogEntry[] ParseLog(string[] lines)
        {
            LogEntry[] result = new LogEntry[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                LogEntry entry = new LogEntry();

                // Извлекаем дату и время (первые 23 символа: "2026-09-01 12:10:01.101")
                string timeStr = line.Substring(0, 23);
                if (DateTime.TryParse(timeStr, out DateTime parsedDate))
                {
                    entry.Timestamp = parsedDate;
                }

                // Ищем Уровень лога (например, [Info], [Warning], [Error], [Fatal])
                int levelStart = line.IndexOf('[') + 1;
                int levelEnd = line.IndexOf(']');
                entry.Level = line.Substring(levelStart, levelEnd - levelStart);

                // Ищем Категорию (вторые квадратные скобки, например, [Server], [Database], [Combat])
                int catStart = line.IndexOf('[', levelEnd + 1) + 1;
                int catEnd = line.IndexOf(']', levelEnd + 1);
                entry.Category = line.Substring(catStart, catEnd - catStart);

                // Все остальное после категории — это само сообщение лога
                int msgStart = catEnd + 2; // пропускаем ']' и пробел
                entry.Message = line.Substring(msgStart);

                result[i] = entry;
            }

            return result;
        }

        // 3. Фильтрация записей по конкретной дате
        public static LogEntry[] FilterByDate(LogEntry[] entries, DateTime targetDate)
        {
            // Сначала считаем, сколько записей совпадает по дате, чтобы выделить массив нужного размера
            int count = 0;
            foreach (var entry in entries)
            {
                if (entry.Timestamp.Date == targetDate.Date) count++;
            }

            LogEntry[] filtered = new LogEntry[count];
            int index = 0;
            foreach (var entry in entries)
            {
                if (entry.Timestamp.Date == targetDate.Date)
                {
                    filtered[index++] = entry;
                }
            }
            return filtered;
        }

        // 4. Фильтрация записей по уровню лога (Error, Info и т.д.)
        public static LogEntry[] FilterByLevel(LogEntry[] entries, string level)
        {
            int count = 0;
            foreach (var entry in entries)
            {
                if (entry.Level == level) count++;
            }

            LogEntry[] filtered = new LogEntry[count];
            int index = 0;
            foreach (var entry in entries)
            {
                if (entry.Level == level)
                {
                    filtered[index++] = entry;
                }
            }
            return filtered;
        }

        // 5. Фильтрация записей по категории (Combat, Database и т.д.)
        public static LogEntry[] FilterByCategory(LogEntry[] entries, string category)
        {
            int count = 0;
            foreach (var entry in entries)
            {
                if (entry.Category == category) count++;
            }

            LogEntry[] filtered = new LogEntry[count];
            int index = 0;
            foreach (var entry in entries)
            {
                if (entry.Category == category)
                {
                    filtered[index++] = entry;
                }
            }
            return filtered;
        }

        // 6. Поиск по тексту сообщения (Регистронезависимый)
        public static LogEntry[] Search(LogEntry[] entries, string keyword)
        {
            int count = 0;
            foreach (var entry in entries)
            {
                // Приводим обе строки к нижнему регистру для проверки (Case Insensitive)
                if (entry.Message.ToLower().Contains(keyword.ToLower())) count++;
            }

            LogEntry[] filtered = new LogEntry[count];
            int index = 0;
            foreach (var entry in entries)
            {
                if (entry.Message.ToLower().Contains(keyword.ToLower()))
                {
                    filtered[index++] = entry;
                }
            }
            return filtered;
        }

        // 7. Подсчет количества записей определенного уровня
        public static int CountByLevel(LogEntry[] entries, string level)
        {
            int count = 0;
            foreach (var entry in entries)
            {
                if (entry.Level == level) count++;
            }
            return count;
        }

        // 8. Определение статуса сервера (если есть Fatal — значит упал)
        public static string GetServerStatus(LogEntry[] entries)
        {
            foreach (var entry in entries)
            {
                if (entry.Level == "Fatal")
                {
                    return "КРИТИЧЕСКАЯ ОШИБКА: сервер остановлен";
                }
            }
            return "Сервер работает в штатном режиме";
        }
    }
}
