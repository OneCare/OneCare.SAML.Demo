
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OneCare.Portal.SP.Models;


namespace OneCare.Portal.SP.Controllers
{
	public static class AppSettings
	{
		public const string IdentityProviderKey = "IdentityProvider";
	}

	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			//Database.Insert(
			//	new ExternalAppDoc
			//	{
			//		Name = "OneCarePartnerSP",
			//		Password = "WhoKnowsMyPassword",
			//		IdP = "urn:onecare:OneCare.Partner.IdP"
			//	});
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your app description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}
