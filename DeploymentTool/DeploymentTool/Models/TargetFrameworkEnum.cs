using System.ComponentModel.DataAnnotations;

namespace DeploymentTool.Models
{
    /// <summary>
    /// An enumeration for choosing target platform
    /// </summary>
    public enum TargetFramework
    {
        [Display(Name = "ASP.NET Standard")] DotNetStandard = 1,
        [Display(Name = "ASP.NET Core")] DotNetCore
    };
}