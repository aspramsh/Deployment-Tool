using System.Net;
using System.Net.Mail;

namespace DeploymentTool.Models.EmailEntities
{
    public class EmailSmtpClient : IEmailSmtpClient
    {
        private readonly MailAddress _from;
        private readonly ISmtpClient _client;

        /// <summary>
        /// A constructor for Email Smtp Client
        /// </summary>
        public EmailSmtpClient(SmtpSettingsModel smtpSettings)
        {
            _client = new SmtpClientWrapper
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpSettings.NetworkCredentials.Username,
                 smtpSettings.NetworkCredentials.Password),

                EnableSsl = smtpSettings.EnableSsl,
                Port = smtpSettings.Port,
                Host = smtpSettings.Host
            };

            _from = new MailAddress(smtpSettings.From);
        }

        public EmailSmtpClient(ISmtpClient client, string from)
        {
            _client = client;
            _from = new MailAddress(from);
        }
        public void Send<T>(MessageModel messageModel, T bodyModel)
        {
            var mailMessage = CreateEmailMessage(messageModel);

            mailMessage.Body = ReplaceRendererHelper.Parse(messageModel.Body, bodyModel);
            
            _client.Send(mailMessage);

            // Dispose unmanaged resource
            mailMessage.Dispose();
        }

        private MailMessage CreateEmailMessage(MessageModel messageModel)
        {
            var mailMessage = new MailMessage
            {
                From = _from,
                Subject = messageModel.Subject,
                Priority = messageModel.Priority,
                IsBodyHtml = messageModel.IsBodyHtml
            };

            foreach (var to in messageModel.ToAddresses)
            {
                mailMessage.To.Add(to);
            }

            if (messageModel.CcAddresses != null)
            {
                foreach (var cc in messageModel.CcAddresses)
                {
                    mailMessage.To.Add(cc);
                }
            }

            if (messageModel.BccAddresses != null)
            {
                foreach (var bcc in messageModel.BccAddresses)
                {
                    mailMessage.To.Add(bcc);
                }
            }

            if (messageModel.Attachments != null)
            {
                foreach (var attachment in messageModel.Attachments)
                {
                    mailMessage.Attachments.Add(attachment);
                }
            }

            return mailMessage;
        }
    }
}
