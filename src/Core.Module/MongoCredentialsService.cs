using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PinkSombrero.Core
{
	public class MongoCredentialsService
	{
		public MongoCredentialsService(IEnumerable<MongoCluster> targetDatabaseConfigs)
		{
			_clustersById = targetDatabaseConfigs.ToDictionary(c => c.ClusterId);
		}

		public IEnumerable<MongoCluster> GetClusters() => _clustersById.Values;

		public async Task<string> CreateUser(User user, string password)
		{
			foreach (var mongoCluster in _clustersById.Values)
			{
				try
				{
					await createUser(mongoCluster, user.Id, password);
				}
				// 11000 == пользователь с таким именем уже существует
				catch (MongoCommandException e) when (e.Code == 11000)
				{
					await createUser(mongoCluster, user.Id, password, updateExisting: true);
				}
			}

			return user.Id;
		}

		public async Task GrantWriteAccess(User user, IEnumerable<string> clusterIds)
		{
			var writeConcern = WriteConcern.WMajority
				.With(wTimeout: TimeSpan.FromMilliseconds(5000));

			foreach (var clusterId in clusterIds)
			{
				var mongoCluster = _clustersById[clusterId];

				var command = new BsonDocument
				{
					{ "updateUser", user.Id },
					{ "roles", new BsonArray(mongoCluster.PermanentRoles.Concat(mongoCluster.TemporaryRoles).Select(r => new BsonDocument {
						{ "role", r.RoleName},
						{ "db", r.DatabaseName} }))
					},
					{ "writeConcern", writeConcern.ToBsonDocument() }
				};

				await mongoCluster.AdminDB.RunCommandAsync<BsonDocument>(command);
			}
		}

		public async Task RevokeWriteAccess(User user)
		{
			var writeConcern = WriteConcern.WMajority
				.With(wTimeout: TimeSpan.FromMilliseconds(5000));

			foreach (var mongoCluster in _clustersById.Values)
			{
				var command = new BsonDocument
				{
					{"updateUser", user.Id},
					{ "roles", new BsonArray(mongoCluster.PermanentRoles.Select(r => new BsonDocument {
						{ "role", r.RoleName},
						{ "db", r.DatabaseName} }))
					},
					{"writeConcern", writeConcern.ToBsonDocument()}
				};

				await mongoCluster.AdminDB.RunCommandAsync<BsonDocument>(command);
			}
		}

		private async Task createUser(MongoCluster mongoCluster, string name, string password, bool updateExisting = false)
		{
			var writeConcern = WriteConcern.WMajority
				.With(wTimeout: TimeSpan.FromMilliseconds(5000));

			// Construct the createUser command.
			var command = new BsonDocument
			{
				{ updateExisting ? "updateUser" : "createUser", name },
				{ "pwd", password },
				{ "customData", new BsonDocument("createdBy", "mongodb credentials service") },
				{ "roles", new BsonArray(mongoCluster.PermanentRoles.Select(r => new BsonDocument {
					{ "role", r.RoleName},
					{ "db", r.DatabaseName} }))
				},
				{ "writeConcern", writeConcern.ToBsonDocument() }
			};

			await mongoCluster.AdminDB.RunCommandAsync<BsonDocument>(command);
		}

		private readonly Dictionary<string, MongoCluster> _clustersById;
	}
}