using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;


using NCI.Web.CDE;
using NCI.Search;

using Common.Logging;

namespace CancerGov.Search.AutoSuggest
{
    /// <summary>
    /// This class is the user interface and provides the connection between 
    /// the WCF service and the business layer. A ServiceContract decoration is 
    /// required in order for it to be used with the WCF
    /// </summary>
    [ServiceContract]
    public class AutoSuggestSearchService
    {
        static ILog log = LogManager.GetLogger(typeof(AutoSuggestSearchService));

        /// <summary>
        /// Method to interface through the WCF do get results from an API query
        /// This will return the data in JSON format
        /// </summary>
        /// <param name="language">The language used to query the API</param>
        /// <param name="criteria">The partial text used to query the API</param>
        /// <returns>Returns the search results</returns>
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "SearchJSON/{language}?term={criteria}")]
        [OperationContract]
        public AutoSuggestSearchServiceCollection SearchJSON(string language, string criteria)
        {
            // The below values not used by the system. Retained so it can used in the future for 
            // customization.
            int maxRows=0;
            bool contains = true;

            return Search(language, criteria, maxRows, contains);
        }

        /// <summary>
        /// Method to interface through the WCF do get results from a database query.
        /// This will return the data in XML format
        /// </summary>
        /// <param name="language">The language used to query the API</param>
        /// <param name="criteria">The partial text used to query the API</param>
        /// <returns>Returns the search results</returns>
        [WebGet(ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "SearchXML/{language}?term={criteria}")]
        [OperationContract]
        public AutoSuggestSearchServiceCollection SearchXML(string language, string criteria)
        {
            // The below values not used by the system. Retained so it can used in the future for 
            // customization.
            int maxRows = 0;
            bool contains = true;

            return Search(language, criteria, maxRows, contains);
        }

        /// <summary>
        /// Method used to query API for results.
        /// </summary>
        /// <param name="language">The language used to query the API</param>
        /// <param name="criteria">The partial text used to query the API</param>
        /// <param name="size">The maximum number of items that the API will return</param>
        /// <param name="contains">Indicator on whether the text is to be search from the beginning of the text or anywhere in the string</param>
        /// <returns>Returns the search results</returns>
        private AutoSuggestSearchServiceCollection Search(string language, string criteria, int size, bool contains)
        {
            // Create the collection variable
            AutoSuggestSearchServiceCollection sc = new AutoSuggestSearchServiceCollection();

            // Language converted to an enum
            DisplayLanguage displayLanguage = (DisplayLanguage)Enum.Parse(typeof(DisplayLanguage), language);

            try
            {
                // Pass the given API parameters to the business layer
                AutoSuggestAPIResultCollection apiCollection = AutoSuggestSearchManager.Search(displayLanguage, criteria, size, contains);

                // Use Linq to extract the data from the business layer and create the service data objects
                // TermID is 0 always, that value is not part of the result received from the API call.
                //But can be used in the future for other purposes.
                var collection = apiCollection.Results.Select(r => new AutoSuggestSearchServiceItem(
                    0,
                    r.Term,
                    string.Empty
                    ));

                sc.AddRange(collection);
            }
            catch (Exception ex)
            {
                // Log the error that occured
                log.Error("Error in AutoSuggestSearchService", ex);
            }

            return sc;
        }
    }
}
