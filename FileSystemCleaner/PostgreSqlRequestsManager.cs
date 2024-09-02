using System;
using System.Collections.Generic;

namespace FileSystemCleaner
{
    // Класс реализующий интерфейс IDBRequestsManager для работы с СУБД PostgreSQL
    internal class PostgreSqlRequestsManager : IDBRequestsManager
    {
        public string getSelectTablesAndColumnsRequest()
        {
            return "SELECT\r\n    tc.table_name AS referencing_table,\r\n    kcu.column_name AS referencing_column\r\nFROM\r\n    information_schema.table_constraints AS tc\r\nJOIN\r\n    information_schema.key_column_usage AS kcu\r\n    ON tc.constraint_name = kcu.constraint_name\r\n    AND tc.table_schema = kcu.table_schema\r\nJOIN\r\n    information_schema.constraint_column_usage AS ccu\r\n    ON ccu.constraint_name = tc.constraint_name\r\nWHERE\r\n    tc.constraint_type = 'FOREIGN KEY'\r\n    AND ccu.table_name = 'file'\r\n    AND ccu.column_name = 'file_id';";
        }

        public string getSelectUnusedFilesRequest(List<DBColumn> tablesAndColumns)
        {
            string res = "select file_id, path from file";
            if (tablesAndColumns.Count == 0)
                return res + ";";

            res += " WHERE file_id NOT IN (";
            for (int i = 0; i < tablesAndColumns.Count; i++)
            {
                res += "SELECT " + tablesAndColumns[i].ColumnName + " FROM " + tablesAndColumns[i].TableName;
                if (i < tablesAndColumns.Count - 1)
                    res += " UNION ";
                else
                    res += ");";
            }
            return res;
        }

        public string getDeleteFilesRequest(List<File> files)
        {
            string res = "delete from file where";
            if (files.Count == 0)
                return res + " true";

            res += " file_id in (";
            for (int i = 0; i < files.Count; i++)
            {
                res += files[i].file_id;
                if (i < files.Count - 1)
                    res += ",  ";
                else
                    res += ");";
            }

            return res;
        }
    }
}
