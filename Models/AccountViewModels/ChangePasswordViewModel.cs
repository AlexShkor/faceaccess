using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueJsAspNetCoreSample.Models.AccountViewModels
{
    public class ChangePasswordViewModel
    {
        public string Email { set; get; }
        public string NewPassword { set; get; }
    }
}
