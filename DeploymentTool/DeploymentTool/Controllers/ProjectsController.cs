using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DeploymentTool.Models;
using AutoMapper;
using Data.Models;
using Data.Repositories;
using DeploymentTool.Data;
using DeploymentTool.Models.EmailEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DeploymentTool.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ISpecificationRepository _specificationRepository;

        private readonly ITypeRepository _typeRepository;

        private readonly IMapper _mapper;

        private readonly ApplicationDbContext _userContext;

        private readonly SmtpSettingsModel _maiSettingsModel;

        private readonly AppSettings _config;

        private readonly IHostingEnvironment _environment;

        public ProjectsController(ISpecificationRepository specificationRepository,
            ITypeRepository typeRepository, IMapper mapper, ApplicationDbContext userContext, 
            IOptions<AppSettings> appSettings, 
            IHostingEnvironment environment)
        {
            _config = appSettings.Value;
            _specificationRepository = specificationRepository;
            _typeRepository = typeRepository;
            _mapper = mapper;
            _userContext = userContext;
            _maiSettingsModel = _config.SmtpSettingsModel;
            _maiSettingsModel.NetworkCredentials = _config.NetworkCredentialsModel;
            _environment = environment;
        }
        // GET: Projects
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(string searchString, int target)
        {
            ViewData["CurrentFilter"] = searchString;

            ViewData["Framework"] = target;

            var specifications = (searchString != null) ?
                (await _specificationRepository.FindByAsync(s => s.ProjectName.Contains(searchString)))
                : (await _specificationRepository.GetAllAsync());

            specifications = (target != 0) ?
                specifications.Where(s => s.TypeId == target) :
                specifications;

            var specificationsToReturn = new List<DeploymentSpecificationModel>();
            foreach (var specification in specifications)
            {
                var specificationEntity = _mapper.Map<DeploymentSpecificationModel>(specification);
                specificationsToReturn.Add(specificationEntity);
            }
            return View(specificationsToReturn);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int id)
        {
            var specification = await _specificationRepository.GetSingleAsync(s => s.Id == id);
            var specificationToReturn = _mapper.Map<DeploymentSpecificationModel>(specification);
            return View(specificationToReturn);
        }

        [Authorize(Roles = "admin")]
        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeploymentSpecificationModel specification)
        {
            try
            {
                var specificationEntity = _mapper.Map<DeploymentSpecification>(specification);
                _specificationRepository.Add(specificationEntity);
                var type = await _typeRepository.GetSingleAsync(t => t.Id == specificationEntity.TypeId);
                if (type == null)
                {
                    _typeRepository.Add(specificationEntity.Type);
                }
                var isSaveSuccessfully = await _specificationRepository.SaveAsync();
                if (!isSaveSuccessfully)
                {
                    return Content("Creating a Specification Failed on Save.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var specification = await _specificationRepository.GetSingleAsync(s => s.Id == id);
            var specificationToEdit = _mapper.Map<DeploymentSpecificationModel>(specification);
            return View(specificationToEdit);
        }

        // POST: Projects/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeploymentSpecificationModel specification)
        {
            try
            {
                var specificationToUpdate = _mapper.Map<DeploymentSpecification>(specification);
                _specificationRepository.Update(specificationToUpdate);

                var isSaveSuccessful = await _specificationRepository.SaveAsync();
                if (!isSaveSuccessful)
                {
                    return Content("Update Failed on Save.");
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var specification = await _specificationRepository.GetSingleAsync(s => s.Id == id);
            var specificationToDelete = _mapper.Map<DeploymentSpecificationModel>(specification);
            return View(specificationToDelete);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DeploymentSpecificationModel specificationModel)
        {
            try
            {
                var specification = _mapper.Map<DeploymentSpecification>(specificationModel);
                _specificationRepository.Delete(specification);
                var isSaveSuccessful = await _specificationRepository.SaveAsync();

                if (!isSaveSuccessful)
                {
                    return Content("Delete Failed on Save.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult GetUsersWithNoRoles()
        {
            var users = _userContext
                .Users
                .Where(u => _userContext
                .UserRoles.FirstOrDefault(r => u.Id == r.UserId) == null).ToList();
            return View(users);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(ApplicationUser user)
        {
            user.NormalizedUserName = user.UserName.ToLowerInvariant();
            user.NormalizedEmail = user.Email.ToLowerInvariant();
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.RegisterId = Guid.NewGuid().ToString();
            user.RegisterDate = DateTime.Now;
            await _userContext.Users.AddAsync(user);
            if  (_userContext.SaveChanges() == 0)
            {
                return Content("Creating the user failed on save.");
            }
            SendEmail(user);
            return RedirectToAction(nameof(GetUsersWithNoRoles));
        }

        [HttpGet]
        public IActionResult CreatePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword(string id, PasswordCreationModel model)
        {
            var user = await _userContext.Users.SingleAsync(u => u.RegisterId == Base64UrlEncoder.Decode(id));
            if ((DateTime.Now - user.RegisterDate).TotalHours > 24)
            {
                return Content("Your Registration time has expired.");
            }
            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(user, model.Password);
            user.PasswordHash = hashed;
            _userContext.Users.Update(user);

            if (await _userContext.SaveChangesAsync() == 0)
            {
                return Content("Updating data failed on save.");
            }

            var role = await _userContext.Roles.SingleAsync(r => r.Name == "editor");
            var userRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            _userContext.UserRoles.Add(userRole);

            if (await _userContext.SaveChangesAsync() == 0)
            {
                return Content("Adding role failed on save.");
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        private void SendEmail(ApplicationUser user)
        {
            var registerIdEncoded = Base64UrlEncoder.Encode(user.RegisterId);
            var message = new MessageModel
            {
                ToAddresses = new List<string> { user.Email },
                Subject = $"Password Request - {DateTime.Now.ToLongDateString()}",
                Body =
                    $"<html><body>Hi, <br/><br/> Please follow the link to set your Password: <br/> " +
                    $"{_config.GetPasswordLink(_environment)}/{registerIdEncoded} <br/> The link will expire in 24 hours.</body><html>",
                IsBodyHtml = true
            };
            var client = new EmailSmtpClient(_maiSettingsModel);

            client.Send(message, new BodyModel { To = "aspramshadyan@gmail.com" });
        }
        public class BodyModel
        {
            public string To { get; set; }
        }
    }
}