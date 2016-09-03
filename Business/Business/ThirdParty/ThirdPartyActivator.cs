using System;
using System.Linq;
using Elmah;

namespace Core.Business.ThirdParty
{
    /// <summary>
    /// Activator for calling Start methods at initialization where programs can subscribe to, only occurs one
    /// </summary>
    public class ThirdPartyActivator
    {
        /// <summary>
        /// Calls the start method of third party tools that inherit from the interface IStartup when the website is starting up
        /// This event only occurs once.
        /// </summary>
        public void Start()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.IsDynamic == false))
            {
                foreach (var exportedType in assembly.ExportedTypes.Where(type => typeof(IStartup).IsAssignableFrom(type) && type.BaseType != null))
                {
                    var instance = Activator.CreateInstance(exportedType);
                    var method = exportedType.GetMethod("Start");
                    try
                    {
                        method.Invoke(instance, null);
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.GetDefault(null).Log(new Error(ex));
                    }
                }
            }
        }
    }
}


