using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace BeeSafe
{
    class Emailer
    {
        private SmtpClient smtp;
        private MailAddress sender;
        string clientHost = String.Empty, username = String.Empty, password = String.Empty, receiverEmail = String.Empty;

        private static Emailer instance;

        private Emailer()
        {

            string[] lines = File.ReadAllLines($"{Application.StartupPath}\\MailSettings.txt");
            foreach (string line in lines)
            {
                string[] splited = line.Split(':');
                if (splited[0] == "SMTP")
                {
                    clientHost = splited[1];
                }
                else if (splited[0] == "From")
                {
                    username = splited[1];
                }
                else if (splited[0] == "To")
                {
                    receiverEmail = splited[1];
                }
                else if (splited[0] == "Password")
                {
                    password = splited[1];
                }
            }

            this.smtp = new SmtpClient(clientHost, 25);

            smtp.Credentials = new NetworkCredential(username, password);
            smtp.EnableSsl = true;

            this.sender = new MailAddress(username);

        }



        public static Emailer getInstance()
        {
            if (instance == null)
            {
                instance = new Emailer();
            }

            return instance;
        }

        private void sendMessageToClientWithSubject(String message, String clientEmail, String subject)
        {
            MailMessage mail = new MailMessage();

            mail.From = this.sender;
            mail.To.Add(clientEmail);
            mail.Subject = subject;
            mail.Body = message;

            //this.smtp.Send(mail);
        }

        public void sendMessage(String message, String clientEmail)
        {
            this.sendMessageToClientWithSubject(message, clientEmail, "Default subject");
        }

        public void logException(Exception exception)
        {
            this.sendMessageToClientWithSubject(exception.ToString(), receiverEmail, "Error on BeeSafe");
        }

        public void logOnLiquidEnded()
        {
            this.sendMessageToClientWithSubject($"Жидкость закончена", receiverEmail, "Жидкость закончилась.");
        }
    }
}