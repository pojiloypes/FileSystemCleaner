Данная утилита предназначена для удаления из базы данных и файловой системы файлов, неиспользуемых в других записях базы данных.
Выполняются следующие функции:
- подключение к уже имеющейся базе данных
- ввод строки подключения;
- поиск таблиц, ссылающихся на таблицу File;
- поиск неиспользуемых файлов;
- удаление файлов из файловой системы;
- удаление записей из таблицы File;
- логирование хода работы программы;
- отправка файла логирования на указанную почту;
- поддерживаетсчя работа с базами данных на Postgresql и MS SQL Server;

Для возможности работы с другими СУБД необходимо создать новый класс, реализующий интерфейс IDBRequestsManager и дополнить методы setDBRequestsManagerType, setConnectionType, getDms класса Program.
Так же в классе Program необходимо передать в конструктор класса ConfigurationManager путь к конфигурационному файлу. Для имзенения системной почты отправки или получения необходимо отредактировать конфигурационный файл, но есть возможность во время работы программы указать собственные данные.



This utility is designed to delete files unused in other database records from the database and file system.
The following functions are performed:
- connecting to an existing database
- entering a connection string
- search for tables referring to the File table;
- search for unused files
- deleting files from the file system;
- deleting records from the File table;
- logging of the program operation progress;
- sending the logging file to the specified mail;
- Supports working with Postgresql and MS SQL Server databases;

To be able to work with other DBMSs, it is necessary to create a new class that implements the IDBRequestsManager interface and supplement the setDBRequestsManagerType, setConnectionType, getDms methods of the Program class.
Also in the Program class it is necessary to pass the path to the configuration file to the ConfigurationManager class constructor. For system mail sending or receiving it is necessary to edit the configuration file, but it is possible to specify your own data while the program is running.
