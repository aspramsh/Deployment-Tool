using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class DeploymentSpecification
    {
        [Key]
        public int Id { get; set; }

        public string AppPoolName { get; set; }

        public string DeploymentPath { get; set; }

        public string ProjectName { get; set; }

        public string WebsiteName { get; set; }

        public string BranchName { get; set; }

        [ForeignKey("TypeId")]
        public Type Type { get; set; }

        public int TypeId { get; set; }

        public string ProjectPath { get; set; }
    }
}
