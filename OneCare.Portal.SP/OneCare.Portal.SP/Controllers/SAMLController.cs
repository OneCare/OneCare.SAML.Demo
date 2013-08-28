
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ComponentSpace.SAML2;

using OneCare.Portal.SP.Utils;
using OneCare.Portal.SP.Models;


namespace OneCare.Portal.SP.Controllers
{
	public class SAMLController : Controller
	{
		public const string PartnerIdSessionKey = "SSOPartnerId";
		public const string SSOReceivedSessionKey = "SSOReceived";
		public const string IdPUsernameSessionKey = "IdPUsername";
		public const string NeedToUpdateUserSessionKey = "NeedToUpdateUser";
		public const string AssociatedUserIdSessionKey = "AssociatedUserId";

		public ActionResult AssertionConsumerService()
		{
			var ssoReceived = PartnerSSOData.SSOReceived;
			var partnerUsername = default(string);
			var user = default(UserDoc);

			// This is the first time through, retrieve the user info from the IdP.
			if (!ssoReceived)
			{
				var isInResponseTo = false;
				var partnerIdP = default(string);
				var attributes = default(IDictionary<string, string>);
				var targetUrl = default(string);

				// Receive and process the SAML assertion contained in the SAML response.
				// The SAML response is received either as part of IdP-initiated or SP-initiated SSO.
				SAMLServiceProvider.ReceiveSSO(
					Request,
					out isInResponseTo,
					out partnerIdP,
					out partnerUsername,
					out attributes,
					out targetUrl);
				PartnerSSOData.SSOReceived = true;
				PartnerSSOData.PartnerUsername = partnerUsername;

				// Try to find a single OneCare user that is associated the partner's user.
				var partnerAppId = PartnerSSOData.PartnerApp.Id;
				user = Database.FindUnique<UserDoc>(u => 
					(u.ExternalAccounts != null)
					&& u.ExternalAccounts.SingleOrDefault(a =>
						(a.AppId == partnerAppId)
						&& (a.Username == partnerUsername)) != null);
				if (user == null)
				{
					// Redirect to the partner-based OneCare login page and wait for it to return.
					return RedirectToAction("Associate", "PartnerSSO", new { returnUrl = Request.Url.AbsolutePath });
				}
			}
			else
			{
				// We had to associate a OneCare user with the partner's user so we
				// are here after returning from partner-based OneCare login page.
				user = PartnerSSOData.AssociatedUser;
			}

			// We have an associated user -- make sure they are logged in.
			Response.ForgetUserIdentity();
			Response.SaveUserIdentity(user.NewUser());
			PartnerSSOData.Clear();

			return RedirectToAction("Index", "Home");
		}

		public ActionResult SLOService()
		{
			// Receive the single logout request or response.
			// If a request is received then single logout is being initiated by the identity provider.
			// If a response is received then this is in response to single logout having been initiated by the service provider.
			var isRequest = false;
			var logoutReason = default(string);
			var partnerIdP = default(string);
			SAMLServiceProvider.ReceiveSLO(Request, out isRequest, out logoutReason, out partnerIdP);

			if (isRequest)
			{
				// Logout locally.
				Response.ForgetUserIdentity();

				// Respond to the IdP-initiated SLO request indicating successful logout.
				SAMLServiceProvider.SendSLO(Response, null);
			}
			else
			{
				// SP-initiated SLO has completed.
				Response.ForgetUserIdentity();
			}

			return RedirectToAction("Index", "Home", null);
		}

		ActionResult RedirectToLocal(string returnUrl)
		{
			return Url.IsLocalUrl(returnUrl) ?
				Redirect(returnUrl) as ActionResult
				: RedirectToAction("Index", "Home") as ActionResult;
		}
	}
}
