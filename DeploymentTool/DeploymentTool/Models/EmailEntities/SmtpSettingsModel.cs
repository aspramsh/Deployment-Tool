namespace DeploymentTool.Models.EmailEntities
{
    public class SmtpSettingsModel
    {
        /// <summary>
        /// Email Smtp Settings
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Email server Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Email network credentials
        /// </summary>
        public NetworkCredentialsModel NetworkCredentials { get; set; }

        public bool EnableSsl { get; set; } = true;

        public string From { get; set; }

        public string To { get; set; }
    }
}