using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local - used by MongoDB C# driver
// ReSharper disable UnusedAutoPropertyAccessor.Local - used by MongoDB C# driver

namespace PinkSombrero.Core
{
	public class UserWriteAccessLogEntry
	{
		public UserWriteAccessLogEntry(User user, DateTime requestEndTime, string reason)
		{
			Username = user.Id;
			RequestStartTime = DateTime.UtcNow;
			RequestEndTime = requestEndTime;
			Reason = reason;
		}

		[BsonId]
		public ObjectId Id { get; private set; }

		[BsonElement("username"), BsonRequired]
		public string Username { get; private set; }

		[BsonElement("reason"), BsonRequired]
		public string Reason { get; private set; }

		[BsonElement("requestStartTime"), BsonRequired]
		public DateTime RequestStartTime { get; private set; }

		[BsonElement("requestEndTime"), BsonRequired]
		public DateTime RequestEndTime { get; private set; }

		[BsonElement("duration"), BsonRequired]
		public TimeSpan Duration => RequestEndTime - RequestStartTime;
	}
}