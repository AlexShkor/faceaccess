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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectOxford.Face;
using MongoDB.Bson;
using MongoDB.Driver;
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
        private IFaceServiceClient _faceClient;
        private MongoDatabase _db;
        private IConfiguration _configuration;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory,
            MongoDatabase db,
            IConfiguration configuration,
            IFaceServiceClient faceClient)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _db = db;
            _configuration = configuration;
            _faceClient = faceClient;
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
                    return this.Json(Ok(new ArrayList() {role, user.Id}));
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
        public async Task<IActionResult> ChangePasswordFromUserProfile([FromBody]ChangePasswordViewModel model)
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
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);
            if (result.Succeeded)
            {
                return this.Json(Ok());
            }
            return this.Json(BadRequest(result.Errors));
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
   
        [HttpPost]
        public IActionResult GetUser([FromBody] UserInfo user)
        {
            if (user.UserId != null)
            {
                var userProfile = _db.Persons.AsQueryable().First(x => x.Id == user.UserId);

                if (userProfile.Photo == "" | userProfile.Photo == null)
                {
                    var imageBase64 = _db.AvatarDefault.AsQueryable().First().Photo;
                    userProfile.Photo = _configuration["FaceClient:ImgPrefix"] + imageBase64;
                    return this.Json(userProfile);
                }
                    var avatarUser = _configuration["FaceClient:ImgPrefix"] + userProfile.Photo;
                    userProfile.Photo = avatarUser;
                    userProfile.IsUploadPhoto = true;
                    return this.Json(userProfile);                        
            }
            return this.Json(BadRequest());
        }

        [HttpPost]
        public IActionResult GetAvatarDefault()
        {
            var imageBase64 = _db.AvatarDefault.AsQueryable().First().Photo;
            if (imageBase64 != "")
            {
                var avatarDefault = _configuration["FaceClient:ImgPrefix"] + imageBase64;
                return this.Json(avatarDefault);
            }
            return this.Json(BadRequest());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] PersonDocument user)
        {
            if (user.Photo != null)
            {
                var photo = user.Photo.Substring(_configuration["FaceClient:ImgPrefix"].Length);
                user.Photo = photo;
            }
            var filter = Builders<PersonDocument>.Filter.Eq(x => x.Id, user.Id);
            var result = await _db.Persons.ReplaceOneAsync(filter, user);
            if (result != null)
            {
              return this.Json(Ok());  
            }
            return this.Json(BadRequest());
        }

        private async Task CreatePerson(string id, string name)
        {
            PersonDocument doc = new PersonDocument();
            var data = _faceClient.CreatePersonAsync(_configuration["FaceClient:PersonGroupKey"], name);
            doc.PersonId = data.Result.PersonId;
            doc.Created = DateTime.Now;
            doc.Id = id;
            doc.Name = name;
            await _db.Persons.InsertOneAsync(doc);
        }

        public class UserInfo
        {
            public string UserId { get; set; }
        }
    }
}