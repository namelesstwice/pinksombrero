using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;

namespace PinkSombrero.Web
{
	public class AspNetKernelAdapter : IServiceProvider
	{
		public AspNetKernelAdapter(IKernel kernel)
		{
			Kernel = kernel;
			Kernel.Components.Add<IMissingBindingResolver, GenericOptionsResolver>();
			Kernel.Settings.AllowNullInjection = true;
		}

		public IKernel Kernel { get; }

		public object GetService(Type serviceType)
		{
			if (typeof(IEnumerable).IsAssignableFrom(serviceType))
			{
				var genericArgs = serviceType.GetGenericArguments();

				if (genericArgs.Length != 1)
					return Kernel.Get(serviceType);

				var genericArg = genericArgs[0];

				return castSlow(Kernel.GetAll(genericArg), genericArg);
			}

			return Kernel.TryGet(serviceType);
		}

		private static IEnumerable castSlow(IEnumerable series, Type elementType)
		{
			var method = _cast.MakeGenericMethod(elementType);
			return method.Invoke(null, new [] { series }) as IEnumerable;
		}

		private static readonly MethodInfo _cast = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast));
	}
}