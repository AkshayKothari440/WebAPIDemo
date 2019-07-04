using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPIDemo.Model;

namespace WebAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;


        private readonly string LoginSuccess = "Login Successfull...";
        private readonly string Login2FA = "User login required two step authentication.";
        private readonly string LoginLocked = "Your account is locked few time.";
        private readonly string LoginFailed = "Login Failed";

        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }
        [HttpPost]
        [Route("Register")]
        //POST : /api/ApplicationUser/Register
        public async Task<Object> PostApplicationUser(ApplicationUserModel model)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = model.UserName,
                FullName = model.FullName,
                Email = model.Email
            };
            try
            {
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<Object> Login(LoginModel model)
        //{
        //    //ViewData["ReturnUrl"] = returnUrl;
        //    if (ModelState.IsValid)
        //    {
        //        // This doesn't count login failures towards account lockout
        //        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,false,false);


        [HttpPost]
        [Route("Login")]
        //POST : /api/ApplicationUser/Login
        public async Task<Object> Login(LoginModel model)
        {

            try
            {
               
                bool isEmail = Regex.IsMatch(model.UserName, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                if (isEmail)
                {
                    var user = await _userManager.FindByEmailAsync(model.UserName);
                    if(user!=null)
                    {
                        var Status = await AttemptLogin(user.UserName, model.Password);
                        return Ok(Status);
                    }
                    else
                    {
                        return "Email Not Found";
                    }

                }
                else
                {
                    var Status = await AttemptLogin(model.UserName,model.Password);
                    return Ok(Status);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private async Task<string> AttemptLogin(string UserName,string Password)
        {
            var result = await _signInManager.PasswordSignInAsync(UserName, Password, false, false);

            if (result.Succeeded)
            {
                return LoginSuccess;

            }
            else if (result.RequiresTwoFactor)
            {
                return Login2FA;
            }
            else if (result.IsLockedOut)
            {
                return LoginLocked;
            }
            else
            {
                return LoginFailed;
            }
        }

    }
}