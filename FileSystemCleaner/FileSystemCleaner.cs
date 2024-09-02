using System;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace FileSystemCleaner
{
    public class Loger : ILoger
    {
        // Поля для хранения настроек электронной почты и данных логирования
        EmailSettings emailSettings;
        StringBuilder loggingData;

        // Конструктор, принимающий настройки электронной почты и инициализирующий объект для хранения данных логов
        public Loger(EmailSettings emailSettings)
        {
            this.emailSettings = emailSettings;
            loggingData = new StringBuilder();
        }

        // Метод для добавления данных в лог
        public void logData(string data)
        {
            loggingData.Append(data);
        }

        // Метод для создания файла лога, сохранения данных и возврата пути к созданному файлу
        public string createLogFile()
        {
            // Генерация имени файла на основе текущей даты и времени
            string logFileName = DateTime.Now.ToString().Replace(":", "-") + " ProgramWorkLog.txt";

            // Запись данных в файл и очистка буфера логов
            System.IO.File.AppendAllText(logFileName, loggingData.ToString());
            loggingData.Clear();

            // Возврат имени созданного файла
            return logFileName;
        }

        // Метод для установки адреса электронной почты отправителя
        public void setSenderEmail()
        {
            string ans = "";

            // Запрос подтверждения у пользователя на использование системного адреса электронной почты
            while (ans != "Y" && ans != "N")
            {
                Console.Write($"Хотите использовать системный адрес электронной почты ({emailSettings.SenderEmail}) для отправки файла логирования? (Y/N): ");
                ans = Console.ReadLine();
            }

            // Если пользователь соглашается, дальнейший ввод не требуется
            if (ans == "Y")
                return;

            // Иначе, запрос на ввод нового адреса электронной почты и пароля
            Console.Write("Введите почту, с которой будет отправлено письмо: ");
            emailSettings.SenderEmail = Console.ReadLine();

            Console.Write("Введите пароль: ");
            emailSettings.SenderPassword = Console.ReadLine();
        }

        // Метод для установки адреса электронной почты получателя
        public void setRecipientEmail()
        {
            string ans = "";

            // Запрос подтверждения у пользователя на использование системного адреса электронной почты получателя
            while (ans != "Y" && ans != "N")
            {
                Console.Write($"Хотите использовать системный адрес электронной почты ({emailSettings.RecipientEmail}) для получения файла логирования? (Y/N): ");
                ans = Console.ReadLine();
            }

            // Если пользователь соглашается, дальнейший ввод не требуется
            if (ans == "Y")
                return;

            // Иначе, запрос на ввод нового адреса электронной почты получателя
            Console.Write("Введите почту, на которую будет отправлен файл: ");
            emailSettings.RecipientEmail = Console.ReadLine();
        }

        // Метод для установки SMTP-сервера и порта на основе адреса отправителя
        public void setServerAndPort()
        {
            // Определение SMTP-сервера и порта в зависимости от домена электронной почты отправителя
            if (emailSettings.SenderEmail.Contains("yandex"))
            {
                emailSettings.SmtpServer = "smtp.yandex.ru";
                emailSettings.SmtpPort = 25;
            }
            else
            {
                emailSettings.SmtpServer = "smtp.gmail.com";
                emailSettings.SmtpPort = 587;
            }
        }

        // Метод для изменения информации о почте
        public void changeEmailInfo()
        {
            string ans = "";

            // Запрос у пользователя, хочет ли он изменить параметры отправки почты
            while (ans != "Y" && ans != "N")
            {
                Console.Write("Хотите ли вы изменить параметры отправки файла логирования? (Y/N): ");
                ans = Console.ReadLine();
            }

            // Если пользователь не хочет менять параметры, выход из метода
            if (ans == "N")
                return;

            // Иначе, выполнение методов для изменения адресов и сервера
            setSenderEmail();
            setRecipientEmail();
            setServerAndPort();
        }

        // Метод для отправки файла лога по электронной почте
        public void sendLogFileByEmail()
        {
            // Заголовок и тело письма
            string subject = "Работа приложения по очистке системы от неиспользуемых файлов";
            string body = "Прилагается файл с логами";

            // Создание файла лога и получение его пути
            string attachmentPath = createLogFile();
            bool errorFlag = true;

            // Цикл для повторных попыток отправки письма в случае ошибки
            while (errorFlag)
            {
                try
                {
                    // Вызов метода для изменения параметров отправки, если необходимо
                    changeEmailInfo();

                    // Создание объекта MailMessage и настройка параметров письма
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(emailSettings.SenderEmail);
                    mail.To.Add(emailSettings.RecipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;

                    // Добавление файла лога в качестве вложения
                    Attachment attachment = new Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);

                    // Настройка клиента SMTP и отправка письма
                    SmtpClient smtpClient = new SmtpClient(emailSettings.SmtpServer, emailSettings.SmtpPort)
                    {
                        Credentials = new NetworkCredential(emailSettings.SenderEmail, emailSettings.SenderPassword),
                        EnableSsl = true
                    };

                    smtpClient.Send(mail);

                    Console.WriteLine("Письмо успешно отправлено");
                    errorFlag = false; // Завершение цикла в случае успешной отправки
                }
                catch (Exception ex)
                {
                    // Обработка ошибок при отправке письма
                    Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                    Console.Write("Чтобы ввести данные заново, введите \"Y\", чтобы не отправлять письмо, введите \"N\": ");
                    if (Console.ReadLine() == "N")
                        errorFlag = false; // Завершение цикла, если пользователь не хочет повторять попытку
                }
            }
        }
    }
}
