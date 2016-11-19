using System;
using Microsoft.Extensions.DependencyInjection;

namespace PinkSombrero.Web
{
	class NinjectScopeFactory : IServiceScopeFactory
	{
		public NinjectScopeFactory(IServiceProvider root)
		{
			_root = (AspNetKernelAdapter) root;
		}

		public IServiceScope CreateScope()
		{
			return new NinjectScope(_root.Kernel);
		}

		private readonly AspNetKernelAdapter _root;
	}
}