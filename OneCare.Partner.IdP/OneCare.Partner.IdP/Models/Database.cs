
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Raven.Client;
using Raven.Client.Document;
using Raven.Abstractions.Data;


namespace OneCare.Partner.IdP.Models
{
	public class Database
	{
		static DocumentStore mDB = null;

		[ThreadStatic]
		static IDocumentSession mSession = null;

		public static DocumentStore Store
		{
			get
			{
				if (mDB == null)
				{
					mDB = new DocumentStore
					{
						ConnectionStringName = "RavenServerDB"
					};
					mDB.Initialize();
				}
				return mDB;
			}
		}

		public static T Read<T>(Func<IDocumentSession, T> op)
		{
			var result = default(T);
			if (mSession == null)
			{
				using (mSession = OpenReadSession())
				{
					result = op(mSession);
				}
				mSession = null;
			}
			else
				result = op(mSession);
			return result;
		}

		public static IEnumerable<T> Select<T>(Func<T, bool> filter)
		{
			return Read<IEnumerable<T>>(session =>
				session.Query<T>().Where(filter));
		}

		public static IEnumerable<T> List<T>()
		{
			return Read<IEnumerable<T>>(session =>
				session.Query<T>().ToList());
		}

		public static T Find<T>(string id)
		{
			return Read<T>(session =>
				session.Load<T>(id));
		}

		public static T Find<T>(Func<T, bool> filter)
		{
			return Read<T>(session =>
				session.Query<T>().FirstOrDefault(filter));
		}

		public static bool Has<T>(Func<T, bool> filter)
		{
			return Find<T>(filter) != null;
		}

		public static void Write(Action<IDocumentSession> op)
		{
			if (mSession == null)
			{
				using (mSession = OpenWriteSession())
				{
					if (op != null)
						op(mSession);
					mSession.SaveChanges();
				}
				mSession = null;
			}
			else
			{
				op(mSession);
				mSession.SaveChanges();
			}
		}

		public static async Task<T> Write<T>(Func<IDocumentSession, Task<T>> op)
		{
			var result = default(T);
			if (mSession == null)
			{
				using (mSession = OpenWriteSession())
				{
					result = await op(mSession);
					mSession.SaveChanges();
				}
				mSession = null;
			}
			else
			{
				result = await op(mSession);
				mSession.SaveChanges();
			}
			return result;
		}

		public static T Insert<T>(T entity)
		{
			Write(session =>
				session.Store(entity));
			return entity;
		}

		public static T Update<T>(T entity)
		{
			Write(null);
			return entity;
		}

		public static string Encrypt(string plaintext)
		{
			return Convert.ToBase64String(
				(new SHA256Managed()).ComputeHash(
					(new UnicodeEncoding()).GetBytes(plaintext)));
		}

		static IDocumentSession OpenReadSession()
		{
			return mSession ?? Store.OpenSession();
		}

		static IDocumentSession OpenWriteSession()
		{
			return mSession ?? Store.OpenSession();
		}
	}
}