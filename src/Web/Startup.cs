using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninject;
using PinkSombrero.Web.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PinkSombrero.Web
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile("appsettings.agentSpecific.json", optional: true);

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			var kernel = new StandardKernel();
			
			kernel.Load(new Core.Module());

			kernel.Bind<IConfigurationRoot>().ToConstant(Configuration);
			kernel.Bind<ActiveDirectoryService>().ToSelf().InSingletonScope();
			kernel.Bind<AuthService>().ToSelf().InSingletonScope();

			return kernel.PopulateServices(services);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationScheme = AuthService.Scheme,
				LoginPath = new PathString("/Auth/Login/"),
				AutomaticAuthenticate = true,
				AutomaticChallenge = true
			});

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Credentials}/{action=Index}/{id?}");
			});
		}
	}
}
