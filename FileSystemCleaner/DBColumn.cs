namespace FileSystemCleaner
{
    // Класс DBColumn представляет столбец базы данных, включая имя таблицы и имя столбца.
    public class DBColumn
    {
        // Свойство TableName хранит имя таблицы, в которой находится столбец.
        public string TableName { get; set; }

        // Свойство ColumnName хранит имя столбца в таблице базы данных.
        public string ColumnName { get; set; }
    }
}
