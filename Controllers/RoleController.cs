using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;

        public RoleController(RoleManager<Role> roleMgr, UserManager<User> userMgr)
        {
            roleManager = roleMgr;
            userManager = userMgr;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if(string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Nazwa roli jest niepoprawna");
            }

            var newRole = new Role
            {
                Name = roleName
            };

            var roleResult = await roleManager.CreateAsync(newRole);

            if (roleResult.Succeeded)
            {
                return Ok();
            }

            return Problem(roleResult.Errors.First().Code, null, 500);
        }

        [HttpPost("user/{userEmail}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToTole(string userEmail, [FromBody] string roleName)
        {
            var user = userManager.Users.SingleOrDefault(u => u.Email == userEmail);

            if (user is null)
            {
                return NotFound("Nie znaleziono wybranego użytkownika");
            }

            var result = await userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Code, null, 500);
        }
    }
}