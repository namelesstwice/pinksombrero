using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace PinkSombrero.Core
{
	public class UserRepository
	{
		public UserRepository(IMongoDatabase mongoDatabase)
		{
			_writeAccessLog = mongoDatabase.GetCollection<UserWriteAccessLogEntry>("Users.WriteAccessRequests");
			_userCollection = mongoDatabase.GetCollection<User>("Users");
		}

		public async Task<IReadOnlyCollection<User>> GetAsync(int skip, int limit)
			=> await _userCollection.Find(u => true).Skip(skip).Limit(limit).ToListAsync();

		public Task<User> GetAsync(string id)
			=> _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

		public async Task<IReadOnlyCollection<User>> GetUsersWithOutdatedWriteAccess()
			=> await _userCollection.Find(u => u.WriteSession != null && u.WriteSession.ExpireTime < DateTime.UtcNow).ToListAsync();

		public Task InsertAsync(User user)
			=> _userCollection.InsertOneAsync(user);

		public Task UpdateAccessRightsAsync(User user, AccessRights newRights)
			=> _userCollection.UpdateOneAsync(
				u => u.Id == user.Id,
				Builders<User>.Update.Set(u => u.AccessRights, newRights));

		public Task UpdateDatabaseUsername(User user, string newDatabaseUsername)
			=> _userCollection.UpdateOneAsync(
				u => u.Id == user.Id,
				Builders<User>.Update.Set(u => u.DatabaseUsername, newDatabaseUsername));

		public Task UpdateWriteSession(User user, DatabaseWriteSession writeSession)
			=> _userCollection.UpdateOneAsync(
				u => u.Id == user.Id,
				Builders<User>.Update.Set(u => u.WriteSession, writeSession));

		public Task<bool> HasAnyUsers()
			=> _userCollection.Find(u => true).AnyAsync();

		public Task AddWriteAccessRequestAsync(UserWriteAccessLogEntry logEntry)
			=> _writeAccessLog.InsertOneAsync(logEntry);

		private readonly IMongoCollection<User> _userCollection;
		private readonly IMongoCollection<UserWriteAccessLogEntry> _writeAccessLog;
	}
}