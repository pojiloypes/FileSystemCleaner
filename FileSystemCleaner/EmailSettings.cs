namespace FileSystemCleaner
{
    // Класс EmailSettings хранит настройки для отправки электронной почты.
    public class EmailSettings
    {
        // Свойство SenderEmail хранит адрес электронной почты отправителя.
        public string SenderEmail { get; set; }

        // Свойство SenderPassword хранит пароль для учетной записи отправителя.
        public string SenderPassword { get; set; }

        // Свойство RecipientEmail хранит адрес электронной почты получателя.
        public string RecipientEmail { get; set; }

        // Свойство SmtpServer хранит адрес SMTP-сервера для отправки почты.
        public string SmtpServer { get; set; }

        // Свойство SmtpPort хранит порт для подключения к SMTP-серверу.
        public int SmtpPort { get; set; }
    }
}
