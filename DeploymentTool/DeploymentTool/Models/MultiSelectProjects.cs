using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DeploymentTool.Models
{
    /// <summary>
    /// A class for creating multiselect dropdown list
    /// </summary>
    public class MultiSelectProjects
    {
        /// <summary>
        /// An array for keeping project IDs
        /// </summary>
        public int[] ProjectIds { get; set; }

        /// <summary>
        /// A list for keeping project names and IDs
        /// </summary>
        public List<SelectListItem> Projects { set; get; }

        public IEnumerable<DeploymentSpecificationModel> Specifications { get; set; }
    }
}
