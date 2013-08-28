
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OneCare.Portal.SP.Models;


namespace OneCare.Portal.SP.Controllers
{
	public class UserAdminController : Controller
	{
		//
		// GET: /UserAdmin/

		[Authorize(Roles = "Admin")]
		public ActionResult Index()
		{
			return View(Database.List<UserDoc>().Select(doc => doc.NewUserAdmin()));
		}

		//
		// GET: /UserAdmin/Details/5

		[Authorize(Roles = "Admin")]
		public ActionResult Details(string id)
		{
			var user = Database.Find<UserDoc>(id.Replace("-", "/")).NewUserAdmin();
			return View(user);
		}

		//
		// GET: /UserAdmin/Create

		[Authorize(Roles = "Admin")]
		public ActionResult Create()
		{
			return View();
		}

		//
		// POST: /UserAdmin/Create

		[HttpPost]
		public ActionResult Create(UserAdminModel user)
		{
			try
			{
				// TODO: Add insert logic here
				Database.Write(session =>
				{
					var doc = user.NewUserDoc();
					session.Store(doc);
					if (user.IsAdmin)
						session.Store(new AdminDoc { Id = null, UserId = doc.Id });
				});
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /UserAdmin/Edit/5

		[Authorize(Roles = "Admin")]
		public ActionResult Edit(string id)
		{
			var user = Database.Find<UserDoc>(id.Replace("-", "/")).NewUserAdmin();
			return View(user);
		}

		//
		// POST: /UserAdmin/Edit/5

		[HttpPost]
		public ActionResult Edit(string id, UserAdminModel newUser)//FormCollection collection)
		{
			try
			{
				// TODO: Add update logic here
				Database.Write(session =>
				{
					var user = Database.Find<UserDoc>(id.Replace("-", "/"));
					user.Load(newUser);

					var admin = Database.Find<AdminDoc>(a => a.UserId == user.Id);
					if ((admin == null) && newUser.IsAdmin)
						session.Store(new AdminDoc { Id = null, UserId = user.Id });
					else if ((admin != null) && !newUser.IsAdmin)
						session.Delete(admin);
				});
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		//
		// GET: /UserAdmin/Delete/5

		[Authorize(Roles = "Admin")]
		public ActionResult Delete(string id)
		{
			var user = Database.Find<UserDoc>(id.Replace("-", "/")).NewUserAdmin();
			return View(user);
		}

		//
		// POST: /UserAdmin/Delete/5

		[HttpPost]
		public ActionResult Delete(string id, FormCollection collection)
		{
			try
			{
				// TODO: Add delete logic here
				Database.Write(session =>
				{
					var userId = id.Replace("-", "/");
					var admin = Database.Find<AdminDoc>(a => a.UserId == userId);
					if (admin != null)
						session.Delete(admin);
					var user = Database.Find<UserDoc>(userId);
					if (user != null)
						session.Delete(user);
				});
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}
	}
}
