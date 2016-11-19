using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;

namespace PinkSombrero.Processing
{
	class ConfigurationModule : NinjectModule
	{
		public override void Load()
		{
			var currentExecutablePath = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

			var builder = new ConfigurationBuilder()
				.SetBasePath(currentExecutablePath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile("appsettings.agentSpecific.json", optional: true);
			
			Kernel.Bind<IConfigurationRoot>().ToConstant(builder.Build());
		}
	}
}