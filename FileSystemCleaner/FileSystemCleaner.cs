using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FileSystemCleaner
{
    // Класс FileSystemCleaner отвечает за очистку файловой системы и базы данных от неиспользуемых файлов.
    class FileSystemCleaner
    {
        // Поля класса для хранения зависимостей и настроек.
        IDBRequestsManager requestsManager;  // Менеджер для выполнения SQL-запросов, специфичных для СУБД.
        DbConnection connection;  // Соединение с базой данных.
        ILoger loger;  // Логгер для ведения логов.
        string connectionString;  // Строка подключения к базе данных.
        public bool errorFlag { get; set; }  // Флаг ошибок, указывающий на наличие проблем при выполнении операций.

        // Конструктор инициализирует зависимости и устанавливает соединение с базой данных.
        public FileSystemCleaner(IDBRequestsManager requestsManager, ILoger loger, DbConnection connection, string connectionString)
        {
            this.requestsManager = requestsManager;
            this.loger = loger;
            this.connection = connection;
            this.connectionString = connectionString;
            setConnection();  // Устанавливаем соединение при создании объекта.
        }

        // Метод, определяющий тип базы данных по строке подключения и возвращающий соответствующее соединение.
        // своего рода легаси код :D: так как я не понял устраивает ли заказчика опредление субд таким способом, он более не используется
        // если окажестя что устраивает, лучше будет использовать его, чем то что исопльзуется сейчас
        DbConnection getDbConnection()
        {
            if (connectionString.Contains("Host=") && connectionString.Contains("Username="))
            {
                // PostgreSQL
                requestsManager = new PostgreSqlRequestsManager();
                return new NpgsqlConnection(connectionString);
            }
            else if (connectionString.Contains("Server=") && connectionString.Contains("Database="))
            {
                // MS SQL Server
                requestsManager = new MSSqlServerRequestsManager();
                return new SqlConnection(connectionString);
            }

            // Не удалось определить тип базы данных.
            return null;
        }

        // Метод для установки соединения с базой данных.
        public void setConnection()
        {
            try
            {
                // Логируем попытку подключения с параметрами строки подключения.
                loger.logData($"[{DateTime.Now.ToString()}] Попытка подключения к базе данных с параметрами:\n\t{connectionString.Replace(";", "\n\t")}");
                connection.ConnectionString = connectionString;

                if (connection == null)
                {
                    // Если соединение не удалось установить, логируем сообщение о неподдерживаемой СУБД.
                    loger.logData($"\n[{DateTime.Now.ToString()}] Работа с данной СУБД в данный момент не поддерживается.");
                    Console.WriteLine("Работа с данной СУБД в данный момент не поддерживается.");
                    errorFlag = true;  // Устанавливаем флаг ошибки.
                }
                else
                {
                    connection.Open();  // Открываем соединение с базой данных.
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок подключения и логирование сообщения об ошибке.
                Console.WriteLine("Возникла ошибка: " + ex.Message);
                loger.logData($"\n[{DateTime.Now.ToString()}] Возникла ошибка при подключении: " + ex.Message);
                errorFlag = true;  // Устанавливаем флаг ошибки.
            }
        }

        // Метод для получения списка таблиц и столбцов, связанных с файлами, из базы данных.
        List<DBColumn> getTablesAndColumnsReferensedOnFile()
        {
            List<DBColumn> tablesAndColumns = new List<DBColumn>();

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = requestsManager.getSelectTablesAndColumnsRequest();  // Запрос для выбора таблиц и столбцов.

                using (DbDataReader reader = command.ExecuteReader())
                {
                    // Заполнение списка таблиц и столбцов.
                    while (reader.Read())
                    {
                        tablesAndColumns.Add(new DBColumn
                        {
                            TableName = reader.GetValue(0).ToString(),
                            ColumnName = reader.GetValue(1).ToString()
                        });
                    }
                }
            }

            return tablesAndColumns;
        }

        // Метод для получения списка неиспользуемых файлов из базы данных.
        List<File> getUnusedFiles()
        {
            List<File> unusedFiles = new List<File>();
            try
            {
                // Получаем список таблиц и столбцов, связанных с файлами.
                List<DBColumn> tablesAndColumns = getTablesAndColumnsReferensedOnFile();
                // Получаем SQL-запрос для поиска неиспользуемых файлов.
                string getUnusedFilesRequest = requestsManager.getSelectUnusedFilesRequest(tablesAndColumns);

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = getUnusedFilesRequest;

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        // Заполнение списка неиспользуемых файлов.
                        while (reader.Read())
                        {
                            unusedFiles.Add(new File(int.Parse(reader.GetValue(0).ToString()), reader.GetValue(1).ToString()));
                        }
                    }
                }
            }
            catch (Npgsql.PostgresException ex)
            {
                // Обработка ошибок при выполнении запроса и логирование сообщения об ошибке.
                Console.WriteLine("Ошибка получения данных: " + ex.Message);
                loger.logData($"\n[{DateTime.Now.ToString()}] Ошибка получения данных: " + ex.Message);
                errorFlag = true;  // Устанавливаем флаг ошибки.
            }
            return unusedFiles;
        }

        // Метод для удаления файлов из базы данных.
        void deleteFilesFromDataBase(List<File> files)
        {
            if (files.Count > 0)
            {
                // Получаем SQL-запрос для удаления файлов.
                string deleteFilesRequest = requestsManager.getDeleteFilesRequest(files);

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteFilesRequest;
                    using (DbDataReader reader = command.ExecuteReader()) { }
                }
                loger.logData($"\n[{DateTime.Now.ToString()}] Записи в базе данных удалены");
            }
        }

        // Метод для удаления файлов из файловой системы.
        void deleteFilesFromSystem(List<File> filesNotInOtherTables)
        {
            if (filesNotInOtherTables != null)
            {
                // Проходим по каждому файлу и удаляем его, если он существует в файловой системе.
                foreach (File file in filesNotInOtherTables)
                {
                    if (System.IO.File.Exists(file.path))
                    {
                        System.IO.File.Delete(file.path);
                        loger.logData($"\n[{DateTime.Now.ToString()}] Удален файл: {file.path}");
                    }
                    else
                    {
                        loger.logData($"\n[{DateTime.Now.ToString()}] Не найден файл: {file.path}");
                    }
                }
            }
        }

        // Метод для вывода информации о файлах, которые будут удалены.
        void printFilesInfo(List<File> files)
        {
            loger.logData($"\n[{DateTime.Now.ToString()}] Количество удаляемых записей = {files.Count}");
            Console.WriteLine($"Количество удаляемых записей = {files.Count}");
            if (files.Count > 0)
            {
                Console.WriteLine("Найденные неиспользуемые файлы: ");
                foreach (File f in files)
                {
                    loger.logData($"\n\tid: {f.file_id}; path:{f.path}");
                    Console.WriteLine($"{f.path}");
                }
            }
        }

        // Основной метод для очистки файловой системы и базы данных.
        public void clear()
        {
            if (!errorFlag)
            {
                // Получаем список неиспользуемых файлов.
                List<File> filesNotInOtherTables = getUnusedFiles();

                // Выводим информацию о файлах.
                printFilesInfo(filesNotInOtherTables);

                // Удаляем файлы из файловой системы.
                deleteFilesFromSystem(filesNotInOtherTables);

                // Удаляем записи о файлах из базы данных.
                deleteFilesFromDataBase(filesNotInOtherTables);

                // Закрываем соединение с базой данных.
                connection.Close();
            }
            // Отправляем лог-файл по электронной почте.
            loger.sendLogFileByEmail();
        }
    }
}
