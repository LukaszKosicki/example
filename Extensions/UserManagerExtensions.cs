using API.Models.Entity.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<User> GetByUserNameOrEmail(this UserManager<User> userManager, string userNameEmail)
        {
            var user = await userManager.FindByNameAsync(userNameEmail);

            if (user == null)
            {
                user = await userManager.FindByEmailAsync(userNameEmail);
            }

            return user;
        }
    }
}
