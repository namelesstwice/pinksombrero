using Ninject;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ninject.Syntax;

namespace PinkSombrero.Web
{
	public static class NinjectRegistrationExtensions
	{
		public static IServiceProvider PopulateServices(this IKernel kernel, IServiceCollection serviceCollection)
		{
			var wrapper = kernel.ForAspNet();
			kernel.Bind<IServiceProvider>().ToConstant(wrapper);
			kernel.Bind<IServiceScopeFactory>().To<NinjectScopeFactory>().InSingletonScope();

			foreach (var svc in serviceCollection)
			{
				if (svc.ImplementationType == typeof(OptionsManager<>))
					continue;

				if (svc.ImplementationType != null)
				{
					kernel.Bind(svc.ServiceType).To(svc.ImplementationType).inScope(svc.Lifetime);
				}
				else if (svc.ImplementationFactory != null)
				{
					kernel.Bind(svc.ServiceType).ToMethod(ctx => svc.ImplementationFactory(wrapper)).inScope(svc.Lifetime);
				}
				else
				{
					kernel.Bind(svc.ServiceType).ToConstant(svc.ImplementationInstance).inScope(svc.Lifetime);
				}
			}

			return wrapper;
		}

		public static IServiceProvider ForAspNet(this IKernel kernel)
		{
			return new AspNetKernelAdapter(kernel);
		}

		private static void inScope(this IBindingInSyntax<object> to, ServiceLifetime lifetime)
		{
			switch (lifetime)
			{
				case ServiceLifetime.Singleton:
					to.InSingletonScope();
					break;
				case ServiceLifetime.Scoped:
					to.InScope(ctx => ctx.Kernel);
					break;
				case ServiceLifetime.Transient:
					to.InTransientScope();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
			}
		}
	}
}