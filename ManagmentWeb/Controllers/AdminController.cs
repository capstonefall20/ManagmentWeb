using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagmentWeb.Enum;
using ManagmentWeb.Models;
using ManagmentWeb.Models.Entity;
using ManagmentWeb.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManagmentWeb.Controllers
{
   
    public class AdminController : SecureController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository _repository;
        public AdminController(IRepository repository, UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            ViewBag.Roles= await _repository.GetRolesAsync();
            return View(await _repository.GetUsersAsync(null));
        }
        [HttpGet]
        public async Task<IActionResult> UserInfoAjax(int? RoleId)
        {
            var result = await this._repository.GetUsersAsync(RoleId);
            return PartialView("/Views/Shared/PartialViews/Admin/_Users.cshtml", result);
        }
        [Authorize(Roles = "Admin,Project Creator")]
        public async Task<IActionResult> AddUsers()
        {
            UserModel userModel = new UserModel();
            userModel.Roles = await _repository.GetRolesAsync();
            return View(userModel);
        }
        [Authorize(Roles = "Admin,Project Creator")]
        [HttpPost]
        public async Task<IActionResult> AddUsers(UserModel userModel)
        {
            try
            {
                ApplicationUser applicationUser = new ApplicationUser();
                applicationUser.FirstName = userModel.FirstName;
                applicationUser.LastName = userModel.LastName;
                applicationUser.Mobile = userModel.Mobile;
                applicationUser.Email = userModel.Email;
                applicationUser.UserName = userModel.Email;
                var user = await _userManager.CreateAsync(applicationUser, userModel.Password);
                if (user.Succeeded)
                {
                    await _userManager.AddToRoleAsync(applicationUser, userModel.RoleName);
                    ModelState.Clear();
                    userModel = new UserModel();
                    ViewBag.Success = "User has been created";
                }
                userModel.Roles = await _repository.GetRolesAsync();
               
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
            }
            return View(userModel);
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel userModel)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(this.UserId.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var result =await _userManager.ChangePasswordAsync(user, userModel.OldPassword, userModel.ConfirmNewPassword);
           
            if (result.Succeeded)
            {
                ViewBag.Success = "Password has been changed";
            }
            else
            {
                ViewBag.Error = result.Errors.FirstOrDefault().Description;
            }
            return View(userModel);
        }
        [HttpGet]
        public async Task<IActionResult> LogTime()
        {
           
             return View(await this._repository.GetLogTimeAsync());
        }
        [HttpGet]
        public async Task<IActionResult> AddLogTime()
        {
            LogTimeModel logTimeModel = new LogTimeModel();
            var result = await this._repository.GetUsersAsync(null);
            logTimeModel.Users = result.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            return View(logTimeModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddLogTime(LogTimeModel logTimeModel)
        {
            logTimeModel.CreatedBy = this.UserId;
            if (await this._repository.AddLogTimeAsync(logTimeModel))
            {
                logTimeModel = new LogTimeModel();
                var result = await this._repository.GetUsersAsync(null);
                ModelState.Clear();
                logTimeModel.Users = result.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
                ViewBag.Success = "Time has been logged";
            }
            else
            {
                ViewBag.Error = "There are error while saving";
            }
            return View(logTimeModel);
        }
    }
}