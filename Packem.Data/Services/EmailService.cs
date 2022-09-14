using System;
using System.Text;
using Packem.Data.Interfaces;
using Packem.Data.Helpers;
using System.Net.Mail;

namespace Packem.Data.Services
{
    public class EmailService : IEmailService
    {
        private readonly IExceptionService _exceptionService;

        public MailMessage Eml;

        /// <summary>
        /// Default function to clear memory and release to system
        /// </summary>
        ~EmailService()
        {
            Eml = null;
            GC.Collect();
        }

        /// <summary>
        /// Sends an email containing a string (simple plain text email)
        /// </summary>
        /// <param name="Body"></param>
        /// <param name="Subject"></param>
        /// <param name="To"></param>
        /// <param name="CC"></param>
        /// <param name="BCC"></param>
        /// <param name="Attachment"></param>
        /// <returns></returns>
        public string SendEmail(string Body, string Subject, EmailNameAddress[] To, EmailNameAddress[] CC, EmailNameAddress[] BCC, EmailAttachement[] Attachment, SmtpHelper smtp)
        {
            if (string.IsNullOrEmpty(Body) || string.IsNullOrWhiteSpace(Body))
            {
                return "Body cannot be null or blank";
            }
            else if (string.IsNullOrEmpty(Subject) || string.IsNullOrWhiteSpace(Subject))
            {
                return "Subject cannot be null or blank";
            }
            else if (To.Length == 0)
            {
                return "Please Add 1 address to send mail";
            }

            if (smtp == null)
            {
                return "SMTP server details not found";
            }

            Eml = new MailMessage();
            foreach (EmailNameAddress EmlAddr in To)
            {
                Eml.To.Add(new MailAddress(EmlAddr.EmailId.Trim(), EmlAddr.Name.Trim(), Encoding.UTF8));
            }

            foreach (EmailNameAddress EmlAddr in CC)
            {
                Eml.CC.Add(new MailAddress(EmlAddr.EmailId.Trim(), EmlAddr.Name.Trim(), Encoding.UTF8));
            }

            foreach (EmailNameAddress EmlAddr in BCC)
            {
                Eml.Bcc.Add(new MailAddress(EmlAddr.EmailId.Trim(), EmlAddr.Name.Trim(), Encoding.UTF8));
            }

            try
            {
                Eml.Body = Body;
                Eml.BodyEncoding = Encoding.UTF8;
                Eml.DeliveryNotificationOptions = DeliveryNotificationOptions.None;
                Eml.From = new MailAddress(smtp.FromEmail.Trim(), smtp.FromName.Trim(), Encoding.UTF8);
                Eml.IsBodyHtml = true;
                Eml.Priority = MailPriority.Normal;
                Eml.Subject = Subject.Trim();
                Eml.SubjectEncoding = Encoding.UTF8;

                SmtpClient Smtp = new SmtpClient();
                Smtp.DeliveryFormat = SmtpDeliveryFormat.International;
                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                Smtp.EnableSsl = true;
                Smtp.Host = smtp.ServerName.Trim();
                Smtp.Port = smtp.ServerSSLPort;
                Smtp.UseDefaultCredentials = false;
                Smtp.Credentials = new System.Net.NetworkCredential(smtp.NetworkUsername.Trim(), smtp.NetworkPassword.Trim());
                Smtp.SendAsync(Eml, null);

                return "Success";
            }
            catch (Exception)
            {
                throw;
            }


        }

        /// <summary>
        /// Sends an email based on a email template. Used for email verification and forgot password
        /// </summary>
        /// <param name="body">Email template</param>
        /// <param name="subject">Email subject</param>
        /// <param name="to">Email Recipient</param>
        /// <param name="smtp"Smtp settings></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public string SendEmail(AlternateView body, string subject, EmailNameAddress[] to, SmtpHelper smtp, EmailNameAddress[] cc = null, EmailNameAddress[] bcc = null, EmailAttachement[] attachment = null)
        {
            Eml = new MailMessage();
            foreach (EmailNameAddress EmlAddr in to)
            {
                Eml.To.Add(new MailAddress(EmlAddr.EmailId.Trim(), EmlAddr.Name.Trim(), Encoding.UTF8));
            }
            try
            {
                Eml.AlternateViews.Add(body);
                Eml.BodyEncoding = Encoding.UTF8;
                Eml.DeliveryNotificationOptions = DeliveryNotificationOptions.None;
                Eml.From = new MailAddress(smtp.FromEmail.Trim(), smtp.FromName.Trim(), Encoding.UTF8);
                Eml.IsBodyHtml = true;
                Eml.Priority = MailPriority.Normal;
                Eml.Subject = subject.Trim();
                Eml.SubjectEncoding = Encoding.UTF8;

                SmtpClient Smtp = new SmtpClient();
                Smtp.DeliveryFormat = SmtpDeliveryFormat.International;
                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                Smtp.EnableSsl = true;
                Smtp.Host = smtp.ServerName.Trim();
                Smtp.Port = smtp.ServerSSLPort;
                Smtp.UseDefaultCredentials = false;
                Smtp.Credentials = new System.Net.NetworkCredential(smtp.NetworkUsername.Trim(), smtp.NetworkPassword.Trim());
                Smtp.SendAsync(Eml, null);

                return "Success";
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}

