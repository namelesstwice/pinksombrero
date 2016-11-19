using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace PinkSombrero.Core
{
	public class MongoCluster
	{
		public MongoCluster(string clusterId, string connectionString, IEnumerable<DatabaseRole> permanentRoles, IEnumerable<DatabaseRole> temporaryRoles)
		{
			ConnectionString = connectionString;
			ClusterId = clusterId;
			PermanentRoles = permanentRoles.ToArray();
			TemporaryRoles = temporaryRoles.ToArray();

			AdminDB = new MongoClient(ConnectionString).GetDatabase("admin");
		}

		public string ClusterId { get; private set; }
		public string ConnectionString { get; private set; }
		public IReadOnlyCollection<DatabaseRole> PermanentRoles { get; private set; }
		public IReadOnlyCollection<DatabaseRole> TemporaryRoles { get; private set; }

		public IMongoDatabase AdminDB { get; }
	}
}