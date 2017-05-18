using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace faceaccess.Services
{
   public interface IJwtGenerator
   {
       string GetToken(string userId, string role);
   }
}
