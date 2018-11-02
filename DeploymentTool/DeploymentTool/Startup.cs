using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DeploymentTool.Data;
using DeploymentTool.Models;
using DeploymentTool.Services;
using Publisher;
using Data;
using Data.Repositories;
using Data.Models;
using DeploymentTool.Models.EmailEntities;
using Microsoft.Extensions.Logging;

namespace DeploymentTool
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionStrings:PublisherDBConnection"];
            services.AddDbContext<ProjectPublisherContext>(o => o.UseSqlServer(connectionString));

            services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 6;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 10;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
    });

            var appSettings = new AppSettings
            {
                Link = Configuration.GetSection("PasswordLink").Get<CurrentEnvironment>(),
                FoldersNotToDelete = Configuration.GetSection("FoldersNotToDelete").Get<FoldersNotToDelete>(),
                NetworkCredentialsModel = Configuration.GetSection("NetworkCredentialsModel").Get<NetworkCredentialsModel>(),
                Projects = Configuration.GetSection("Projects").Get<Projects>(),
                SmtpSettingsModel = Configuration.GetSection("SmtpSettingsModel").Get<SmtpSettingsModel>(),
                UserAccounts = Configuration.GetSection("UserAccounts").Get<UserAccounts>()
            };
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            services.AddOptions();

            services.Configure<AppSettings>(Configuration);

            // register the repository
            services.AddScoped<ISpecificationRepository, SpecificationRepository>();

            services.AddScoped<ITypeRepository, TypeRepository>();

            services.AddSingleton(appSettings);

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DeploymentSpecificationModel, DeploymentSpecification>()
                .ForMember(dest => dest.TypeId, opt => opt.MapFrom(src => src.Framework));

                cfg.CreateMap<DeploymentSpecification, DeploymentSpecificationModel>()
                .ForMember(dest => dest.Framework, opt => opt.MapFrom(src => src.TypeId));
            });

            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);

            services.AddTransient<UsersDbContextSeedData>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ProjectPublisherContext
            context, UsersDbContextSeedData seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            seeder.SeedUsers();

            context.EnsureSeedDataForContext();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
