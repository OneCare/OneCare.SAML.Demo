
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using OneCare.Portal.SP.Controllers;
using OneCare.Portal.SP.Models;
using System.Web.Mvc;
using System.Security.Policy;
using System.Web.Routing;
using System.Web.SessionState;


namespace OneCare.Portal.SP.Utils
{
	public static class Extensions
	{
		public static string GetValue(this HttpSessionStateBase session, string key, string defaultValue=null)
		{
			var result = session[key];
			return (result == null) ? defaultValue : result as string;
		}

		public static int GetValue(this HttpSessionStateBase session, string key, int defaultValue)
		{
			var result = session[key];
			return (result == null) ? defaultValue
				: (result is string) ? int.Parse(result as string)
				: (int)result;
		}

		public static bool GetValue(this HttpSessionStateBase session, string key, bool defaultValue)
		{
			var result = session[key];
			return (result == null) ? defaultValue
				: (result is string) ? bool.Parse(result as string)
				: (bool)result;
		}

		public static T GetValue<T>(this HttpSessionStateBase session, string key, T defaultValue = null)
			where T : class
		{
			var result = session[key];
			return (result == null) ? defaultValue : result as T;
		}

		public static string GetValue(this HttpSessionState session, string key, string defaultValue = null)
		{
			var result = session[key];
			return (result == null) ? defaultValue : result as string;
		}

		public static int GetValue(this HttpSessionState session, string key, int defaultValue)
		{
			var result = session[key];
			return (result == null) ? defaultValue
				: (result is string) ? int.Parse(result as string)
				: (int)result;
		}

		public static bool GetValue(this HttpSessionState session, string key, bool defaultValue)
		{
			var result = session[key];
			return (result == null) ? defaultValue
				: (result is string) ? bool.Parse(result as string)
				: (bool)result;
		}

		public static T GetValue<T>(this HttpSessionState session, string key, T defaultValue = null)
			where T : class
		{
			var result = session[key];
			return (result == null) ? defaultValue : result as T;
		}

		public static void SaveUserIdentity(this HttpResponseBase response, UserModel user)
		{
			var userCookieName = AccountController.UserCookieName;

			var value = string.Format(
				"{0};{1};{2};{3};{4}",
				user.Id,
				user.Username,
				user.Email,
				user.FirstName,
				user.LastName);
			var ticket = new FormsAuthenticationTicket(
				1,
				userCookieName,
				DateTime.Now,
				DateTime.Now.AddDays(30),
				true,
				value,
				FormsAuthentication.FormsCookiePath);
			response.SetCookie(
				new HttpCookie(
					userCookieName,
					FormsAuthentication.Encrypt(ticket)));
		}

		public static void ForgetUserIdentity(this HttpResponseBase response)
		{
			response.SaveUserIdentity(
				new UserModel
				{
					Id = string.Empty,
					Username = string.Empty,
					Email = string.Empty,
					FirstName = string.Empty,
					LastName = string.Empty
				});
		}

		//public static void RedirectToLocal(this Controller controller, string returnUrl)
		//{
		//	var context = controller.HttpContext;
		//	var rc = new RequestContext(context, controller.RouteData);
		//	var helper = new UrlHelper(rc);
		//	if (controller.Url.IsLocalUrl(returnUrl))
		//		context.Response.Redirect(returnUrl);
		//	else
		//		context.Response.Redirect(helper.Action("Index", "Home"));
		//}

		public static ActionResult RedirectToLocal(
			this Controller self,
			string returnUrl,
			string controller = "Home",
			string action = "Index")
		{
			return self.Url.IsLocalUrl(returnUrl) ?
				new RedirectResult(returnUrl) as ActionResult
				: new RedirectToRouteResult(
					new RouteValueDictionary(
						new
						{
							controller = controller,
							action = action
						})) as ActionResult;
		}
	}
}