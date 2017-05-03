using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectOxford.Face;
using VueJsAspNetCoreSample.Documents;
using VueJsAspNetCoreSample.Models.AccountViewModels;
using VueJsAspNetCoreSample.Services;

namespace VueJsAspNetCoreSample.Controllers
{
    using Microsoft.AspNetCore.Identity.MongoDB;

    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private IFaceServiceClient _faceClient = new FaceServiceClient("ae10dbb146c749ce8810068d9b83a868");
        const string _personGroupKey = "paralect";
        private MongoDatabase _db;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory,
            MongoDatabase db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");

                    var user = await _userManager.FindByNameAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();
                    return this.Json(Ok(role));
                }                
                    return this.Json(BadRequest("Invalid login or password."));
            }
            return this.Json(BadRequest("Invalid login or password"));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await CreatePerson(user.Id, model.FullName);
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return this.Json(Ok());
                }
                return this.Json(BadRequest(result.Errors));
            }
            return this.Json(BadRequest("Invalid login or password"));
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return this.Json(Ok());          
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var schemeAndHost = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;
                var callbackUrl = $"{schemeAndHost}/#/ResetPassword/{code}";                              
                _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>{schemeAndHost}</a>");
                return this.Json(Ok());
            }
            return this.Json(BadRequest("Invalid email"));
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.Json(BadRequest("IsValid false"));
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return this.Json(BadRequest("User does not exist"));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return this.Json(Ok());
            }
            return this.Json(BadRequest(result.Errors));
        }

        private async Task CreatePerson(string id, string name)
        {
            PersonDocument doc = new PersonDocument();
            var data = _faceClient.CreatePersonAsync(_personGroupKey, name);
            doc.PersonId = data.Result.PersonId;
            doc.Created = DateTime.Now;
            doc.Id = id;
            doc.Name = name;
            await _db.Persons.InsertOneAsync(doc);
        }
    }
}