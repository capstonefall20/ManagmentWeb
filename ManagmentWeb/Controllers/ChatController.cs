using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ManagmentWeb.Controllers
{
    public class ChatController : SecureController
    {
        public IActionResult Index()
        {
            ViewBag.UserId = this.UserId;
            return View();
        }
    }
}