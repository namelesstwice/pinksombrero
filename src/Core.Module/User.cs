using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local - used by MongoDB C# driver
// ReSharper disable UnusedAutoPropertyAccessor.Local - used by MongoDB C# driver

namespace PinkSombrero.Core
{
	public class User
	{
		public User(string id, bool isAdmin = false)
		{
			Id = id;
			PermanentRoles = new List<DatabaseRole>();

			if (isAdmin)
				AccessRights = AccessRights.Admin;
		}

		[BsonId]
		public string Id { get; private set; }

		[BsonElement("accessRights"), BsonRequired]
		public AccessRights AccessRights { get; private set; }

		[BsonElement("databaseUsername"), BsonIgnoreIfNull]
		public string DatabaseUsername { get; private set; }

		[BsonElement("permanentRoles"), BsonRequired]
		public IReadOnlyCollection<DatabaseRole> PermanentRoles { get; private set; }

		[BsonElement("writeSession"), BsonIgnoreIfNull]
		public DatabaseWriteSession WriteSession { get; private set; }
	}
}