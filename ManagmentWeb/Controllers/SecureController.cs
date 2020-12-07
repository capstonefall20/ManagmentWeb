 
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagmentWeb.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SecureController : Controller
    {
        public int UserId
        {
            get
            {
                return this.User.GetUserId();
            }
        }
        public string UserName
        {
            get
            {

                if (this.User != null && this.User.Claims.Where(c => c.Type == CustomClaims.UserName).Count() > 0)
                {
                    return this.User.Claims.Where(c => c.Type == CustomClaims.UserName).SingleOrDefault().Value;
                }
                return "";
            }
        }

    }
}