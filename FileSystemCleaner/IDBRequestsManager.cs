using System;
using System.Collections.Generic;

namespace FileSystemCleaner
{
    // Интерфейс IDBRequestsManager определяет набор методов, которые должны быть реализованы для работы с запросами к базе данных.
    public interface IDBRequestsManager
    {
        // Метод для получения SQL-запроса, который выбирает таблицы и столбцы, используемые в приложении.
        public string getSelectTablesAndColumnsRequest();

        // Метод для получения SQL-запроса, который выбирает неиспользуемые файлы на основе списка таблиц и столбцов.
        public string getSelectUnusedFilesRequest(List<DBColumn> tablesAndColumns);

        // Метод для получения SQL-запроса, который удаляет файлы из базы данных на основе списка файлов.
        public string getDeleteFilesRequest(List<File> files);
    }
}
