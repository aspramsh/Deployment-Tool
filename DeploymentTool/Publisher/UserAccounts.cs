namespace Publisher
{
    public class UserAccounts
    {
        /// <summary>
        /// The name of the computer where project is deployed
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// Server's local admin user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Server's Administrator password
        /// </summary>
        public string Password { get; set; }
    }
}
