using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Ninject;
using Ninject.Modules;

namespace PinkSombrero.Core
{
	public class Module : NinjectModule
	{
		public override void Load()
		{
			Kernel
				.Bind<IMongoDatabase>()
				.ToMethod(ctx =>
				{
					var ownDbConfig = ctx.Kernel.Get<IConfigurationRoot>().GetSection("database").GetSection("system");
					return new MongoClient(ownDbConfig["connectionString"]).GetDatabase(ownDbConfig["databaseName"]);
				})
				.InSingletonScope();

			Kernel
				.Bind<MongoCredentialsService>()
				.ToMethod(ctx => new MongoCredentialsService(parseTargetDatabaseConfig(ctx.Kernel.Get<IConfigurationRoot>())))
				.InSingletonScope();

			Kernel.Bind<UserRepository>().ToSelf().InSingletonScope();
		}

		private static IEnumerable<MongoCluster> parseTargetDatabaseConfig(IConfigurationRoot configurationRoot) => 
			from targetDbSection in configurationRoot.GetSection("database").GetSection("target").GetChildren()
			select new MongoCluster(
				targetDbSection["clusterId"],
				targetDbSection["connectionString"],
				targetDbSection.GetSection("permanentRoles").GetChildren().Select(parseRole),
				targetDbSection.GetSection("temporaryRoles").GetChildren().Select(parseRole));

		private static DatabaseRole parseRole(IConfigurationSection roleSection) 
			=> new DatabaseRole(roleSection["name"], roleSection["db"]);
	}
}