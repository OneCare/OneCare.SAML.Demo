
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

using ComponentSpace.SAML2.Configuration;

using OneCare.Partner.IdP.Models;


namespace OneCare.Partner.IdP
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			// Load the saml.config configuration.
			SAMLConfiguration.Load();
		}

		protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
		{
			try
			{
				var cookie = Request.Cookies[OneCare.Partner.IdP.Controllers.AccountController.UserCookieName];
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
							Context.User = user;
						}
						else
							Context.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
					}
				}
			}
			catch
			{
				Context.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
			}
		}
	}
}