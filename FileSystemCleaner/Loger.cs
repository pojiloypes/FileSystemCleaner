using System;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace FileSystemCleaner
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string RecipientEmail { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
    }

    static public class Loger
    {
        static StringBuilder loggingData;

        static public void run()
        {
            loggingData = new StringBuilder();
        }

        static public void logData(string data)
        {
            loggingData.Append(data);
        }
        static string createLogFile()
        {
            string logFileName = DateTime.Now.ToString().Replace(":", "-") + " ProgramWorkLog.txt";
            System.IO.File.AppendAllText(logFileName, loggingData.ToString());
            loggingData.Clear();
            return logFileName;
        }

        static void setEmailInfo(EmailSettings emailSettings)
        {
            Console.Write("Введите почту с которой будет отправлено письмо: ");
            emailSettings.SenderEmail = Console.ReadLine();

            Console.Write("Введите пароль: ");
            emailSettings.SenderPassword = Console.ReadLine();

            Console.Write("Введите почту на которую будет отправлено письмо: ");
            emailSettings.RecipientEmail = Console.ReadLine();
        }

        static void setServerAndPort(EmailSettings emailSettings)
        {
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

        static public void sendLogFileByEmail()
        {
            EmailSettings emailSettings = new EmailSettings();

            string subject = "Работа приложения по очистке системы от неиспользуемых файлов"; 
            string body = "Прилагается файл с логами";
            string attachmentPath = createLogFile();
            bool errorFlag = true;

            while(errorFlag)
            {
                try
                {
                    setEmailInfo(emailSettings);
                    setServerAndPort(emailSettings);

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(emailSettings.SenderEmail);
                    mail.To.Add(emailSettings.RecipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;

                    Attachment attachment = new Attachment(attachmentPath);
                    mail.Attachments.Add(attachment);

                    SmtpClient smtpClient = new SmtpClient(emailSettings.SmtpServer, emailSettings.SmtpPort)
                    {
                        Credentials = new NetworkCredential(emailSettings.SenderEmail, emailSettings.SenderPassword),
                        EnableSsl = true
                    };

                    smtpClient.Send(mail);

                    Console.WriteLine("Письмо успешно отправлено");
                    errorFlag = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                    Console.WriteLine("Чтобы ввести данные заново введите \"Y\", чтобы не отправлять письмо введите \"N\"");
                    if (Console.ReadLine() == "N")
                        errorFlag = false;
                }
            } 
        }
    }
}
