using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Data.Common;

namespace FileSystemCleaner
{
    class Program
    {
        // Метод для создания экземпляра IDBRequestsManager в зависимости от типа СУБД
        static IDBRequestsManager setDBRequestsManagerType(string dms)
        {
            IDBRequestsManager rm = null; // Инициализация переменной для менеджера запросов
            switch (dms)
            {
                case "PostgreSQL":
                    rm = new PostgreSqlRequestsManager(); // Создаем менеджер запросов для PostgreSQL
                    break;
                case "MS SQL Server":
                    rm = new MSSqlServerRequestsManager(); // Создаем менеджер запросов для MS SQL Server
                    break;
            }
            return rm;
        }

        // Метод для создания соединения с базой данных в зависимости от типа СУБД
        static DbConnection setConnectionType(string dms)
        {
            DbConnection con = null; // Инициализация переменной для соединения
            switch (dms)
            {
                case "PostgreSQL":
                    con = new NpgsqlConnection(); // Создаем соединение для PostgreSQL
                    break;
                case "MS SQL Server":
                    con = new SqlConnection(); // Создаем соединение для MS SQL Server
                    break;
            }
            return con;
        }

        // Метод для получения типа СУБД от пользователя
        static string getDms()
        {
            string dms = ""; // Инициализация переменной для типа СУБД
            // Цикл продолжается, пока не введено допустимое значение (PostgreSQL или MS SQL Server)
            while (dms != "PostgreSQL" && dms != "MS SQL Server")
            {
                Console.Write("Введите название СУБД (PostgreSQL или MS SQL Server): ");
                dms = Console.ReadLine(); // Получаем тип СУБД от пользователя
            }
            return dms;
        }

        static void Main(string[] args)
        {
            // Создаем объект ConfigurationManager для работы с конфигурационным файлом
            ConfigurationManager cm = new ConfigurationManager("замените меня");

            // Выводим список доступных строк подключения из конфигурационного файла
            cm.printConnectionStrings();

            bool errorFlag = true; // Флаг для проверки ошибок при вводе
            string connectionString = "";
            string dbName = "", dms = "";

            // Цикл для выбора базы данных или ввода новой строки подключения
            while (errorFlag)
            {
                Console.Write("Введите номер базы данных, которую хотели бы очистить или \"-1\", если хотите подключиться к новой базе данных: ");
                int choice = int.Parse(Console.ReadLine());  // Получаем выбор пользователя
                string key = cm.getConnectionString(choice - 1);  // Получаем строку с названием БД и СУБД для получения в будущем строки подключения

                // Если пользователь выбрал -1, запрашиваем ввод новой строки подключения
                if (choice == -1)
                {
                    Console.Write("Введите название базы данных: ");
                    dbName = Console.ReadLine();  // Получаем имя базы данных

                    dms = getDms();  // Определяем тип СУБД

                    Console.Write("Введите строку подключения: ");
                    connectionString = Console.ReadLine();  // Получаем строку подключения от пользователя

                    errorFlag = false;  // Завершаем цикл, так как ошибка отсутствует
                }
                // Если выбор пользователя соответствует существующей строке подключения
                else if (key != null)
                {
                    errorFlag = false;  // Завершаем цикл, так как ошибка отсутствует
                    connectionString = cm.getConnectionString(key);  // Получаем строку подключения по ключу
                    dms = key.Split("-")[1];  // Определяем тип СУБД по ключу
                }
            }

            // Получаем настройки почты из конфигурационного файла
            EmailSettings emailSettings = cm.GetEmailSettings();

            // Создаем логгер с использованием настроек почты
            ILoger loger = new Loger(emailSettings);

            // Определяем тип менеджера запросов к базе данных в зависимости от СУБД
            IDBRequestsManager requestsManager = setDBRequestsManagerType(dms);

            // Создаем соединение с базой данных в зависимости от СУБД
            DbConnection connection = setConnectionType(dms);

            // Создаем объект FileSystemCleaner и передаем необходимые зависимости
            FileSystemCleaner cleaner = new FileSystemCleaner(requestsManager, loger, connection, connectionString);

            // Запускаем процесс очистки файловой системы
            cleaner.clear();

            // Если очистка прошла успешно и была введена новая строка подключения, добавляем ее в конфигурационный файл
            if (!cleaner.errorFlag && dbName != "")
                cm.AddConnectionString(dbName, dms, connectionString);
        }
    }
}
