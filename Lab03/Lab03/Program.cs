using System;
using System.IO;

namespace ReportGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Проверяем, есть ли файл лога рядом с программой
            if (!File.Exists("event_server.log"))
            {
                Console.WriteLine("Файл лога не найден!");
                return;
            }

            // Читаем все строчки из файла в массив
            string[] lines = File.ReadAllLines("event_server.log");

            // Запускаем сборку отчета и выводим результат
            string report = BuildReport(lines);
            Console.WriteLine(report);
        }

        public static string BuildReport(string[] lines)
        {
            // Переменные, куда мы сохраним результаты
            string eventDate = "";
            string winner = "";
            int winnerPoints = 0;
            string eventItem = "";
            int consolationPoints = 0;
            int warningsCount = 0;
            int errorsCount = 0;

            // Этот флаг показывает, идет ли сейчас игровое событие
            bool isEventRunning = false;

            // Проходим циклом по каждой строчке лога
            foreach (string line in lines)
            {
                // 1. Если нашли строчку о начале события — включаем обработку
                if (line.Contains("Событие началось: Восстание Ледяного Пламени"))
                {
                    isEventRunning = true;

                    // Берем первые 10 символов строки (там дата в формате гггг-мм-дд)
                    string datePart = line.Substring(0, 10);
                    // Превращаем текст в настоящую дату
                    if (DateTime.TryParse(datePart, out DateTime parsedDate))
                    {
                        // Переводим в привычный формат дд.мм.гггг (например, 01.09.2026)
                        eventDate = parsedDate.ToString("dd.MM.yyyy");
                    }
                    continue; // Переходим к следующей строке лога
                }

                // 2. Если нашли строчку о закрытии события — выключаем обработку
                if (line.Contains("Событие \"Восстание Ледяного Пламени\" закрыто"))
                {
                    isEventRunning = false;
                }

                // Если событие СЕЙЧАС НЕ ИДЕТ, то просто пропускаем строчку
                if (isEventRunning == false)
                {
                    continue;
                }

                // 3. Считаем предупреждения и ошибки
                if (line.Contains("[Warning]"))
                {
                    warningsCount++;
                }
                if (line.Contains("[Error]"))
                {
                    errorsCount++;
                }

                // 4. Ищем победителя события
                if (line.Contains("[Reward]") && line.Contains("объявлены победителями события"))
                {
                    // Отрезаем техническую часть лога в начале строки
                    int startIdx = line.IndexOf("] ") + 2;
                    string message = line.Substring(startIdx);

                    // Находим, где заканчивается имя клана
                    int endIdx = message.IndexOf(" объявлены победителями");
                    winner = message.Substring(0, endIdx);
                }

                // 5. Ищем очки победителя (строка сработает, когда имя победителя уже найдено)
                if (winner != "" && line.Contains("[Reward]") && line.Contains(winner + " получили"))
                {
                    int startIdx = line.IndexOf("] ") + 2;
                    string message = line.Substring(startIdx);

                    string phrase = winner + " получили ";
                    int numberStart = message.IndexOf(phrase) + phrase.Length;
                    int numberEnd = message.IndexOf(" очков");

                    string pointsText = message.Substring(numberStart, numberEnd - numberStart);
                    int.TryParse(pointsText, out winnerPoints);
                }

                // 6. Ищем ивентовый предмет
                if (line.Contains("[Loot]") && line.Contains("получили ивентовый предмет:"))
                {
                    int startIdx = line.IndexOf("] ") + 2;
                    string message = line.Substring(startIdx);

                    string phrase = "получили ивентовый предмет: ";
                    int itemStart = message.IndexOf(phrase) + phrase.Length;

                    eventItem = message.Substring(itemStart).Trim();
                }

                // 7. Ищем утешительную награду для Железных волков
                if (line.Contains("[Reward]") && line.Contains("Железных волков получили утешительную награду:"))
                {
                    int startIdx = line.IndexOf("] ") + 2;
                    string message = line.Substring(startIdx);

                    string phrase = "Железных волков получили утешительную награду: ";
                    int numberStart = message.IndexOf(phrase) + phrase.Length;
                    int numberEnd = message.IndexOf(" очков");

                    string pointsText = message.Substring(numberStart, numberEnd - numberStart);
                    int.TryParse(pointsText, out consolationPoints);
                }
            }

            //Собираем итоговый текст по шаблону из задания
            string report = $"# Итоги события: Восстание Ледяного Пламени\n\n" +
                            $"Дата: {eventDate}\n" +
                            $"Победитель: {winner}\n" +
                            $"Очки победителя: {winnerPoints}\n" +
                            $"Ивентовый предмет: {eventItem}\n" +
                            $"Утешительная награда Железных волков: {consolationPoints} очков\n" +
                            $"Предупреждений во время события: {warningsCount}\n" +
                            $"Ошибок во время события: {errorsCount}";

            return report;
        }
    }
}
