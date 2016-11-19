using Ninject;
using Topshelf;

namespace PinkSombrero.Processing
{
	class Program
	{
		public static void Main(string[] args)
		{
			var kernel = new StandardKernel();
			kernel.Load<ConfigurationModule>();
			kernel.Load<Core.Module>();
			kernel.Bind<OutdatedWriteSessionsProcessor>()
				.ToSelf()
				.InSingletonScope();

			HostFactory.Run(x =>
			{
				x.Service<OutdatedWriteSessionsProcessor>(s =>
				{
					s.ConstructUsing(name => kernel.Get<OutdatedWriteSessionsProcessor>());
					s.WhenStarted(tc => tc.Start());
					s.WhenStopped(tc => tc.Stop());
				});

				x.RunAsLocalSystem();

				x.SetDescription("Task processing for mongo credentials service");
				x.SetDisplayName("Task processing for mongo credentials service");
				x.SetServiceName("MongoCredSvc.Processing");

				x.EnableServiceRecovery(r =>
				{
					r.RestartService(0);
				});
			});
		}
	}
}
