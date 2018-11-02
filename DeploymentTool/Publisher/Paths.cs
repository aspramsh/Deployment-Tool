namespace Publisher
{
    public class Paths
    {
        /// <summary>
        /// Project's .Sln folder
        /// </summary>
        public string Solution { get; set; }

        /// <summary>
        /// Publish destination
        /// </summary>
        public string Publish { get; set; }

        /// <summary>
        /// IIS application pool where the project is deployed
        /// </summary>
        public string IISAppPoolName { get; set; }
        /// <summary>
        /// default constructor
        /// </summary>
        public Paths()
        {
        }

        /// <summary>
        /// parameterfull constructor
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="publish"></param>
        /// <param name="iisAppPoolName"></param>
        public Paths(string solution, string publish, string iisAppPoolName)
        {
            this.Solution = solution;
            this.Publish = publish;
            this.IISAppPoolName = iisAppPoolName;
        }
    }
}
