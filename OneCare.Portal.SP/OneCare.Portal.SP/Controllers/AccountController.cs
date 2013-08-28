
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using OneCare.Portal.SP.Utils;
using OneCare.Portal.SP.Models;


namespace OneCare.Portal.SP.Controllers
{
	public class AccountController : Controller
	{
		public const string UserCookieName = "OneCare.Portal.SP.User";

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				var user = Database.Find<UserDoc>(u => u.Username.ToLower() == model.Username.ToLower());
				if (user == null)
				{
					try
					{
						user = Database.Insert(model.NewUser());
						var identity = user.NewUser();
						Response.SaveUserIdentity(identity);
						return RedirectToAction("Index", "Home");
					}
					catch
					{
						ModelState.AddModelError("", "Couldn't add user to permanent store.");
					}
				}
				else
					ModelState.AddModelError("", "User name is already taken.");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginModel model)
		{
			if (ModelState.IsValid)
			{
				var pwd = Database.Encrypt(model.Password);
				var user = Database.Find<UserDoc>(u =>
					(u.Username.ToLower() == model.Username.ToLower())
					&& (u.Password == pwd));
				if (user != null)
				{
					var identity = user.NewUser();
					Response.SaveUserIdentity(identity);
					return RedirectToAction("Index", "Home");
				}
				else
					ModelState.AddModelError("", "Invalid user name or password.");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff(string returnUrl)
		{
			Response.ForgetUserIdentity();
			return RedirectToLocal(returnUrl);
		}

		ActionResult RedirectToLocal(string returnUrl)
		{
			return Url.IsLocalUrl(returnUrl) ?
				Redirect(returnUrl) as ActionResult
				: RedirectToAction("Index", "Home") as ActionResult;
		}
	}
}
