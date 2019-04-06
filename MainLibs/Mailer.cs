using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace TejiAdesa.MainLibs
{
    public static class Mailer
    {
        public static void Send(string to, List<String> bCcs, string from, string subject, string body, string smtp)
        {
            try
            {
                MailAddress fromMA = new MailAddress(from);
                MailAddress toMA = new MailAddress(to);
                MailMessage mail = new MailMessage(fromMA, toMA);
                bCcs.AsParallel().ForAll<String>(bcc => mail.Bcc.Add(bcc));

                mail.Subject = subject;
                mail.Body = body;

                SmtpClient smtpC = new SmtpClient(smtp);
                smtpC.Send(mail);
            }
            catch (ArgumentNullException argNullEx)
            { }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Send(string to, string from, string subject, string body, string smtp)
        {
            try
            {
                MailAddress fromMA = new MailAddress(from);
                MailAddress toMA = new MailAddress(to);
                MailMessage mail = new MailMessage(fromMA, toMA);

                mail.Subject = subject;
                mail.Body = body;

                SmtpClient smtpC = new SmtpClient(smtp);
                smtpC.Send(mail);
            }
            catch (NullReferenceException nullEx)
            { }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
