
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
	public class SAMLController : Controller
	{
		public const string AttributesSessionKey = "SAMLAttributes";

		public ActionResult AssertionConsumerService()
		{
			var isInResponseTo = false;
			var partnerIdP = default(string);
			var userName = default(string);
			var attributes = default(IDictionary<string, string>);
			var targetUrl = default(string);

			// Receive and process the SAML assertion contained in the SAML response.
			// The SAML response is received either as part of IdP-initiated or SP-initiated SSO.
			SAMLServiceProvider.ReceiveSSO(
				Request,
				out isInResponseTo,
				out partnerIdP,
				out userName,
				out attributes,
				out targetUrl);

			// If no target URL is provided, provide a default.
			if (targetUrl == null)
				targetUrl = "~/";

			var userId = attributes["UserId"];
			var user = Database.Find<UserDoc>(u => u.Id == userId);
			Response.SaveUserIdentity(user.NewUser());

			// Save the attributes.
			Session[AttributesSessionKey] = attributes;

			// Redirect to the target URL.
			return RedirectToLocal(targetUrl);
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
