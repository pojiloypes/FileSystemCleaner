namespace FileSystemCleaner
{
    // Интерфейс ILoger определяет контракт для логгера, который может записывать данные в лог, 
    // создавать файлы логов и отправлять их по электронной почте.
    public interface ILoger
    {
        // Метод для записи данных в лог
        public void logData(string data);

        // Метод для создания файла лога и возврата пути к нему
        public string createLogFile();

        // Метод для установки адреса электронной почты отправителя и пароля
        public void setSenderEmail();

        // Метод для установки адреса электронной почты получателя
        public void setRecipientEmail();

        // Метод для установки сервера и порта для отправки электронной почты
        public void setServerAndPort();

        // Метод для изменения информации о почте, если это необходимо
        public void changeEmailInfo();

        // Метод для отправки файла лога по электронной почте
        public void sendLogFileByEmail();
    }
}
