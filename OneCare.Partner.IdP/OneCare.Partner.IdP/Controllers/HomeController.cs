
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OneCare.Partner.IdP.Models;


namespace OneCare.Partner.IdP.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated && User.Identity.Name == "joe@partnerIdP")
			{
				Database.Write(session =>
				{
					var user = Database.Find<UserDoc>(u => u.Id == (User as UserModel).Id);
					if ((user != null) && (user.Attributes == null))
					{
						user.Attributes = new List<UserAttribute>();
						user.Attributes.Add(
							new UserAttribute
							{
								PartnerSP = "http://localhost/OneCare.Partner.SP",
								Name = "UserId",
								Value = "UserDocs/1"
							});
					}
				});
			}
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
