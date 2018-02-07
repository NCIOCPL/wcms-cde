using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using Newtonsoft.Json;

using CancerGov.ClinicalTrials.Basic.v2.Configuration;
using CancerGov.ClinicalTrials.Basic.v2.DataManagers;
using CancerGov.ClinicalTrials.Basic.v2.SnippetControls;
using CancerGov.ClinicalTrialsAPI;
using NCI.Core;
using NCI.Util;
using NCI.Web.CDE;
using NCI.Web.CDE.Application;
using NCI.Web.CDE.Modules;
using NCI.Web.CDE.UI.Configuration;
using NCI.Web;
using System.Net.Http;

namespace CancerGov.ClinicalTrials.Basic.v2.HttpHandlers
{
    public class CTSCachedPrintRequestHandler : IHttpHandler
    {

        /// <summary>
        /// Gets the Snippet Controls Config.
        /// </summary>
        private static BasicCTSPageInfo _config = null;

        static CTSCachedPrintRequestHandler()
        {
            //TODO: Validate this, and maybe cache it, and well, pull this out into something so it can be shared.
            //TODO: Add watcher for config.

            //////////////////////////////
            // Load the configuration XML from the App Settings
            _config = BasicCTSPageInfo.GetConfig();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the URL for the ClinicalTrials API from BasicClinicalTrialSearchAPISection:GetAPIUrl()
        /// </summary>
        protected string ApiUrl
        {
            get { return BasicClinicalTrialSearchAPISection.GetAPIUrl(); }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;

            CTSPrintManager manager = new CTSPrintManager(_config);

            if (request.HttpMethod == "POST")
            {
                GeneratePrintCacheAndRedirect(context, manager);
            } else if (request.HttpMethod == "GET")
            {
                ReturnPrintCache(context, manager);
            }
            else
            {
                ErrorPageDisplayer.RaisePageByCode(this.GetType().ToString(), 400);
            }
        }

        protected virtual string GetEmailUrl(Guid printID)
        {
            string popUpemailUrl = "";

            string title = "Clinical Trials Results";
            title = System.Web.HttpUtility.UrlEncode(Strings.StripHTMLTags(title.Replace("&#153;", "__tm;")));

            string emailUrl = "/CTS.Print/Display?PrintID=" + printID.ToString();
            string invokedFrom = "&invokedFrom=" + EmailPopupInvokedBy.ClinicalTrialPrintableSearchResults.ToString("d");

            popUpemailUrl = "/common/popUps/PopEmail.aspx?title=" + title + invokedFrom + "&docurl=" + System.Web.HttpUtility.UrlEncode(emailUrl.Replace("&", "__amp;")) + "&language=en";
            popUpemailUrl = popUpemailUrl + HashMaster.SaltedHashURL(HttpUtility.UrlDecode(title) + emailUrl);
            
            return popUpemailUrl;
        }

        private void ReturnPrintCache(HttpContext context, CTSPrintManager manager)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            //Set our output to be HTML
            response.ContentType = "text/HTML";
            response.ContentEncoding = Encoding.UTF8;

            Guid printID = new Guid();

            // Validate if the printID passed in through the URL is a valid Guid
            try
            {
                printID = Guid.Parse(request.QueryString["printid"]);
            }
            catch
            {
                // Incorrect parameter for printid (not guid)
                ErrorPageDisplayer.RaisePageByCode(this.GetType().ToString(), 400);
                throw new InvalidPrintIDException("Invalid PrintID parameter for CTS Print");
            }


            // If there is no error, send the printID to the manager to retrieve the cached print content
            string printContent = manager.GetPrintContent(printID);

            printContent = printContent.Replace("${generatePrintURL}", GetEmailUrl(printID));

            response.Write(printContent);
            response.End();

        }

