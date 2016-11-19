using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;

namespace PinkSombrero.Web
{
	public class GenericOptionsResolver : NinjectComponent, IMissingBindingResolver
	{
		public IEnumerable<IBinding> Resolve(
			Multimap<Type, IBinding> bindings, IRequest request)
		{
			var service = request.Service;
			if (!service.IsGenericType)
				return Enumerable.Empty<IBinding>();

			var type = service.GetGenericArguments()[0];
			if (service.GetGenericTypeDefinition() != typeof(IOptions<>))
				return Enumerable.Empty<IBinding>();
			
			var validatorType = typeof(OptionsManager<>).MakeGenericType(type);
			var binding = new Binding(service)
			{
				ProviderCallback = StandardProvider.GetCreationCallback(validatorType)
			};

			return new[] { binding };
		}
	}
}