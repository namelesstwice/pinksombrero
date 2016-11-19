using System;
using System.DirectoryServices.Protocols;
using System.Net;

namespace PinkSombrero.Web.Services
{
	public class ActiveDirectoryService
	{
		public bool ValidateCredentials(string login, string password, out string normalizedName)
		{
			string username, domain;
			if (!tryParseDomainName(login, out username, out domain))
			{
				normalizedName = null;
				return false;
			}

			normalizedName = $"{domain.ToUpper()}/{username.ToUpper()}";
			var credentials = new NetworkCredential(username, password, domain);

			var id = new LdapDirectoryIdentifier(domain);

			using (var connection = new LdapConnection(id, credentials, AuthType.Kerberos))
			{
				connection.SessionOptions.Sealing = true;
				connection.SessionOptions.Signing = true;

				try
				{
					connection.Bind();
				}
				catch (LdapException lEx)
				{
					if (lEx.ErrorCode == _errorLogonFailure)
					{
						return false;
					}
					throw;
				}
			}
			return true;
		}

		private bool tryParseDomainName(string fullName, out string username, out string domain)
		{
			int separatorIndex = -1;
			
			if ((separatorIndex = fullName.IndexOf("/", StringComparison.Ordinal)) > -1)
			{
				domain = fullName.Substring(0, separatorIndex);
				username = fullName.Substring(separatorIndex + 1, fullName.Length - separatorIndex - 1);
				return true;
			}

			if ((separatorIndex = fullName.IndexOf("\\", StringComparison.Ordinal)) > -1)
			{
				domain = fullName.Substring(0, separatorIndex);
				username = fullName.Substring(separatorIndex + 1, fullName.Length - separatorIndex - 1);
				return true;
			}

			if ((separatorIndex = fullName.IndexOf("@", StringComparison.Ordinal)) > -1)
			{
				username = fullName.Substring(0, separatorIndex);
				domain = fullName.Substring(separatorIndex + 1, fullName.Length - separatorIndex - 1);
				return true;
			}

			username = domain = null;
			return false;
		}

		private const int _errorLogonFailure = 0x31;
	}
}
