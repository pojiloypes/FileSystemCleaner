using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileSystemCleaner
{
    public class ConfigurationManager
    {
        // Поле для хранения конфигурации
        IConfiguration config;

        // Путь к файлу appsettings.json
        string pathToAppSettings;

        // Конструктор принимает путь к файлу конфигурации
        public ConfigurationManager(string pathToAppSettings)
        {
            this.pathToAppSettings = pathToAppSettings;

            // Загрузка конфигурации из файла JSON
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Устанавливаем базовый путь как текущую директорию
                .AddJsonFile(pathToAppSettings, optional: false, reloadOnChange: true) // Добавляем JSON-файл конфигурации
                .Build(); // Строим объект конфигурации
        }

        // Метод для получения настроек электронной почты
        public EmailSettings GetEmailSettings()
        {
            // Извлекаем секцию EmailSettings из конфигурации и маппим её на объект EmailSettings
            return config.GetSection("EmailSettings").Get<EmailSettings>();
        }

        // Метод для вывода на консоль всех строк подключения из конфигурации
        public void printConnectionStrings()
        {
            // Извлекаем секцию ConnectionStrings из конфигурации
            var connectionStrings = config.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();

            Console.WriteLine("В системе хранятся следующие базы данных:");
            int index = 1;

            // Если строки подключения существуют, выводим их на консоль
            if (connectionStrings != null)
            {
                foreach (var connectionString in connectionStrings)
                {
                    // Разделяем ключ на имя базы данных и тип СУБД
                    Console.WriteLine($"{index}) {connectionString.Key.Split("-")[0]} ({connectionString.Key.Split("-")[1]})");
                    index++;
                }
            }
        }

        // Метод для получения строки подключения по индексу
        public string getConnectionString(int index)
        {
            var connectionStrings = config.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();

            // Возвращаем строку подключения или null, если индекс некорректен
            if (connectionStrings == null || index >= connectionStrings.Count || index < 0)
                return null;
            else
                return connectionStrings.ElementAt(index).Key;
        }

        // Метод для получения строки подключения по ключу
        public string getConnectionString(string key)
        {
            var connectionStrings = config.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();

            // Возвращаем строку подключения или null, если ключ не найден
            if (connectionStrings != null && connectionStrings.ContainsKey(key))
                return connectionStrings[key];
            else
                return null;
        }

        // Метод для добавления новой строки подключения в конфигурацию
        public void AddConnectionString(string name, string dms, string connectionString)
        {
            var connectionStrings = config.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();
            if (connectionStrings == null)
                connectionStrings = new Dictionary<string, string>();

            // Добавляем новую строку подключения в словарь
            connectionStrings[name + "-" + dms] = connectionString;

            // Читаем текущий JSON-файл конфигурации
            var jsonConfig = System.IO.File.ReadAllText(pathToAppSettings);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonConfig);

            // Добавляем новую строку подключения в JSON-объект
            jsonObj["ConnectionStrings"][name + "-" + dms] = connectionString;

            // Сериализуем обновлённый JSON-объект обратно в файл
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(pathToAppSettings, output);
        }
    }
}
