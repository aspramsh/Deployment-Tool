using Microsoft.AspNetCore.Hosting;
using Publisher;

namespace DeploymentTool.Models.EmailEntities
{
    public class AppSettings
    {
        public Projects Projects { get; set; }

        public FoldersNotToDelete FoldersNotToDelete { get; set; }

        public SmtpSettingsModel SmtpSettingsModel { get; set; }

        public UserAccounts UserAccounts { get; set; }

        public NetworkCredentialsModel NetworkCredentialsModel { get; set; }

        public CurrentEnvironment Link { get; set; }

        public string GetPasswordLink(IHostingEnvironment env)
        {
            if (env == null)
                return Link.Development_IISExpress;

            switch (env.EnvironmentName)
            {
                case "Development":
                    return Link.Development;
                case "Production":
                    return Link.Production;
                case "Development_IISExpress":
                    return Link.Development_IISExpress;
            }

            return Link.Development_IISExpress;
        }
    }
}
