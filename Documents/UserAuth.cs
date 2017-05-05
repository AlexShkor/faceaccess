using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.MongoDB;

namespace VueJsAspNetCoreSample.Documents
{
    public class UserAuth : IdentityUser
    {
        public string Photo { set; get; }
    }
}
