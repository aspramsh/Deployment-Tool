namespace Publisher
{
    /// <summary>
    /// Parameters class for git actions
    /// </summary>
    public class Params
    {
        /// <summary>
        /// Path of project files
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// Git repository branch name
        /// </summary>
        public string BranchName { get; set; }

        /// <summary>
        /// Git repository name
        /// </summary>
        public string RepoName { get; set; }

        public string WebsiteName { get; set; }

        /// <summary>
        /// parameterless constructor
        /// </summary>
        public Params()
        {
        }

        /// <summary>
        /// A constructor for faster initialisation
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="branchName"></param>
        /// <param name="repoName"></param>
        /// <param name="websiteName"></param>
        public Params(string projectPath, string branchName, string repoName, string websiteName)
        {
            ProjectPath = projectPath;
            BranchName = branchName;
            RepoName = repoName;
            WebsiteName = websiteName;
        }
    }
}
