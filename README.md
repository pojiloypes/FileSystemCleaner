Данная утилита предназначена для удаления из базы данных и файловой системы файлов, неиспользуемых в других записях базы данных.
Выполняются следующие функции:
- ввод строки подключения;
- поиск таблиц, ссылающихся на таблицу File;
- поиск неиспользуемых файлов;
- удаление файлов из файловой системы;
- удаление записей из таблицы File;
- логирование хода работы программы;
- отправка файла логирования на указанную почту;
- поддерживаетсчя работа с базами данных на Postgresql и MS SQL Server;
Для возможности работы с другими СУБД необходимо создать новый класс, наследующий интерфейс IDBRequestsManager и дополнить метод getDbConnection класса FileSystemCleaner.



This utility is designed to delete files unused in other database records from the database and file system.
The following functions are performed:
- entering a connection string;
- search for tables referring to the File table;
- search for unused files;
- deleting files from the file system;
- deleting records from the File table;
- logging of program operation;
- sending the logging file to the specified e-mail;
- Supports working with Postgresql and MS SQL Server databases;
To be able to work with other DBMSs, it is necessary to create a new class inheriting the IDBRequestsManager interface and supplement the getDbConnection method of the FileSystemCleaner class.
