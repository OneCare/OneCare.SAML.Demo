
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using OneCare.Partner.IdP.Utils;
using OneCare.Partner.IdP.Models;


namespace OneCare.Partner.IdP.Controllers
{
	public class AccountController : Controller
	{
		public const string UserCookieName = "OneCare.Partner.IdP.User";


		//
		// GET: /Account/

		public ActionResult Index()
		{
			return View();
		}

		//
		// GET: /Account/Register

		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register

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

		//
		// GET: /Account/Register

		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Register

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginModel model, string returnUrl)
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
					return RedirectToLocal(returnUrl);
				}
				else
					ModelState.AddModelError("", "Invalid user name or password.");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}


		//
		// POST: /Account/LogOff

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			Response.ForgetUserIdentity();
			return RedirectToAction("Index", "Home");
		}

		ActionResult RedirectToLocal(string returnUrl)
		{
			return Url.IsLocalUrl(returnUrl) ?
				Redirect(returnUrl) as ActionResult
				: RedirectToAction("Index", "Home") as ActionResult;
		}
	}
}
