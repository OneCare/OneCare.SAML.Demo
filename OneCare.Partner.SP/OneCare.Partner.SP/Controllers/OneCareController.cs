
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

using ComponentSpace.SAML2.Utility;


namespace OneCare.Partner.SP.Controllers
{
	public class OneCareController : Controller
	{
		public ActionResult SSOLogin()
		{
			var url = WebConfigurationManager.AppSettings["OneCareSSOLoginUrl"];
			var appName = WebConfigurationManager.AppSettings["OneCareSSOLoginAppName"];
			var password = WebConfigurationManager.AppSettings["OneCareSSOLoginPassword"];

			Response.AddHeader("Authentication", string.Format("{0}:{1}", appName, password));
			Response.RedirectPermanent(url);
			//var request = HttpWebRequest.CreateHttp(url);
			////request.CookieContainer = new CookieContainer();

			//HttpBasicAuthentication.AddAuthorizationHeader(request, appName, password);

			//var response = request.GetResponse();
			//if (response.ContentType == "")
			//	throw new Exception("Invalid response.");

			return RedirectToAction("Index", "Home");
		}
	}
}
