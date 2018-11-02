using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class Type
    {
        [Key]
        public int Id { get; set; }
        public Type()
        {
            DeploymentSpecification = new HashSet<DeploymentSpecification>();
        }

        public string Name { get; set; }

        public ICollection<DeploymentSpecification> DeploymentSpecification { get; set; }
    }
}
