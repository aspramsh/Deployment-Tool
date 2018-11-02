namespace Publisher
{
    /// <summary>
    /// Projects class to add constant strings as configuration
    /// </summary>
    public class Projects
    {
        /// <summary>
        /// project name property
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// A property for keeping powershell script path
        /// </summary>
        public string PowershellScriptPath { get; set; }

        /// <summary>
        /// Project's default path
        /// </summary>
        public string InitialPublishPath { get; set; }

        /// <summary>
        /// Sql script path
        /// </summary>
        public string SqlScriptPath { get; set; }

        /// <summary>
        /// Connection string for executing sql script
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
