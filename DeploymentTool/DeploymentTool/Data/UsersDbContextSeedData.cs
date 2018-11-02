using DeploymentTool.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DeploymentTool.Data
{
    public class UsersDbContextSeedData
    {
        private readonly ApplicationDbContext _context;

        public UsersDbContextSeedData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async void SeedUsers()
        {
            var userAdmin = new ApplicationUser
            {
                UserName = "Administrator@email.com",
                NormalizedUserName = "administrator@email.com",
                Email = "Administrator@email.com",
                NormalizedEmail = "administrator@email.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            await AddRoles(userAdmin, "admin", "password1");

            var userEditor = new ApplicationUser
            {
                UserName = "Editor@email.com",
                NormalizedUserName = "editor@email.com",
                Email = "Editor@email.com",
                NormalizedEmail = "editor@email.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            await AddRoles(userEditor, "editor", "password2");
        }

        public async Task AddRoles(ApplicationUser user, string roleName, string userPassword)
        {
            var roleStore = new RoleStore<IdentityRole>(_context);

            if (!_context.Roles.Any(r => r.Name == roleName))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = roleName, NormalizedName = roleName });
            }

            if (!_context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, userPassword);
                user.PasswordHash = hashed;
                var userStore = new UserStore<ApplicationUser>(_context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, roleName);
            }
            await _context.SaveChangesAsync();
        }
    }
}