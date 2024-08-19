using System;
using System.Collections.Generic;

namespace FileSystemCleaner
{
    public interface IDBRequestsManager
    {
        public string getSelectTablesAndColumnsRequest();
        public string getSelectUnusedFilesRequest(List<Tuple<string, string>> tablesAndColumns);
        public string getDeleteFilesRequest(List<File> files);
    }
}
