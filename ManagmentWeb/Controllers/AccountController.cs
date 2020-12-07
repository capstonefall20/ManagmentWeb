using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using ManagmentWeb.Enum;
using ManagmentWeb.Models;
using ManagmentWeb.Models.Entity;
using ManagmentWeb.Repository.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManagmentWeb.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IRepository _repository;
		public AccountController(IRepository repository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this._userManager = userManager;
			this._signInManager = signInManager;
			_repository = repository;
		}
		public IActionResult Login(string ReturnUrl)
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				return Redirect("/admin/addusers");
				//await signInManager.SignOutAsync();
			}
			LoginModel loginModel = new LoginModel();
			return View(loginModel);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginModel loginModel)
		{
			if (ModelState.IsValid)
			{
				return await Authenticate(loginModel);
			}
			return View(loginModel);
		}
		private async Task<IActionResult> Authenticate(LoginModel loginModel)
		{

			ApplicationUser user = null;
			try
			{
				user = await _userManager.FindByNameAsync(loginModel.Email);
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Email or Password is incorrect";
				return View("Login", loginModel);
			}

			if (user == null)
			{
				ViewBag.Error = "Email or Password is incorrect";
				return View("Login", loginModel);
			}
			if (!String.IsNullOrEmpty(loginModel.Password) && !await _userManager.CheckPasswordAsync(user, loginModel.Password))
			{
				ViewBag.Error = "Email or Password is incorrect";
				return View("Login", loginModel);
			}
			var roles = await _userManager.GetRolesAsync(user);
			var claims = new List<Claim> {
				new Claim(ClaimTypes.Name, user.Email),
				new Claim(CustomClaims.UserId, user.Id.ToString())
			};
			if (roles != null)
			{
				foreach (var item in roles)
				{
					claims.Add(new Claim(ClaimTypes.Role, item.ToString()));
				}
			}
			var claimsIdentity = new ClaimsIdentity(
			claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authProperties = new AuthenticationProperties();

			await HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
			return Redirect("/project/projects");
		}
		public async Task<IActionResult> Logoff(string ReturnUrl)
		{
			if (HttpContext.Request.Cookies.Count > 0)
			{
				var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));
				foreach (var cookie in siteCookies)
				{
					Response.Cookies.Delete(cookie.Key);
				}
			}

			await HttpContext.SignOutAsync(
			CookieAuthenticationDefaults.AuthenticationScheme);
			HttpContext.Session.Clear();
			return Redirect("/account/login");
		}
		public IActionResult SignUp()
		{
			UserModel userModel = new UserModel();
			return View(userModel);
		}
		[HttpPost]
		public async Task<IActionResult> SignUp(UserModel userModel)
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
					await _userManager.AddToRoleAsync(applicationUser, "Worker");
					userModel = new UserModel();
					ViewBag.Success = "Sign up has completed successfully.";
				}
				else
				{
					ViewBag.Error = user.Errors.FirstOrDefault().Description;
				}

			}
			catch (Exception ex)
			{

				ViewBag.Error = ex.Message;
			}
			return View(userModel);
		}
		public async Task<IActionResult> AccessDenied()
		{
			return View();
		}
		public async Task<IActionResult> ForgotPassword()
		{
			LoginModel loginModel = new LoginModel();
			return View(loginModel);
		}
		public string Reverse(string text)
		{
			if (text == null) return null;

			// this was posted by petebob as well 
			char[] array = text.ToCharArray();
			Array.Reverse(array);
			return new String(array);
		}
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(LoginModel loginModel)
		{
			ApplicationUser user = null;
			try
			{
				user = await _userManager.FindByNameAsync(loginModel.Email);

			}
			catch (Exception ex)
			{

				// return BadRequest(new LoginResult { Error = new ClientDeleted(), Message = ex.Message, Succeeded = false });
			}
			if (user != null)
			{
				string newpassword = Reverse(DateTime.Now.Ticks.ToString()).Substring(0, 6);
				await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), newpassword);

				MailMessage mail = new MailMessage();
				mail.To.Add(user.Email);
				//mail.To.Add("Another Email ID where you wanna send same email");
				mail.From = new MailAddress("denishlocal@gmail.com");
				mail.Subject = "Email using Gmail";

				string Body = "Hi, your new password is " + newpassword + "";
				mail.Body = Body;

				mail.IsBodyHtml = true;
				SmtpClient smtp = new SmtpClient();
				smtp.UseDefaultCredentials = false;
				smtp.Port = 587;
				smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
				smtp.Credentials = new System.Net.NetworkCredential("denishlocal@gmail.com", "denishlocal@123");
				//Or your Smtp Email ID and Password
				smtp.EnableSsl = true;
				smtp.Send(mail);
				ViewBag.Success = "Email has been sent to you email address with new password";
			}
			else
			{
				ViewBag.Error = "Email does not exist";
			}

			return View(loginModel);
		}

	}
}