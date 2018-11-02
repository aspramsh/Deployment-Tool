using System.ComponentModel.DataAnnotations;


namespace DeploymentTool.Models
{
    public class DeploymentSpecificationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Project Path is Required")]
        [Display(Name = "Project Path")]
        [RegularExpression(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$", ErrorMessage = "Path is Not Valid.")]
        public string ProjectPath { get; set; }

        [Required(ErrorMessage = "The Application Pool Name is Required")]
        [Display(Name = "Application Pool Name")]
        public string AppPoolName { get; set; }

        [Required(ErrorMessage = "The Deployment Path is Required")]
        [Display(Name = "Deployment Path")]
        [RegularExpression(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$", ErrorMessage = "Path is Not Valid.")]
        public string DeploymentPath { get; set; }

        [Required(ErrorMessage = "The Project Name is Required")]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "The Website Name is Required")]
        [Display(Name = "Website Name")]
        public string WebsiteName { get; set; }

        [Required(ErrorMessage = "The Branch Name is Required")]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Display(Name = "Target Framework")]
        public TargetFramework Framework { get; set; }
    }
}
