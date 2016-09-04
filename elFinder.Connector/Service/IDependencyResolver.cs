using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Service
{
	public interface IDependencyResolver
	{
		TService Resolve<TService>();
		IDisposable BeginResolverScope();
	}

	public static class DependencyResolver
	{
		private static IDependencyResolver _resolver;

		private static object _resolverMutex = new object();

		/// <summary>
		/// Gets current resolver.
		/// <remarks>No need to make it public.</remarks>
		/// </summary>
		internal static IDependencyResolver Resolver
		{
			get { return _resolver; }
			set
			{
				lock( _resolverMutex )
				{
					_resolver = value;
				}
			}
		}

		/// <summary>
		/// Sets current dependency resolver. Call it at the begining of the application.
		/// </summary>
		/// <param name="resolver">The new resolver to set</param>
		public static void SetResolver( IDependencyResolver resolver )
		{
			Resolver = resolver;
		}
	}
}
