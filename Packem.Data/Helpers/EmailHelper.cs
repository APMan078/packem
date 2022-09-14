using System;
namespace Packem.Data.Helpers
{
    /// <summary>
    /// Email object helper
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// From Email
        /// </summary>
        public EmailNameAddress FromEmail;

        /// <summary>
        /// To Email (array for multiple address)
        /// </summary>
        public EmailNameAddress[] ToEmail;

        /// <summary>
        /// CC Email (array for multiple address)
        /// </summary>
        public EmailNameAddress[] CCEmail;

        /// <summary>
        /// BCC Email (array for multiple address)
        /// </summary>
        public EmailNameAddress[] BCCEmail;

        /// <summary>
        /// Email Body
        /// </summary>
        public string Body;

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject;

        /// <summary>
        /// Email Attachement (array for multiple attachement)
        /// </summary>
        public EmailAttachement[] Attachements;

        /// <summary>
        /// Default Init function
        /// </summary>
        public EmailHelper()
        {
            FromEmail = new EmailNameAddress();
            ToEmail = new EmailNameAddress[0];
            CCEmail = new EmailNameAddress[0];
            BCCEmail = new EmailNameAddress[0];
            Body = System.String.Empty;
            Subject = System.String.Empty;
            Attachements = new EmailAttachement[0];
        }

        /// <summary>
        /// Default function to clear data of function and release memory to  system
        /// </summary>
        ~EmailHelper()
        {
            FromEmail = null;
            ToEmail = null;
            CCEmail = null;
            BCCEmail = null;
            Body = null;
            Subject = null;
            Attachements = null;
            System.GC.Collect();
        }
    }

        /// <summary>
        /// Email address object
        /// </summary>
        public class EmailNameAddress
        {
            /// <summary>
            /// Email Address Name
            /// </summary>
            public string Name;

            /// <summary>
            /// Email Address Id
            /// </summary>
            public string EmailId;

            /// <summary>
            /// Default init function
            /// </summary>
            public EmailNameAddress()
            {
                Name = System.String.Empty;
                EmailId = System.String.Empty;
            }

            /// <summary>
            /// Default function to clear data of function and release memory to  system
            /// </summary>
            ~EmailNameAddress()
            {
                Name = null;
                EmailId = null;
                System.GC.Collect();
            }
        }

        /// <summary>
        /// Email attachements object
        /// </summary>
        public class EmailAttachement
        {
            /// <summary>
            /// Attachement name to be shown in email
            /// </summary>
            public string AttachementName;

            /// <summary>
            /// Attachement type
            /// </summary>
            public string AttachementType;

            /// <summary>
            /// Physical Loction of the attachement while attaching to email.
            /// Software will read from this location and it will be attached to the mail as attachement
            /// </summary>
            public string AttachementLocation;

            /// <summary>
            /// Default init function
            /// </summary>
            public EmailAttachement()
            {
                AttachementName = System.String.Empty;
                AttachementType = System.String.Empty;
                AttachementLocation = System.String.Empty;
            }

            /// <summary>
            /// Default function to clear data of function and release memory to  system
            /// </summary>
            ~EmailAttachement()
            {
                AttachementName = null;
                AttachementType = null;
                AttachementLocation = null;
                System.GC.Collect();
        }
    }
}

