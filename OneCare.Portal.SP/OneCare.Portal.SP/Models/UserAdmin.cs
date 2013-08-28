
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace OneCare.Portal.SP.Models
{
	public class AdminDoc
	{
		public string Id { get; set; }
		public string UserId { get; set; }
	}


	public class UserAdminModel
	{
		[Display(Name = "ID")]
		public string Id { get; set; }

		//[Required]
		[Display(Name = "First name")]
		public string FirstName { get; set; }

		//[Required]
		[Display(Name = "Last name")]
		public string LastName { get; set; }

		//[Required]
		[Display(Name = "Email address")]
		public string Email { get; set; }

		//[Required]
		[Display(Name = "User name")]
		public string Username { get; set; }

		//[Required]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		//[Required]
		[Display(Name = "Is administrator?")]
		public bool IsAdmin { get; set; }
	}


	public static class UserAdminExt
	{
		public static bool IsAdmin(this UserModel user)
		{
			return Database.Has<AdminDoc>(admin =>
				admin.UserId == user.Id);
		}

		public static bool IsAdmin(this UserDoc user)
		{
			return Database.Has<AdminDoc>(admin =>
				admin.UserId == user.Id);
		}

		public static UserAdminModel NewUserAdmin(this UserDoc doc)
		{
			return (doc == null) ? null :
				new UserAdminModel
				{
					Id = doc.Id.Replace("/", "-"),
					Username = doc.Username,
					Password = doc.Password,
					Email = doc.Email,
					FirstName = doc.FirstName,
					LastName = doc.LastName,
					IsAdmin = doc.IsAdmin()
				};
		}

		public static void Load(this UserDoc doc, UserAdminModel model)
		{
			if ((doc != null) && (model != null))
			{
				if (!string.IsNullOrEmpty(model.Username) && (model.Username != doc.Username))
					doc.Username = model.Username;
				if (!string.IsNullOrEmpty(model.Password) && (model.Password != doc.Password))
					doc.Password = Database.Encrypt(model.Password);
				if (!string.IsNullOrEmpty(model.Email) && (model.Email != doc.Email))
					doc.Email = model.Email;
				if (!string.IsNullOrEmpty(model.FirstName) && (model.Username != doc.FirstName))
					doc.FirstName = model.FirstName;
				if (!string.IsNullOrEmpty(model.LastName) && (model.Username != doc.LastName))
					doc.LastName = model.LastName;
			}
		}

		public static UserDoc NewUserDoc(this UserAdminModel model)
		{
			return new UserDoc
			{
				Id = (model.Id == null) ? null : model.Id.Replace("-", "/"),
				Username = model.Username,
				Password = Database.Encrypt(model.Password),
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				ExternalAccounts = null
			};
		}
	}
}