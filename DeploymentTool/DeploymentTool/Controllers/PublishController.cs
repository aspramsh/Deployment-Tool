using System;
using Microsoft.AspNetCore.Mvc;
using DeploymentTool.Models;
using Publisher;
using Microsoft.Extensions.Options;
using Data.Repositories;
using System.Collections.Generic;
using AutoMapper;
using System.Threading.Tasks;
using System.Linq;
using System.Management.Automation;
using DeploymentTool.Models.EmailEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DeploymentTool.Controllers
{
    [Authorize(Roles = "editor")]
    public class PublishController : Controller
    {
        private Projects Script { get; set; }

        private UserAccounts Account { get; set; }

        private readonly ISpecificationRepository _specificationRepository;

        private readonly IMapper _mapper;

        private readonly AppSettings _config;

        private readonly FoldersNotToDelete _foldersNotToDelete;

        private readonly SmtpSettingsModel _mailSettingsModel;

        public PublishController(IOptions<AppSettings> appSettings,  
            ISpecificationRepository specificationRepository, ITypeRepository typeRepository, 
            IMapper mapper)
        {
            _specificationRepository = specificationRepository;
            _config = appSettings.Value;
            Script = _config.Projects;
            Account = _config.UserAccounts;
            _mapper = mapper;
            _foldersNotToDelete = _config.FoldersNotToDelete;
            _mailSettingsModel = _config.SmtpSettingsModel;
            _mailSettingsModel.NetworkCredentials = _config.NetworkCredentialsModel;
        }


        public async Task<IActionResult> Index(string searchString, int target)
        {
            ViewData["CurrentFilter"] = searchString;

            ViewData["Framework"] = target;

            var projects = new MultiSelectProjects();

            var specifications = searchString != null ?
                (await _specificationRepository.FindByAsync(s => s.ProjectName.Contains(searchString))).ToList()
                : (await _specificationRepository.GetAllAsync()).ToList();

            specifications = (target != 0) ?
                specifications.Where(s => s.TypeId == target).ToList() :
                specifications.ToList();

            projects.Projects = new List<SelectListItem>();
            projects.ProjectIds = new int[specifications.Count];
            
            for (int i = 0; i < specifications.Count; ++i)
            {
                var item = new SelectListItem
                {
                    Text = specifications[i].ProjectName,
                    Value = specifications[i].Id.ToString()
                };
                projects.Projects.Add(item);
                projects.ProjectIds[i] = specifications[i].Id;
            }
            var specificationsToReturn = new List<DeploymentSpecificationModel>();
            foreach (var specification in specifications)
            {
                var specificationEntity = _mapper.Map<DeploymentSpecificationModel>(specification);
                specificationsToReturn.Add(specificationEntity);
            }

            projects.Specifications = specificationsToReturn;
            return View(projects);
        }

   
        public async Task<IActionResult> PublishProject(int[] projectIds)
        {
            string messageToReturn = null;
            var projectNames = new List<string>();

            foreach (int id in projectIds)
            {
                var specification = await _specificationRepository
                    .GetSingleAsync(s => s.Id == id);
                var specificationModel = _mapper
                    .Map<DeploymentSpecificationModel>(specification);
                bool isUpdated = GetOrUpdatePrj(specificationModel);
                if (!isUpdated)
                {
                    messageToReturn += $"{specificationModel.WebsiteName} has failed on update from bitbucket.";
                    continue;
                }
                string slnPath = $"{specificationModel.ProjectPath}\\{specificationModel.WebsiteName}";
                PathsModel paths = new PathsModel
                {
                    Solution = slnPath,
                    Publish = Script.InitialPublishPath,
                    Target = specificationModel.Framework,
                    IISAppPoolName = specificationModel.AppPoolName
                };
                switch (paths.Target)
                {
                    case TargetFramework.DotNetCore:
                        {
                            Paths pathsForPublisher = new Paths(paths.Solution, paths.Publish, paths.IISAppPoolName);
                            string path = Script.PowershellScriptPath;

                            try
                            {
                                ProjectPublisher.PublishToAFolder(pathsForPublisher, path, specificationModel.DeploymentPath, _foldersNotToDelete, specificationModel.WebsiteName);
                                messageToReturn += $"{specificationModel.WebsiteName} has successfully been published {Environment.NewLine}";
                                projectNames.Add(specificationModel.WebsiteName);
                                break;
                            }

                            catch (Exception e)
                            {
                                messageToReturn += $"{specificationModel.WebsiteName} has failed with following error message {e} {Environment.NewLine}";
                                break;
                            }
                        }

                    case TargetFramework.DotNetStandard:
                        {
                            messageToReturn += $"{specificationModel.WebsiteName} Still Not Specified. {Environment.NewLine}";
                            break;
                        }
                }
            }

            if (projectNames.Count > 0)
            {
                SendEmail(_mailSettingsModel.To, projectNames);
            }
            
            return Json(messageToReturn);
        }

        [HttpPost]
        public IActionResult RunSqlScriptAsync()
        {
            try
            {
                ProjectPublisher.RunSqlScript(_config.Projects.ConnectionString, Script.SqlScriptPath);
                return Json("Sql script execution finished successfuly.");
            }
            catch (Exception e)
            {
                
                return Json(e.InnerException is null ? e.Message : e.InnerException.Message);
            }
        }
        private bool GetOrUpdatePrj(DeploymentSpecificationModel specificationModel)
        {
            string slnPath = specificationModel.ProjectPath;
            Params p = new Params(slnPath, specificationModel.BranchName, specificationModel.ProjectName, specificationModel.WebsiteName);
            string path = Script.PowershellScriptPath;
            bool result = ProjectPublisher.GetFromRepository(p, path, Account);
            return result;
        }

        private void SendEmail(string email, List<string> projects)
        {
            string projectNames = null;

            foreach (var project in projects)
            {
                projectNames += $"{project}" + (projects.IndexOf(project) != projects.Count - 1 ? "," : ".");
            }
            var message = new MessageModel
            {
                ToAddresses = new List<string> { email },
                Subject = $"End of Deployment - {DateTime.Now.ToLongDateString()}",
                Body =
                    $"<html><body>Hi All, <br/><br/> The following projects have been successfully deployed: <br/> {projectNames}",
                IsBodyHtml = true
            };
            var client = new EmailSmtpClient(_mailSettingsModel);

            client.Send(message, new ProjectsController.BodyModel { To = _mailSettingsModel.To});
        }
    }
}