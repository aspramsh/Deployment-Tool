using System.Reflection;

namespace DeploymentTool.Models.EmailEntities
{
    /// <summary>
    /// Email template rendering class
    /// </summary>
    public class ReplaceRendererHelper
    {
        public const string EmailKey = "##";

        public static string Parse<T>(string template, T model, bool isHtml = true)
        {
            foreach (var pi in model.GetType().GetRuntimeProperties())
            {
                template = template.Replace($"{EmailKey} {pi.Name} {EmailKey}", pi.GetValue(model, null).ToString());
            }
            return template;
        }
    }
}