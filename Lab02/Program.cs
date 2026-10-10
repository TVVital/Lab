using System;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Проверка конфигурации сервера ===\n");

        // --- ВХОДНЫЕ ДАННЫЕ (измените значения для теста) ---
        int maxPlayers = 100;
        int port = 25565;
        int rconPort = 15502; // Конфликт портов
        int ramMb = 4048;     // Мало памяти для 100 игроков

        // --- ПРОВЕРКА ПРАВИЛ ---
        string criticalError = "";
        string warningMessage = "";

        // 1. Критическая ошибка: порты совпадают
        if (port == rconPort)
        {
            criticalError = "Основной порт и порт управления (RCON) совпадают! Сервер не запустится.";
        }

        // 2. Критическая ошибка: некорректное число слотов
        if (maxPlayers <= 0)
        {
            criticalError = "Количество игроков должно быть больше нуля.";
        }

        // 3. Предупреждение: нехватка оперативной памяти (нужно хотя бы 40 МБ на игрока)
        if (criticalError == "" && ramMb < maxPlayers * 40)
        {
            warningMessage = $"Выделено всего {ramMb} МБ RAM. Для {maxPlayers} игроков сервер может сильно лагать.";
        }

        // --- ВЫВОД РЕЗУЛЬТАТА ---
        if (criticalError != "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("СТАТУС: [ЗАПУСК НЕВОЗМОЖЕН]");
            Console.ResetColor();
            Console.WriteLine($"Ошибка: {criticalError}");
        }
        else if (warningMessage != "")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("СТАТУС: [ТРЕБУЕТСЯ ПОДТВЕРЖДЕНИЕ]");
            Console.ResetColor();
            Console.WriteLine($"Внимание: {warningMessage}");
            Console.WriteLine("\nЗапустить сервер вопреки предупреждению? (y/n)");

            if (Console.ReadLine()?.ToLower() == "y")
                Console.WriteLine("Сервер запускается...");
            else
                Console.WriteLine("Запуск отменен.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("СТАТУС: [СЕРВЕР ГОТОВ К ЗАПУСКУ]");
            Console.ResetColor();
            Console.WriteLine("Настройки в порядке. Сервер успешно запущен!");
        }

        Console.ReadKey();
    }
}
