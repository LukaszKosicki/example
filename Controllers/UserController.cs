using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities.Auth;
using api.Models.ViewModels.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public UserController(UserManager<User> userMgr, IMapper map)
        {
            userManager = userMgr;
            mapper = map;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            return Problem(detail: result.Errors.First().Code, statusCode: 500);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            var user = await userManager.Users
                .Include(u => u.Houses)
                .Include(u => u.Rooms)
                .SingleOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<User, GetUserViewModel>(user));
        }
    }
}