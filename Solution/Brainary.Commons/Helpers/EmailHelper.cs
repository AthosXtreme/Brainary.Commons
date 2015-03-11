namespace Brainary.Commons.Helpers
{
    using System.Linq;
    using System.Net.Mail;

    public class EmailHelper
    {
        public static void SmtpSend(string to, string subject, string body, params Attachment[] attachments)
        {
            SmtpSend(new MailAddress(to), null, null, subject, body, attachments);
        }

        public static void SmtpSend(string to, string cc, string subject, string body, params Attachment[] attachments)
        {
            SmtpSend(new MailAddress(to), new MailAddress(cc), null, subject, body, attachments);
        }

        public static void SmtpSend(MailAddress to, string subject, string body, params Attachment[] attachments)
        {
            SmtpSend(to, null, null, subject, body, attachments);
        }

        public static void SmtpSend(MailAddress to, MailAddress cc, string subject, string body, params Attachment[] attachments)
        {
            SmtpSend(to, cc, null, subject, body, attachments);
        }

        public static void SmtpSend(MailAddress to, MailAddress cc, MailAddress bcc, string subject, string body, params Attachment[] attachments)
        {
            var message = new MailMessage { Subject = subject, Body = body, IsBodyHtml = true };
            message.To.Add(to);
            if (cc != null) message.CC.Add(cc);
            if (bcc != null) message.Bcc.Add(body);
            if (!attachments.Any()) return;
            foreach (var a in attachments) message.Attachments.Add(a);

            var client = new SmtpClient();
            client.Send(message);
        }
    }
}
