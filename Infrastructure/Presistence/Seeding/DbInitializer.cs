using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Presistence.Data;

namespace Presistence.Seeding
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly UserManager<Users> _userManager;

        public DbInitializer(ApplicationDbContext dbContext,
            RoleManager<IdentityRole<int>> roleManager,
            UserManager<Users> userManager) 
        {
            _dbcontext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task InitializeIdentityAsync()
        {
            // seed default user & role 
            
            // seed roles 
            if (!_roleManager.Roles.Any()) // not contain any role
            {
                // Admin and user
               await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
               await _roleManager.CreateAsync(new IdentityRole<int>("User"));
            }
            // 2. seed users , assign role for each user 
            if (!_userManager.Users.Any())
            {
                var adminUser = new Users
                {
                    UserName = "admin",
                    Email = "Admin@gmail.com",
                    PhoneNumber = "1234567890",
                    UserType = "Admin",
                    InterestsJson = "[]",          
                    TravelGroup = "Solo"
                };
                var User = new Users
                {
                    UserName = "Enas",
                    Email = "Enas@gmail.com",
                    PhoneNumber = "1234567890",
                    UserType = "User",
                     InterestsJson = "[]",        
                    TravelGroup = "Solo"
                };
                await _userManager.CreateAsync(adminUser,"Admin#01");
                await _userManager.CreateAsync(User, "Enas@li1");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                await _userManager.AddToRoleAsync(User, "User");

            }

        }
    }
}
