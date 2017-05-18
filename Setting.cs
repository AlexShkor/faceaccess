using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace faceaccess
{
  public class Setting : Controller
  {
    public const string AdminCredentialsEmail = "i.petrov@paralect.com";
    public const string AdminCredentialsPassword = "123qweASD";

    public const string EmailSenderEmailForAuth = "igor-red008_91@mail.ru";
    public const string EmailSenderName = "Administration Paralect Access";
    public const string EmailSenderPasswordFromMail = "12345678qwertyui";
    public const string EmailSenderHost = "smtp.mail.ru";

    public const string FaceClientSubscriptionKey = "ae10dbb146c749ce8810068d9b83a868";
    public const string FaceClientPersonGroupKey = "paralect";
    public const string FaceClientImgPrefix = "data:image/jpeg;base64,";

    public const string ISSUER = "MyAuthServer";    
    const string KEY = "mysupersecret_secretkey!123";
    public const string AUDIENCE = "http://localhost:5000/";
    public const int LIFETIME = 480; 

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
      return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
  }
}
