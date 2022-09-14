using System.Net.Mail;
using Packem.Data.Helpers;

namespace Packem.Data.Interfaces
{
    public interface IEmailService
    {
        string SendEmail(string Body, string Subject, EmailNameAddress[] To, EmailNameAddress[] CC, EmailNameAddress[] BCC, EmailAttachement[] Attachment, SmtpHelper smpt);

        string SendEmail(AlternateView body, string subject, EmailNameAddress[] to, SmtpHelper smtp, EmailNameAddress[] cc = null, EmailNameAddress[] bcc = null, EmailAttachement[] attachment = null);

    }
}

