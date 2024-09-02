namespace FileSystemCleaner
{
    // Класс File представляет файл, который хранится в файловой системе и связан с записью в базе данных.
    public class File
    {
        // Свойство file_id хранит идентификатор файла в базе данных.
        public int file_id { get; set; }

        // Свойство path хранит путь к файлу в файловой системе.
        public string path { get; set; }

        // Конструктор инициализирует объект File, устанавливая его идентификатор и путь.
        public File(int file_id, string path)
        {
            this.file_id = file_id;
            this.path = path;
        }
    }
}
