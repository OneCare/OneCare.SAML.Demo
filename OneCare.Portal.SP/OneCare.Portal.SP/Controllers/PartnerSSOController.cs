
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Utility;

using OneCare.Portal.SP.Utils;
using OneCare.Portal.SP.Models;


namespace OneCare.Portal.SP.Controllers
{
	public class RequireBasicAuthentication : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var req = filterContext.HttpContext.Request;
			if (String.IsNullOrEmpty(req.Headers["Authorization"]))
			{
				var res = filterContext.HttpContext.Response;
				res.StatusCode = 401;
				res.AddHeader("WWW-Authenticate", "Basic realm=\"Twitter\"");
				res.End();
			}
		}
	}


	public class PartnerSSOData
	{
		public static ExternalAppDoc PartnerApp
		{
			get
			{
				return Session.GetValue<ExternalAppDoc>("PartnerApp");
			}
			set
			{
				Session["PartnerApp"] = value;
			}
		}

		public static bool SSOReceived
		{
			get
			{
				return Session.GetValue("SSOReceived", false);
			}
			set
			{
				Session["SSOReceived"] = value;
			}
		}

		public static string PartnerUsername
		{
			get
			{
				return Session.GetValue("PartnerUsername");
			}
			set
			{
				Session["PartnerUsername"] = value;
			}
		}

		public static UserDoc AssociatedUser
		{
			get
			{
				return Session.GetValue<UserDoc>("AssociatedUser");
			}
			set
			{
				Session["AssociatedUser"] = value;
			}
		}

		public static void Clear()
		{
			PartnerApp = null;
			SSOReceived = false;
			PartnerUsername = null;
			AssociatedUser = null;
		}

		static HttpSessionState Session
		{
			get
			{
				return HttpContext.Current.Session;
			}
		}
	}


	public class PartnerSSOController : Controller
	{
		[RequireBasicAuthentication]
		//[RequireHttps]
		public ActionResult Login()
		{
			// Read application's creads from the header.
			var appName = default(string);
			var password = default(string);
			HttpBasicAuthentication.GetAuthorizationHeader(Request, out appName, out password);
			if (string.IsNullOrEmpty(appName) || string.IsNullOrEmpty(password))
				return new HttpUnauthorizedResult("Invalid username or password.");
			
			var app = Database.FindUnique<ExternalAppDoc>(a => 
				(a.Name == appName) && (a.Password == password));
			if (app == null)
				return new HttpUnauthorizedResult("Invalid username or password.");

			PartnerSSOData.PartnerApp = app;
			
			// Request the user info from the IdP.
			SAMLServiceProvider.InitiateSSO(Response, null, app.IdP);

			return new EmptyResult();
			//return RedirectToAction("Index", "Home");
		}

		public ActionResult Associate(string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
				Response.ForgetUserIdentity();
			ViewBag.PartnerApp = PartnerSSOData.PartnerApp.Name;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Associate(LoginModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var user = default(UserDoc);
				Database.Write(session =>
				{
					var password = Database.Encrypt(model.Password);
					user = Database.FindUnique<UserDoc>(u =>
						(u.Username.ToLower() == model.Username.ToLower())
						&& (u.Password == password));
					if (user != null)
					{
						var app = PartnerSSOData.PartnerApp;
						if (user.ExternalAccounts == null)
							user.ExternalAccounts = new List<ExternalAccountDoc>();
						user.ExternalAccounts.Add(
							new ExternalAccountDoc
							{
								AppId = PartnerSSOData.PartnerApp.Id,
								Username = PartnerSSOData.PartnerUsername
							});
					}
				});
				if (user != null)
				{
					PartnerSSOData.AssociatedUser = user;
					return this.RedirectToLocal(returnUrl);
				}
				ModelState.AddModelError("", "Invalid user name or password.");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}
	}
}
