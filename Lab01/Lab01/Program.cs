namespace Lab01
{
    class Program
    {
        static void Main()
        {
            string Time = "71:55:43";
            float Ping = 999f;
            float Speed = 90.5f;
            ushort Players = 6769;

            Console.WriteLine("----------------Основные параметры-----------------");
            Console.WriteLine($"Время работы сервера: {Time}");
            Console.WriteLine($"Средний пинг: {Ping} м/с");
            Console.WriteLine($"Скорость соединения: {Speed} Мбит/с");
            Console.WriteLine($"Количество игроков на сервере: {Players}");
        }
    }
}