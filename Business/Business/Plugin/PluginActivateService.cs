using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Core.Models;
using Elmah;

namespace Core.Business.Plugin
{
    /// <summary>
    /// Manager for firing various plugin actions
    /// </summary>
    /// <typeparam name="TPluginEnum"></typeparam>
    public class PluginActivateService<TPluginEnum>
    {
        private readonly PluginStorageService<TPluginEnum> _pluginStorageService;

        public PluginActivateService(PluginStorageService<TPluginEnum> pluginStorageService)
        {
            ThrowExceptionIfNotEnum();

            _pluginStorageService = pluginStorageService;
        }

        /// <summary>
        /// Fire a hook
        /// A hook gets fired on various actions like adding a website, user registration etc
        /// With a hook you can add code to the event and prevent further process
        /// </summary>
        /// <param name="pluginHook"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public ResultModel FireHook(TPluginEnum pluginHook, params object[] values)
        {
            foreach (var hook in _pluginStorageService.GetHooks(pluginHook))

                try
                {
                    var instance = Activator.CreateInstance(hook.Type);

                    MethodInfo method = hook.Type.GetMethod(hook.Method);

                    ResultModel result;

                    if (method.GetParameters().Length == 0)
                    {
                        result = (ResultModel)method.Invoke(instance, null);
                    }
                    else
                    {
                        result = (ResultModel)method.Invoke(instance, values.Take(method.GetParameters().Length).ToArray());
                    }

                    if (!result.IsSuccess)
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.GetDefault(null).Log(new Error(ex));
                }

            return new ResultModel();
        }

        /// <summary>
        /// Fire an action
        /// A hook gets fired on various filter actions
        /// These are the normal filter actions from asp.net mvc (OnActionExecuting, OnActionExecuted etc..)
        /// </summary>
        /// <param name="pluginHook"></param>
        /// <param name="values"></param>
        public void FireAction(TPluginEnum pluginHook, params object[] values)
        {
            foreach (var hook in _pluginStorageService.GetHooks(pluginHook))
            {
                try
                {
                    var instance = Activator.CreateInstance(hook.Type);

                    MethodInfo method = hook.Type.GetMethod(hook.Method);

                    method.Invoke(instance, method.GetParameters().Length == 0 ? null : values);
                }
                catch (Exception ex)
                {
                    ErrorLog.GetDefault(null).Log(new Error(ex));
                }
            }
        }

        /// <summary>
        /// Fire a filter
        /// A filter is fired when various parts of the page are loaded
        /// You can add html to the head, body and footer
        /// </summary>
        /// <param name="pluginHook"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<string> FireFilter(TPluginEnum pluginHook, params object[] values)
        {
            Collection<string> filterResults = new Collection<string>();

            foreach (var hook in _pluginStorageService.GetHooks(pluginHook))
            {
                try
                {
                    var instance = Activator.CreateInstance(hook.Type);

                    MethodInfo method = hook.Type.GetMethod(hook.Method);

                    filterResults.Add(((string)method.Invoke(instance, method.GetParameters().Length == 0 ? null : values)));
                }
                catch (Exception ex)
                {
                    ErrorLog.GetDefault(null).Log(new Error(ex));
                }
            }

            return filterResults;
        }

        private void ThrowExceptionIfNotEnum()
        {
            if (!typeof(TPluginEnum).IsEnum)
            {
                throw new Exception("The available plugins must be an enum!");
            }
        }
    }
}
