using Microsoft.Data.SqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FileSystemCleaner
{
    class FileSystemCleaner
    {
        IDBRequestsManager requestsManager;
        DbConnection connection; 
        public bool errorFlag { get; set; }

        public FileSystemCleaner()
        {
            Loger.run();
            setConnectionString(); 
        }

        DbConnection getDbConnection(string connectionString)
        {
            if (connectionString.Contains("Host=") && connectionString.Contains("Username="))
            {
                //PostgreSQL
                requestsManager = new PostgreSqlRequestsManager();
                return new NpgsqlConnection(connectionString);
            }
            else if (connectionString.Contains("Server=") && connectionString.Contains("Database="))
            {
                // MS SQL Server
                requestsManager = new MSSqlServerRequestsManager();
                return new SqlConnection(connectionString);
            }

            //Не удалось определить тип базы данных
            return null;
        }

        public void setConnectionString()
        {
            Console.Write("Введите строку подключения: ");
            string connectionString = Console.ReadLine();

            try
            {
                Loger.logData($"[{DateTime.Now.ToString()}] Попытка подключения к базе данных с параметрами:\n\t{connectionString.Replace(";","\n\t")}");
                connection = getDbConnection(connectionString);
                if (connection == null)
                {
                    Loger.logData($"\n[{DateTime.Now.ToString()}] Работа с данной СУБД в данный момент не поддерживается.");
                    Console.WriteLine("Работа с данной СУБД в данный момент не поддерживается.");
                    errorFlag = true;
                }   
                else
                    connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникла ошибка: " + ex.Message);
                Loger.logData($"\n[{DateTime.Now.ToString()}] Возникла ошибка при подключении: " + ex.Message);
                errorFlag = true;
            }
        }

        List<Tuple<string, string>> getTablesAndColumnsReferensedOnFile()
        {
            List<Tuple<string, string>> tablesAndColumns = new List<Tuple<string, string>>();

            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = requestsManager.getSelectTablesAndColumnsRequest();

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tablesAndColumns.Add(new Tuple<string, string>(reader.GetValue(0).ToString(), reader.GetValue(1).ToString()));
                    }
                }
            }

            return tablesAndColumns;
        }

        List<File> getUnusedFiles()
        {
            List<File> unusedFiles = new List<File>();
            try
            {
                List<Tuple<string, string>> tablesAndColumns = getTablesAndColumnsReferensedOnFile();
                string getUnusedFilesRequest = requestsManager.getSelectUnusedFilesRequest(tablesAndColumns);

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = getUnusedFilesRequest;

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            unusedFiles.Add(new File(int.Parse(reader.GetValue(0).ToString()), reader.GetValue(1).ToString()));
                        }
                    }
                }
            }
            catch (Npgsql.PostgresException ex)
            {
                Console.WriteLine("Ошибка получения данных: " + ex.Message);
                Loger.logData($"\n[{DateTime.Now.ToString()}] Ошибка получения данных: " + ex.Message);
                errorFlag = true;
            }
            return unusedFiles;
        }

        void deleteFilesFromDataBase(List<File> files)
        {
            if (files.Count > 0)
            {
                string deleteFilesRequest = requestsManager.getDeleteFilesRequest(files);

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = deleteFilesRequest;
                    using (DbDataReader reader = command.ExecuteReader()) { }     
                }
                Loger.logData($"\n[{DateTime.Now.ToString()}] Записи в базе данных удалены");
            }
        }

        void deleteFilesFromSystem(List<File> filesNotInOtherTables)
        {
            if (filesNotInOtherTables != null)
            {
                foreach (File file in filesNotInOtherTables)
                {
                    if (System.IO.File.Exists(file.path))
                    {
                        System.IO.File.Delete(file.path);
                        Loger.logData($"\n[{DateTime.Now.ToString()}] Удален файл: {file.path}");
                    }
                    else 
                    {
                        Loger.logData($"\n[{DateTime.Now.ToString()}] Не найден файл: {file.path}");
                    }
                }
            }
        }

        void printFilesInfo(List<File> files)
        {
            Loger.logData($"\n[{DateTime.Now.ToString()}] Количество удаляемых записей = {files.Count}");
            Console.WriteLine($"Количество удаляемых записей = {files.Count}");
            if (files.Count > 0)
            {
                Console.WriteLine("Найденные неиспользуемые файлы: ");
                foreach (File f in files)
                {
                    Loger.logData($"\n\tid: {f.file_id}; path:{f.path}");
                    Console.WriteLine($"{f.path}");
                }
            }
        }

        public void clear()
        {
            if (!errorFlag)
            {
                List<File> filesNotInOtherTables = getUnusedFiles();

                printFilesInfo(filesNotInOtherTables);
                deleteFilesFromSystem(filesNotInOtherTables);
                deleteFilesFromDataBase(filesNotInOtherTables);
                connection.Close();
            }
            Loger.sendLogFileByEmail();
            
        }
    }
}
