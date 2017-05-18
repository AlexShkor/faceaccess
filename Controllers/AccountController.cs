using System;
using System.Collections;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using faceaccess;
using faceaccess.Services;
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
    private IJwtGenerator _jwt;


    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IEmailSender emailSender,
        ILoggerFactory loggerFactory,
        MongoDatabase db,
        IFaceServiceClient faceClient,
        IJwtGenerator jwt)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _emailSender = emailSender;
      _logger = loggerFactory.CreateLogger<AccountController>();
      _db = db;
      _faceClient = faceClient;
      _jwt = jwt;
    }

    [HttpPost]
    [AllowAnonymous]
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
          var token = _jwt.GetToken(user.Id, role);
          return this.Json(Ok(token));
        }
        return this.Json(BadRequest("Invalid login or password."));
      }
      return this.Json(BadRequest("Invalid login or password"));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
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
    [Authorize]
    public async Task<IActionResult> LogOff()
    {
      await _signInManager.SignOutAsync();
      _logger.LogInformation(4, "User logged out.");
      return this.Json(Ok());
    }

    [HttpPost]
    [AllowAnonymous]
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
    [Authorize]
    public async Task<IActionResult> ChangePasswordFromUserProfile([FromBody]ChangePasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return this.Json(BadRequest("Invalid login or password"));
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
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return this.Json(BadRequest("Invalid login or password"));
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
    [Authorize]
    public IActionResult GetUser([FromBody] UserInfo user)
    {
      if (user.UserId != null)
      {
        var userProfile = _db.Persons.AsQueryable().First(x => x.Id == user.UserId);

        if (userProfile.Photo == "" | userProfile.Photo == null)
        {
          var imageBase64 = _db.AvatarDefault.AsQueryable().First().Photo;
          userProfile.Photo = Setting.FaceClientImgPrefix + imageBase64;
          return this.Json(userProfile);
        }
        var avatarUser = Setting.FaceClientImgPrefix + userProfile.Photo;
        userProfile.Photo = avatarUser;
        userProfile.IsUploadPhoto = true;
        return this.Json(userProfile);
      }
      return this.Json(BadRequest());
    }

    [HttpPost]
    [Authorize]
    public IActionResult GetAvatarDefault()
    {
      var imageBase64 = _db.AvatarDefault.AsQueryable().First().Photo;
      if (imageBase64 != "")
      {
        var avatarDefault = Setting.FaceClientImgPrefix + imageBase64;
        return this.Json(avatarDefault);
      }
      return this.Json(BadRequest());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] PersonDocument user)
    {
      if (user.Photo != null)
      {
        var photo = user.Photo.Substring(Setting.FaceClientImgPrefix.Length);
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
      var data = _faceClient.CreatePersonAsync(Setting.FaceClientPersonGroupKey, name);
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