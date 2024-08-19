using System;
using System.Collections.Generic;

namespace FileSystemCleaner
{
    internal class MSSqlServerRequestsManager : IDBRequestsManager
    {
        public string getSelectTablesAndColumnsRequest()
        {
            return "SELECT\r\n    tc.TABLE_NAME AS referencing_table,\r\n    kcu.COLUMN_NAME AS referencing_column\r\nFROM\r\n    INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc\r\nJOIN\r\n    INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu\r\n    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME \r\n    AND tc.TABLE_SCHEMA = kcu.TABLE_SCHEMA\r\nJOIN\r\n    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS rc\r\n    ON tc.CONSTRAINT_NAME = rc.CONSTRAINT_NAME\r\nJOIN\r\n    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS ccu\r\n    ON rc.UNIQUE_CONSTRAINT_NAME = ccu.CONSTRAINT_NAME\r\nWHERE\r\n    tc.CONSTRAINT_TYPE = 'FOREIGN KEY'\r\n    AND ccu.TABLE_NAME = 'file'\r\n    AND ccu.COLUMN_NAME = 'file_id';";
        }

        public string getSelectUnusedFilesRequest(List<Tuple<string, string>> tablesAndColumns)
        {
            string res = "select file_id, path from[file]";
            if (tablesAndColumns.Count == 0)
                return res + ";";

            res += " WHERE file_id NOT IN (";
            for (int i = 0; i < tablesAndColumns.Count; i++)
            {
                res += "SELECT " + tablesAndColumns[i].Item2 + " FROM [" + tablesAndColumns[i].Item1 + "]";
                if (i < tablesAndColumns.Count - 1)
                    res += " UNION ";
                else
                    res += ");";
            }
            return res;
        }

        public string getDeleteFilesRequest(List<File> files)
        {
            string res = "delete from [file] where";
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
