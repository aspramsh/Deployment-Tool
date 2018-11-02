using System;
using Microsoft.AspNetCore.Identity;

namespace DeploymentTool.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
       public string RegisterId { get; set; }

       public DateTime RegisterDate { get; set; }
    }
}
