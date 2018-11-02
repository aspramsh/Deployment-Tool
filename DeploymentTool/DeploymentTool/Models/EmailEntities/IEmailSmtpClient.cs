namespace DeploymentTool.Models.EmailEntities
{
    public interface IEmailSmtpClient
    {
        /// <summary>
        /// Email message model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageModel"></param>
        /// <param name="bodyModel"></param>
        void Send<T>(MessageModel messageModel, T bodyModel);
    }
}
