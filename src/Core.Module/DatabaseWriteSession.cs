using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PinkSombrero.Core
{
	public class DatabaseWriteSession
	{
		public DatabaseWriteSession(IReadOnlyCollection<string> clusterIds, DateTime expireTime)
		{
			if (clusterIds == null) throw new ArgumentNullException(nameof(clusterIds));
			if (clusterIds.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(clusterIds));

			ExpireTime = expireTime;
			ClusterIds = clusterIds;
		}

		[BsonId]
		public ObjectId Id { get; private set; }

		[BsonElement("clusterIds"), BsonRequired]
		public IReadOnlyCollection<string> ClusterIds { get; private set; }

		[BsonElement("expireTime"), BsonRequired]
		public DateTime ExpireTime { get; private set; }
	}
}