
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace OneCare.Portal.SP.Models
{
	public class ExternalAccountDoc
	{
		public string Id { get; set; }
		public string AppId { get; set; }
		public string Username { get; set; }
		public string Data { get; set; }
	}


	public class ExternalAccountModel
	{
		public string Id { get; set; }

		[Required]
		[Display(Name = "Application Id")]
		public string AppId { get; set; }

		[Required]
		[Display(Name = "User name")]
		public string Username { get; set; }

		[Display(Name = "Data")]
		public string Data { get; set; }
	}


	public class ExternalAppDoc
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public string IdP { get; set; }
	}
}
