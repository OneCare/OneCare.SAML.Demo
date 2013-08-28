
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;


namespace OneCare.Partner.SP.Models
{
	public class UserModel : IPrincipal, IIdentity
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public IIdentity Identity
		{
			get
			{
				return this;
			}
		}

		public bool IsInRole(string role)
		{
			return false;// (role.ToLower() == "admin") ? this.IsAdmin() : false;
		}

		public string AuthenticationType
		{
			get
			{
				return "Sam's";
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return !string.IsNullOrEmpty(Username);
			}
		}

		public string Name
		{
			get
			{
				return Username;
			}
		}
	}


	public class UserDoc
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}


	public class RegisterModel
	{
		[Required]
		[Display(Name = "First name")]
		public string FirstName { get; set; }

		[Required]
		[Display(Name = "Last name")]
		public string LastName { get; set; }

		[Required]
		[Display(Name = "Email address")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "User name")]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		[Display(Name = "Confirm password")]
		public string ConfirmPassword { get; set; }
	}


	public class LoginModel
	{
		[Required]
		[Display(Name = "User name")]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}


	public static class UserExt
	{
		public static UserModel NewUser(this UserDoc doc)
		{
			return (doc == null) ? null :
				new UserModel
				{
					Id = doc.Id,
					Username = doc.Username,
					Email = doc.Email,
					FirstName = doc.FirstName,
					LastName = doc.LastName,
				};
		}

		public static UserDoc NewUser(this RegisterModel model)
		{
			return (model == null) ? null :
				new UserDoc
				{
					Id = null,
					Username = model.Username,
					Password = Database.Encrypt(model.Password),
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
				};
		}
	}
}