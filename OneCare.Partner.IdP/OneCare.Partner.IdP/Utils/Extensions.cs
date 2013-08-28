
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using OneCare.Partner.IdP.Controllers;
using OneCare.Partner.IdP.Models;
using System.Security.Cryptography;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace OneCare.Partner.IdP.Utils
{
	public static class Extensions
	{
		public static string GetValue(this HttpSessionStateBase session, string key, string defaultValue = null)
		{
			var result = session[key];
			return (result == null) ? defaultValue : result as string;
		}

		public static int GetValue(this HttpSessionStateBase session, string key, int defaultValue)
		{
			var result = session[key];
			return (result == null) ? defaultValue
				: (result is string) ? int.Parse(result as string)
				: (int)result;
		}

		public static bool GetValue(this HttpSessionStateBase session, string key, bool defaultValue)
		{
			var result = session[key];
			return (result == null) ? defaultValue
				: (result is string) ? bool.Parse(result as string)
				: (bool)result;
		}

		public static void SaveUserIdentity(this HttpResponseBase response, UserModel user)
		{
			var userCookieName = AccountController.UserCookieName;

			var value = string.Format(
				"{0};{1};{2};{3};{4}",
				user.Id,
				user.Username,
				user.Email,
				user.FirstName,
				user.LastName);
			var ticket = new FormsAuthenticationTicket(
				1,
				userCookieName,
				DateTime.Now,
				DateTime.Now.AddDays(30),
				true,
				value,
				FormsAuthentication.FormsCookiePath);
			var cookie = new HttpCookie(
				userCookieName,
				FormsAuthentication.Encrypt(ticket));
			response.SetCookie(cookie);
			HttpContext.Current.Session[userCookieName] = cookie;
		}

		public static void ForgetUserIdentity(this HttpResponseBase response)
		{
			response.SaveUserIdentity(
				new UserModel
				{
					Id = string.Empty,
					Username = string.Empty,
					Email = string.Empty,
					FirstName = string.Empty,
					LastName = string.Empty
				});
		}

		//public static string Encrypt(this SymmetricAlgorithm algorithm, byte[] key, byte[] iv, string plaintext)
		//{
		//	var result = default(string);
		//	if ((algorithm != null) && !string.IsNullOrEmpty(plaintext))
		//	{
		//		using (var mem = new MemoryStream())
		//		{
		//			var encryptor = algorithm.CreateEncryptor(key, iv);
		//			using (var cs = new CryptoStream(mem, encryptor, CryptoStreamMode.Write))
		//			{
		//				using (var sw = new StreamWriter(cs))
		//				{
		//					sw.Write(plaintext);
		//				}
		//			}
		//			result = Convert.ToBase64String(mem.ToArray());
		//		}
		//	}
		//	return result;
		//}

		//public static string Decrypt(this SymmetricAlgorithm algorithm, byte[] key, byte[] iv, string ciphertext)
		//{
		//	var result = default(string);
		//	if ((algorithm != null) && !string.IsNullOrEmpty(ciphertext))
		//	{
		//		using (var mem = new MemoryStream(Convert.FromBase64String(ciphertext)))
		//		{
		//			var decryptor = algorithm.CreateDecryptor(key, iv);
		//			using (var cs = new CryptoStream(mem, decryptor, CryptoStreamMode.Read))
		//			{
		//				using (var sr = new StreamReader(cs))
		//				{
		//					result = sr.ReadToEnd();
		//				}
		//			}
		//		}
		//	}
		//	return result;
		//}

		//public static string Encrypt(this X509Certificate2 cert, string plaintext)
		//{
		//	var rsa = cert.PublicKey.Key as RSACryptoServiceProvider;
		//	return Convert.ToBase64String(rsa.Encrypt(plaintext.GetBytes(), false));
		//}

		//public static string Decrypt(this X509Certificate2 cert, string ciphertext)
		//{
		//	var rsa = cert.PrivateKey as RSACryptoServiceProvider;
		//	return Encoding.ASCII.GetString(rsa.Decrypt(Convert.FromBase64String(ciphertext), false));
		//}

		//public static string EncryptAES(this X509Certificate2 cert, byte[] key, byte[] iv, string plaintext)
		//{
		//	var result = default(string);
		//	if ((cert != null) && !string.IsNullOrEmpty(plaintext))
		//	{
		//		var algorithm = new RijndaelManaged();
		//		var encryptedKey = cert.Encrypt(algorithm.Key.GetString());
		//		var encryptedIV = cert.Encrypt(algorithm.IV.GetString());
		//		var encryptedText = algorithm.Encrypt(key, iv, plaintext);
		//		result = string.Format("{0}{1}{2}", encryptedKey, encryptedIV, encryptedText);
		//	}
		//	return result;
		//}

		//public static string DecrypteAES(this X509Certificate2 cert, string ciphertext)
		//{
		//	var result = default(string);
		//	if ((cert != null) && !string.IsNullOrEmpty(ciphertext))
		//	{
		//		var pos1 = ciphertext.IndexOf('=');
		//		var encryptedKey = ciphertext.Substring(0, pos1);

		//		var pos2 = ciphertext.IndexOf('=', pos1 + 1);
		//		var encryptedIV = ciphertext.Substring(pos1 + 1, pos2 - pos1);

		//		pos1 = ciphertext.IndexOf('=', pos2 + 1);
		//		var encryptedText = ciphertext.Substring(pos1 + 1);

		//		var algorithm = new RijndaelManaged();
		//		result = algorithm.Decrypt(
		//			cert.Decrypt(encryptedKey).GetBytes(),
		//			cert.Decrypt(encryptedIV).GetBytes(),
		//			ciphertext);
		//	}
		//	return result;
		//}

		//public static string GetString(this byte[] b)
		//{
		//	return Convert.ToBase64String(b);
		//}

		//public static byte[] GetBytes(this string s)
		//{
		//	var result = default(byte[]);
		//	if (!string.IsNullOrEmpty(s))
		//	{
		//		result = new byte[s.Length * sizeof(char)];
		//		System.Buffer.BlockCopy(s.ToCharArray(), 0, result, 0, s.Length);
		//	}
		//	return result;
		//}
	}
}