        private void GeneratePrintCacheAndRedirect(HttpContext context, CTSPrintManager manager)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            NciUrl parsedReqUrlParams = new NciUrl(true, true, true);  //We need this to be lowercase and collapse duplicate params. (Or not use an NCI URL)
            parsedReqUrlParams.SetUrl(request.Url.Query);


            ClinicalTrialsAPIClient apiClient = APIClientHelper.GetV1ClientInstance();

            CTSSearchParams searchParams = null;
            try
            {
                // Get mapping file names from configuration
                TrialTermLookupConfig mappingConfig = new TrialTermLookupConfig();
                mappingConfig.MappingFiles.AddRange(_config.MappingFiles.Select(fp => HttpContext.Current.Server.MapPath(fp)));

                CTSSearchParamFactory factory = new CTSSearchParamFactory(new TrialTermLookupService(mappingConfig, apiClient), new ZipCodeGeoLookup());
                searchParams = factory.Create(parsedReqUrlParams);
            }
            catch (Exception ex)
            {
                ErrorPageDisplayer.RaisePageByCode(this.GetType().ToString(), 400); //Anything here is just a bad request.                
            }

            //Set our output to be JSON
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;

            //Try and get the request.
            Request req = null;
            try
            {
                req = GetRequestAndValidate(context);
            }
            catch (Exception)
            {

                ErrorPageDisplayer.RaisePageByCode(this.GetType().ToString(), 400); //Anything here is just a bad request.
                return;
            }

            // Store the cached print content
            Guid printCacheID = manager.StorePrintContent(req.TrialIDs, DateTime.Now, searchParams);

            //Add in debugging helper param to make debugging templates easier.
            //TODO: Refactor so get content and render is a single function.
            if (parsedReqUrlParams.QueryParameters.ContainsKey("debug") && parsedReqUrlParams.QueryParameters["debug"] == "true")
            {
                response.ContentType = "text/HTML";
                response.ContentEncoding = Encoding.UTF8;
                // If there is no error, send the printID to the manager to retrieve the cached print content
                string printContent = manager.GetPrintContent(printCacheID);

                printContent = printContent.Replace("${generatePrintURL}", GetEmailUrl(printCacheID));

                response.Write(printContent);
                response.End();
            }
            else
            {
                // Format our return as JSON
                var resp = JsonConvert.SerializeObject(new
                {
                    printID = printCacheID
                });

                response.Write(resp);
                response.End();
            }
        }

        /// <summary>
        /// Parses the request JSON into a request object.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Request GetRequestAndValidate(HttpContext context)
        {
            Request rtnReq = null;

            try
            {
                context.Request.InputStream.Position = 0;
                using (var inputStream = new StreamReader(context.Request.InputStream))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    rtnReq = (Request)serializer.Deserialize(inputStream, typeof(Request));
                }
            }
            catch (Exception)
            {
                throw new Exception("Could not parse request.");
            }

            return rtnReq;
        }

        /// <summary>
        /// Send an error response back to the client
        /// </summary>
        /// <param name="context">the request context</param>
        /// <param name="status">the status</param>
        private static void SendErrorResponse(HttpContext context, int status)
        {
            ErrorResponse res = new ErrorResponse() { Status = status };

            //context.Response.Write(JsonConvert.SerializeObject(res));
            //context.Response.StatusDescription = message;
            context.Response.StatusCode = status;
            //context.Response.TrySkipIisCustomErrors = true;
            context.Response.End();
        }

        /// <summary>
        /// Defines a response from the FeedbackService
        /// </summary>
        public class ErrorResponse
        {
            public int Status { get; set; }
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// Defines the expected Request object of the FeedbackService
        /// </summary>
        public class Request
        {
            /// <summary>
            /// Gets the list of TrialIDs
            /// </summary>
            public List<String> TrialIDs;
        }

        /// <summary>
        /// Defines a response from the FeedbackService
        /// </summary>
        /*public class Response
        {
            /// <summary>
            /// URL for Stored Results
            /// </summary>
            public String Url;
        }*/
    }
}

