using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Web.SessionState;
using elFinder.Connector.Service;
using Web.Account;

namespace elFinder.Connector
{
    public class Connector : IHttpHandler, IReadOnlySessionState
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (LoginHelper.IsLoggedIn())
            {
                var accountService = new Core.Business.Account.AccountService();

                if (accountService.IsUserAdmin(LoginHelper.GetUserId()))
                {
                    var resolver = DependencyResolver.Resolver;
                    using (resolver.BeginResolverScope())
                    {
                        var parameters = getParameters(context.Request);
                        string cmd = parameters["cmd"];
                        if (string.IsNullOrWhiteSpace(cmd))
                        {
                            sendError(context, "Command not set");
                            return;
                        }
                        var foundCmd = resolver.Resolve<IEnumerable<Command.ICommand>>().FirstOrDefault(x => x.Name.Equals(cmd, StringComparison.OrdinalIgnoreCase));
                        if (foundCmd == null)
                        {
                            sendError(context, "errUnknownCmd");
                            return;
                        }
                        var args = new Command.CommandArgs(parameters, context.Request.Files);
                        var cmdResponse = foundCmd.Execute(args);
                        cmdResponse.Process(context.Response);
                        return;
                    }
                }
            }
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }

        #endregion

        private NameValueCollection getParameters(HttpRequest request)
        {
            return request.QueryString.Count > 0 ? request.QueryString : request.Form;
        }

        private void sendError(HttpContext context, string errorMsg)
        {
            new Response.ErrorResponse(errorMsg).Process(context.Response);
        }
    }
}
