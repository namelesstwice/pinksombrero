using System;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Extensions.ChildKernel;

namespace PinkSombrero.Web
{
	class NinjectScope : IServiceScope
	{
		public NinjectScope(IKernel root)
		{
			_childKernel = new ChildKernel(root);
			ServiceProvider = _childKernel.ForAspNet();
		}

		public void Dispose()
		{
			_childKernel.Dispose();
		}

		public IServiceProvider ServiceProvider { get; }

		private readonly ChildKernel _childKernel;
	}
}