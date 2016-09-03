using System;
using System.Linq;
using System.Web;
using Core.Business.Plugin;
using Web.Infrastructure.Stream;

namespace Web.Plugin
{
    /// <summary>
    /// Http module for page events
    /// Triggers events where plugins can subscribe to
    /// </summary>
    public class HttpPluginModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PostReleaseRequestState += context_PostReleaseRequestState;
        }

        public void Dispose()
        {
           // ignore
        }

        void context_PostReleaseRequestState(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            if (context.Response.ContentType == "text/html")
            {
                var pluginFilter = new ResponseFilterStream(context.Response.Filter, context);
                pluginFilter.TransformString += Transform;
                context.Response.Filter = pluginFilter;
            }
        }

        private string Transform(string response)
        {
            response = TransformHead(response);
            response = TransformBody(response);
            response = TransformFooter(response);

            return response;
        }

        /// <summary>
        /// Writes text before closing the body tag
        /// </summary>
        /// <param name="response"></param>
        private string TransformFooter(string response)
        {
            int indexHtmlTag = response.IndexOf("</body>", StringComparison.Ordinal);

            if (indexHtmlTag != -1)
            {
                response = PluginFilterActivateService.FireHook(PluginFilters.Footer).Aggregate(response, (current, filterResponse) => current.Insert(indexHtmlTag, filterResponse));
            }

            return response;
        }

        /// <summary>
        /// Writes text before closing the head tag
        /// </summary>
        /// <param name="response"></param>
        private string TransformHead(string response)
        {
            int indexHtmlTag = response.IndexOf("</head>", StringComparison.Ordinal);

            if (indexHtmlTag != -1)
            {
                response = PluginFilterActivateService.FireHook(PluginFilters.Head).Aggregate(response, (current, filterResponse) => current.Insert(indexHtmlTag, filterResponse));
            }

            return response;
        }

        /// <summary>
        /// Writes text after opening the body tag
        /// </summary>
        /// <param name="response"></param>
        private string TransformBody(string response)
        {
            const string bodyTag = "<body>";
            int indexHtmlTag = response.IndexOf(bodyTag, StringComparison.Ordinal);

            if (indexHtmlTag != -1)
            {
                response = PluginFilterActivateService.FireHook(PluginFilters.Body).Aggregate(response, (current, filterResponse) => current.Insert(indexHtmlTag + bodyTag.Length + 1, filterResponse));
            }

            return response;
        }
    }
}
