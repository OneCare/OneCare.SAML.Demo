
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

using ComponentSpace.SAML2;

using OneCare.Partner.SP.Utils;
using OneCare.Partner.SP.Models;


namespace OneCare.Partner.SP.Controllers
{
	public class AccountController : Controller
	{
		public const string UserCookieName = "OneCare.Partner.SP.User";

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
						//var identity = user.NewUser();
						//Response.SaveUserIdentity(identity);
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
			var idp = WebConfigurationManager.AppSettings["PartnerIdP"];
			SAMLServiceProvider.InitiateSSO(Response, null, idp);
			return new EmptyResult();
		}

		//[HttpPost]
		//[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		//public ActionResult Login(LoginModel model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		var pwd = Database.Encrypt(model.Password);
		//		var user = Database.Find<UserDoc>(u =>
		//			(u.Username.ToLower() == model.Username.ToLower())
		//			&& (u.Password == pwd));
		//		if (user != null)
		//		{
		//			var identity = user.NewUser();
		//			Response.SaveUserIdentity(identity);
		//			return RedirectToAction("Index", "Home");
		//		}
		//		else
		//			ModelState.AddModelError("", "Invalid user name or password.");
		//	}

		//	// If we got this far, something failed, redisplay form
		//	return View(model);
		//}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			try
			{
				var idp = WebConfigurationManager.AppSettings["PartnerIdP"];
				SAMLServiceProvider.InitiateSLO(Response, null, idp);
			}
			catch
			{
				Response.ForgetUserIdentity();
				return RedirectToAction("Index", "Home");
			}
			return new EmptyResult();
		}
	}
}
