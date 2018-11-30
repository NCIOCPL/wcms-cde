using System;
using System.Web;
using Common.Logging;
using NCI.Web.CDE.PreLoadRedirector.Configuration;
using NCI.Web.CDE.SimpleRedirector;

namespace NCI.Web.CDE
{
    public class PreLoadRedirect : IHttpModule
    {
        static ILog log = LogManager.GetLogger(typeof(SimpleRedirect));

        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
        }

        #endregion

        /// <summary>
        /// The guts of the HTTP module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;

            // Get absolute path of the request URL as pretty URL
            String url = context.Server.UrlDecode(context.Request.Url.AbsolutePath);

            // Can we ignore this file?
            if (Utility.IgnoreWebResource(url))
                return;

            // If the PageAssemblyInstruction is not null, then a page is being served
            // and we can ignore this request.
            if (PageAssemblyContext.Current.PageAssemblyInstruction != null)
                return;


            String redirectUrl = GetRedirectUrl(url, context);
            //check if url if not return
            if (String.IsNullOrEmpty(redirectUrl))
                return;


            String query = context.Server.UrlDecode(context.Request.Url.Query);
            if (!String.IsNullOrEmpty(query))
            {
                // Strip ? from query parameters to add them to redirectUrl query params
                if(redirectUrl.Contains("?"))
                {
                    query = query.Replace("?", "&");
                }

                // Add query parameters and redirect parameter for analytics (if not previously redirected)
                redirectUrl += query + (!query.Contains("redirect=true") ? "&redirect=true" : String.Empty);
            }
            else
            {
                if(!redirectUrl.Contains("redirect=true"))
                {
                    // Add redirect parameter for analytics (if not previously redirected)
                    redirectUrl += redirectUrl.Contains("?") ? "&redirect=true" : "?redirect=true";
                }
            }

            NCI.Web.CDE.Application.PermanentRedirector.DoPermanentRedirect(context.Response, redirectUrl, "Redirect Map");
        }

        /// <summary>
        /// Private helper to find the redirection URL mapped to a given input URL.
        /// </summary>
        /// <param name="url">The URL to examine for a replacement</param>
        /// <param name="context"></param>
        /// <returns></returns>
        private String GetRedirectUrl(String url, HttpContext context)
        {
            String redirect = null;
            string urlWithSlash;

            if (url.LastIndexOf("/") == url.Length - 1)
            {
                urlWithSlash = url;
                url = url.Substring(0, url.Length - 1);
            }
            else
            {
                urlWithSlash = url + "/";
            }

            PreLoadRedirectorConfigurationSection config = PreLoadRedirectorConfigurationSection.Get();

            String datafile = HttpContext.Current.Server.MapPath(config.DataSource.DataFile);
            RedirectionMap urlMap = RedirectionMap.GetMap(datafile, context);

            if (urlMap.Contains(url))
            {
                if (urlMap.ContainsMultiple(url))
                {
                    log.DebugFormat("Url: '{0}' has multiple instances in redirect map.", url);
                }
                redirect = urlMap[url];
                log.DebugFormat("Url '{0}' found; redirects to '{1}'.", url, redirect);
            }
            else if (urlMap.Contains(urlWithSlash))
            {
                if (urlMap.ContainsMultiple(urlWithSlash))
                {
                    log.DebugFormat("Url: '{0}' has multiple instances in redirect map.", urlWithSlash);
                }
                redirect = urlMap[urlWithSlash];
                log.DebugFormat("Url '{0}' found; redirects to '{1}'.", urlWithSlash, redirect);
            }
            else
            {
                log.DebugFormat("No match found for url '{0}.", url);
            }
            return redirect;
        }

    }
}
