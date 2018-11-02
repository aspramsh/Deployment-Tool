using System.ComponentModel.DataAnnotations;

namespace DeploymentTool.Models
{
    public class PathsModel
    {
        /// <summary>
        /// The path of project to be published
        /// </summary>
        [Required(ErrorMessage = "The Path is Required")]
        [Display(Name = "Please Specify The Solution Folder Path: ")]
        [RegularExpression(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$", ErrorMessage = "Path is Not Valid.")]
        public string Solution { get; set; }

        /// <summary>
        /// The destination path of published project
        /// </summary>
        [Required(ErrorMessage = "The Path is Required")]
        [Display(Name = "Please Specify The Publish Folder Path: ")]
        [RegularExpression(@"^(?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$", ErrorMessage = "Path is Not Valid.")]
        public string Publish { get; set; }

        /// <summary>
        /// IIS pool where the project is deployed
        /// </summary>

        [Display(Name = "Please Specify IIS Application Pool Name")]
        public string IISAppPoolName { get; set; }

        /// <summary>
        /// The target framework of the project
        /// </summary>
        [Display(Name = "Please Select Target Platform")]
        public TargetFramework Target { get; set; }
    }
}
