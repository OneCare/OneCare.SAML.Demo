
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Assertions;

using OneCare.Partner.IdP.Utils;
using OneCare.Partner.IdP.Models;


namespace OneCare.Partner.IdP.Controllers
{
	public class SAMLController : Controller
	{
		const string SSOReceivedSessionKey = "SSOReceived";
		const string PartnerSPSessionKey = "PartnerSP";

		public ActionResult SSOService()
		{
			// This method is called either by an SP request for authentication or,
			// as part of that process, by the local AccountController Login method.
			// Once we've received the request, the Session will be set with a temporary
			// flag to indicate that we have already retrieved the SP authentication
			// request.
			var ssoReceived = Session.GetValue(SSOReceivedSessionKey, false);

			var partnerSP = default(string);
			if (!ssoReceived)
			{
				SAMLIdentityProvider.ReceiveSSO(Request, out partnerSP);
				Session[SSOReceivedSessionKey] = true;
				Session[PartnerSPSessionKey] = partnerSP;
			}
			else
				partnerSP = Session.GetValue(PartnerSPSessionKey);

			// If a user isn't logged in locally, get them logged in.  We'll return to
			// this method via the returnUrl.
			UpdateUser();
			if (!User.Identity.IsAuthenticated)
				return RedirectToAction("LogIn", "Account", new { returnUrl = Request.Url.AbsolutePath });

			// If we are here, then the user is logged in locally, we've received
			// an external request for authentication from an SP, and we need to send a 
			// SAML response.
			Session[SSOReceivedSessionKey] = null;
			Session[PartnerSPSessionKey] = null;

			var user = Database.Find<UserDoc>(u => u.Id == (User as UserModel).Id);
			var attributes = new Dictionary<string, string>();
			foreach (var attr in user.Attributes.Where(a => a.PartnerSP == partnerSP))
				attributes[attr.Name] = attr.Value;
			
			SAMLIdentityProvider.SendSSO(
				Response,
				User.Identity.Name,
				attributes as IDictionary<string, string>);

			return new EmptyResult();
		}

		public ActionResult SLOService()
		{
			var isRequest = false;
			var hasCompleted = false;
			var logoutReason = default(string);
			var partnerSP = default(string);

			SAMLIdentityProvider.ReceiveSLO(
				Request,
				Response,
				out isRequest,
				out hasCompleted,
				out logoutReason,
				out partnerSP);

			// If this is a request, then the logout was initiated by a partner SP.
			if (isRequest)
			{
				Response.ForgetUserIdentity();
				SAMLIdentityProvider.SendSLO(Response, null);
			}
			else if (hasCompleted)
			{
				// IdP-initiated SLO has completed.
				Response.ForgetUserIdentity();
				Response.Redirect("~/");
			}

			return new EmptyResult();
		}

		void UpdateUser()
		{
			try
			{
				var cookie = Request.Cookies[AccountController.UserCookieName];
				if (cookie == null)
					cookie = HttpContext.Session[AccountController.UserCookieName] as HttpCookie;
				if (cookie != null)
				{
					var data = cookie.Value;
					if (!string.IsNullOrEmpty(data))
					{
						var values = FormsAuthentication.Decrypt(data).UserData.Split(';');
						if (!string.IsNullOrEmpty(values[0]))
						{
							var user = new UserModel
							{
								Id = values[0],
								Username = values[1],
								Email = values[2],
								FirstName = values[3],
								LastName = values[4],
							};
							HttpContext.User = user;
						}
						else
							HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
					}
				}
				else
					HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
			}
			catch
			{
				HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
			}
		}
	}
}
