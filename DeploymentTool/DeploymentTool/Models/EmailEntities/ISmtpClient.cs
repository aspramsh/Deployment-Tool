using System.Net;
using System.Net.Mail;

namespace DeploymentTool.Models.EmailEntities
{
    public interface ISmtpClient
    {
        /// <summary>
        /// smtp client port
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// email and password
        /// </summary>
        ICredentialsByHost Credentials { get; set; }

        /// <summary>
        /// enable SSL
        /// </summary>
        bool EnableSsl { get; set; }

        void Send(MailMessage message);
    }
}