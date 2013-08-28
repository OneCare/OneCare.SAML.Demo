
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using OneCare.Partner.SP.Controllers;
using OneCare.Partner.SP.Models;


namespace OneCare.Partner.SP.Utils
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
	}
}