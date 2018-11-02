using System.Collections.Generic;
using System.Net.Mail;

namespace DeploymentTool.Models.EmailEntities
{
    public class MessageModel
    {
        public List<string> ToAddresses { get; set; }

        public List<string> CcAddresses { get; set; }

        public List<string> BccAddresses { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public AttachmentCollection Attachments { get; set; }

        public bool IsBodyHtml { get; set; }

        public MailPriority Priority { get; set; }
    }
}