using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace faceaccess.Services
{
  public class JwtService : IJwtGenerator
  {
    public string GetToken(string userId, string role)
    {
        var identity = GetIdentity(userId, role);
      var now = DateTime.UtcNow;
      var jwt = new JwtSecurityToken(
              issuer: Setting.ISSUER,
              audience: Setting.AUDIENCE,
              notBefore: now,
              claims: identity.Claims,
              expires: now.Add(TimeSpan.FromMinutes(Setting.LIFETIME)),
              signingCredentials: new SigningCredentials(Setting.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
      var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
      return encodedJwt;
    }

    private ClaimsIdentity GetIdentity(string userId, string role)
    {
        var claims = new List<Claim>
                {
                    new Claim("id", userId),
                    new Claim("role", role)
                };
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", "id",
                                                                              "role");
        return claimsIdentity;

    }
  }
}
