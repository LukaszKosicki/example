using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities.Auth;
using api.Models.ViewModels.Auth;
using api.Models.ViewModels.Users;
using api.Services;
using api.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly JwtSettings jwtSettings;
        public AuthController(UserManager<User> userMgr, IMapper map, IOptionsSnapshot<JwtSettings> jwtSts)
        {
            userManager = userMgr;
            mapper = map;
            jwtSettings = jwtSts.Value;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegistrationViewModel model)
        {
            var user = mapper.Map<RegistrationViewModel, User>(model);
            var userCreateResult = await userManager.CreateAsync(user, model.Password);
            await userManager.AddToRoleAsync(user, "User");
            
            if (userCreateResult.Succeeded)
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                SparkPostService.SendEmail(user, token, "registration");
                return Ok();
            }
            return Problem(userCreateResult.Errors.First().Code, null, 500);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            var user = userManager.Users.Include(u => u.Houses)
                .SingleOrDefault(u => u.Email == model.Email);
            if (user is null)
            {
                return NotFound();
            }
            var userSignInResult = await userManager.CheckPasswordAsync(user, model.Password);

            if (userSignInResult)
            {
                var roles = await userManager.GetRolesAsync(user);
                return Ok(mapper.Map<User, UserViewModel>(JwtService.GenerateJwt(user, roles, jwtSettings)));
            }

            return BadRequest();
        }

        [Authorize(Roles = "User")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel model)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            if (userManager.CheckPasswordAsync(user, model.Password).Result)
            {
                await userManager.DeleteAsync(user);
                return Ok();
            }

            return Problem(detail: "PasswordMismatch", statusCode: 500);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmationLinkSend(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                SparkPostService.SendEmail(user, token, "registration");
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (!result.Succeeded)
                {
                    return Problem(statusCode: 500, detail: result.Errors.First().Code);
                }

                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                SparkPostService.SendEmail(user, token, "forgot-password");
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailAddress([FromBody] ConfirmEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Problem(statusCode: 500, detail: "ModelIsInvalid");
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound();
            }
            var result = await userManager.ConfirmEmailAsync(user, model.Token);

            if (!result.Succeeded)
            {
                return Problem(statusCode: 500, detail: result.Errors.First().Code);
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult IsLoggedIn()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCurrentUser()
        {
            var userName = HttpContext.User.Identity.Name;
            var user = userManager.Users.Include(u => u.Houses)
                .SingleOrDefault(u => u.UserName == userName);

            if (user != null)
            {
                return Ok(mapper.Map<User, UserViewModel>(user));
            }

            return NotFound();
        }

    }
}