using System;
namespace Packem.Data.Helpers
{
    public class SmtpHelper
    {
        /// <summary>
        /// SMTP Server / Host Name
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// SSL port number
        /// </summary>
        public int ServerSSLPort { get; set; }

        /// <summary>
        /// From Email Name
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// From Email mail id
        /// </summary>
        public string FromEmail { get; set; }

        /// <summary>
        /// Network credentials username
        /// </summary>
        public string NetworkUsername { get; set; }

        /// <summary>
        /// Network credentials password in plain text
        /// </summary>
        public string NetworkPassword { get; set; }

        public string ToEmails { get; set; }
    }
}

