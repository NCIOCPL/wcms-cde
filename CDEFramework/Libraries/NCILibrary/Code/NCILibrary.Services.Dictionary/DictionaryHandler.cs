﻿using System;
using System.Web;
using Common.Logging;
using NCI.Services.Dictionary.BusinessObjects;
using NCI.Services.Dictionary.Handler;
using NCI.Util;

namespace NCI.Services.Dictionary
{
    /// <summary>
    /// Cheesy HTTP handler to make up for WCF not giving us an easy way to remove the quotation marks from
    /// around the string containing the term details.
    /// 
    /// To configure this handler, go to the web.config and in the system.webServer / handlers section, add the new line:
    /// 
    ///     <add name="DictionaryServiceHandler" verb="GET" path="Dictionary.svc" type ="NCI.Services.Dictionary.DictionaryHandler, NCILibrary.Services.Dictionary"/>
    /// 
    /// In the Risk Assessment Tools, in the same section, be sure to add a matching
    /// 
    ///     <remove name="DictionaryServiceHandler" />
    /// 
    /// </summary>
    public class DictionaryHandler : IHttpHandler
    {
        static ILog log = Common.Logging.LogManager.GetLogger(typeof(DictionaryHandler));
        static string errorProcessFormat = "Error processing dictionary request. Query: {0}";
        static string errorVersionFormat = "Unknown version '{0}'.";
        static string errorMethodFormat = "Unknown method '{0}'.";
 
        #region IHttpHandler Members

        /// <summary>
        /// Signals to the ASP.Net system that this object can be resused. (Default implementation.)
        /// This class has no stateful member variables.
        /// </summary>
        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        /// <summary>
        /// Entry point for handling the HTTP request.  ProcessRequest manages the process of
        /// determining which method was requested, invoking it, and returning a JSON response.
        /// Part of IHttpHandler
        /// </summary>
        /// <param name="context">Object containing the details of current HTTP request and response.</param>
        public void ProcessRequest(HttpContext context)
        {
                HttpRequest request = context.Request;
                HttpResponse response = context.Response;

                try
                {
                    // Get the particular method being invoked.
                    ApiMethodType method = ParseApiMethod(request);

                    // Get object for invoking the specific dictionary method.
                    Invoker invoker = Invoker.Create(method, request);

                    // Invoke the requested dictionary method.
                    IJsonizable result = invoker.Invoke();

                    // Put together the JSON response.
                    Jsonizer json = new Jsonizer(result);

                    response.ContentType = "application/json";
                    response.Write(json.ToJsonString());
                }
                // There was something wrong with the request that prevented us from
                // being able to invoke a method.
                catch (HttpParseException ex)
                {
                    response.Status = ex.Message;
                    response.StatusCode = 400;
                }
                // Something went wrong in our code.
                catch (Exception ex)
                {
                    log.ErrorFormat(errorProcessFormat, ex, request.RawUrl);
                    response.StatusDescription = "Error processing dictionary request.";
                    response.StatusCode = 500;
                }
        }

        #endregion

        /// <summary>
        /// Parse the inovked "service" path to determine which method is meant to
        /// be invoked.
        /// </summary>
        /// <param name="request">The current HTTP Request object.</param>
        /// <returns>An ApiMethodType method denoting the invoked web method.</returns>
        /// <remarks>Throws HttpParseException if an invalid path is supplied.</remarks>
        private ApiMethodType ParseApiMethod(HttpRequest request)
        {
            ApiMethodType method = ApiMethodType.Unknown;

            // Get the particular method being invoked by parsing context.Request.PathInfo
            if(string.IsNullOrEmpty(request.PathInfo))
                throw new HttpParseException("Request.Pathinfo is empty.");

            String[] path = Strings.ToListOfTrimmedStrings(request.PathInfo, '/');

            // path[0] -- version
            // path[1] -- Method
            if (path.Length != 2) throw new HttpParseException("Unknown path format.");

            // Only version 1 is presently supported.
            if (!string.Equals(path[0], "v1", StringComparison.CurrentCultureIgnoreCase))
            {
                String msg = String.Format(errorVersionFormat, path[0]);
                log.Error(msg);
                throw new HttpParseException(msg);
            }

            // Attempt to retrieve the desired method.
            method = ConvertEnum <ApiMethodType>.Convert(path[1], ApiMethodType.Unknown);
            if (method == ApiMethodType.Unknown)
            {
                String msg = String.Format(errorMethodFormat, path[1]);
                log.Error(msg);
                throw new HttpParseException(msg);
            }

            return method;
        }
    }
}
