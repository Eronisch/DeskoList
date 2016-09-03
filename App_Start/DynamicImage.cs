using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using SoundInTheory.DynamicImage;
using Topsite;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(DynamicImage), "PreStart")]

namespace Topsite
{
	public static class DynamicImage
	{
		public static void PreStart()
		{
			DynamicModuleUtility.RegisterModule(typeof(DynamicImageModule));
		}
	}
}