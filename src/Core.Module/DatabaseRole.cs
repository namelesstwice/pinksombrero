using MongoDB.Bson.Serialization.Attributes;

namespace PinkSombrero.Core
{
	public class DatabaseRole
	{
		public DatabaseRole(string roleName, string databaseName)
		{
			RoleName = roleName;
			DatabaseName = databaseName;
		}

		[BsonElement("roleName")]
		public string RoleName { get; private set; }

		[BsonElement("dbName")]
		public string DatabaseName { get; private set; }
	}
}