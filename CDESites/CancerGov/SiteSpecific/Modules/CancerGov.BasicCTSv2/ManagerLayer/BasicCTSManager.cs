using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CancerGov.ClinicalTrialsAPI;
using Newtonsoft.Json.Linq;
using MoreLinq;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public class BasicCTSManager
    {
        /// <summary>
        /// Gets the API client this instance of a BasicCTSManager will use
        /// </summary>
        private IClinicalTrialsAPIClient Client { get; set; }



        /// <summary>
        /// Creates a new instance of a BasicCTSManager
        /// </summary>
        /// <param name="host">The hostname of the API</param>        
        [Obsolete("This will need to be retired before SDS")]
        public BasicCTSManager(string host)
            : this(APIClientHelper.GetV1ClientInstance()) { }

        /// <summary>
        /// Creates a new instance of a BasicCTSManager
        /// </summary>
        /// <param name="client"></param>
        public BasicCTSManager(IClinicalTrialsAPIClient client)
        {
            this.Client = client;
        }

        #region Public Methods

        /// <summary>
        /// Returns a Clinical Trial
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ClinicalTrial Get(string id)
        {
            ClinicalTrial trial = Client.Get(id);
            RemoveNonRecruitingSites(trial);
            return trial;
        }

        /// <summary>
        /// Gets a selection of trials based on a query
        /// </summary>
        /// <param name="query">The Query without paging or include/exclude fields</param>
        /// <param name="pageNumber">The current page number</param>
        /// <param name="itemsPerPage">Items per page</param>
        /// <param name="includeFields">A list of fields to include (DEFAULT: Return all fields)</param>
        /// <returns></returns>
        public ClinicalTrialsCollection GetClinicalTrials(JObject query, int pageNumber = 0, int itemsPerPage = 10, string[] includeFields = null )
        {
            int from = GetPageOffset(pageNumber, itemsPerPage);

            ClinicalTrialsCollection rtnResults = new ClinicalTrialsCollection();

            //Fetch results
            rtnResults = Client.List(
                searchParams: query,
                size: itemsPerPage,
                from: from,
                includeFields: includeFields
            );

            //Remove all the inactive sites from all the trials.
            foreach (ClinicalTrial trial in rtnResults.Trials)
            {
                RemoveNonRecruitingSites(trial);
            }

            return rtnResults;
        }

        /// <summary>
        /// Performs a search against the Clinical Trials API
        /// </summary>
        /// <param name="searchParams">Search paramesters</param>
        /// <param name="dynamicFilterParams">Deserialized dynamic search parameters</param>
        /// <returns>Clinical Trials collection</returns>
        public ClinicalTrialsCollection Search(CTSSearchParams searchParams, int pageNumber = 0, int itemsPerPage = 10)
        {
            int from = GetPageOffset(pageNumber, itemsPerPage);

            Dictionary<string, object> filterCriteria = searchParams.ToFilterCriteria();

            //Get our list of trials from the API client
            ClinicalTrialsCollection rtnResults = new ClinicalTrialsCollection();

            //Fetch results
            rtnResults = Client.List(
                searchParams: filterCriteria,
                size: itemsPerPage,
                from: from,
                includeFields: CTSConstants.IncludeFields
            );

            //Remove all the inactive sites from all the trials.
            foreach (ClinicalTrial trial in rtnResults.Trials)
            {
                RemoveNonRecruitingSites(trial);
            }

            return rtnResults;
        }

        /// <summary>
        /// Gets the page offset.
        /// </summary>
        /// <param name="pageNumber">The page number of the results</param>
        /// <param name="itemsPerPage">The number of items per page</param>
        /// <returns></returns>
        private int GetPageOffset(int pageNumber, int itemsPerPage)
        {
            int from = 0;

            if (pageNumber > 1)
            {
                from = (pageNumber - 1) * itemsPerPage;
            }

            return from;
        }

        /// <summary>
        /// Gets the cancer type display name the user selected
        /// </summary>
        /// <param name="cancertypeids">The IDs of the term</param>
        /// <param name="key">The term key of the unique menu name</param>
        /// <returns></returns>
        [Obsolete("With the new SDS release this will be obsolete.")]
        public string GetCancerTypeDisplayName(string[] cancertypeids, string key)
        {
            string displayName = string.Empty;

            if (!String.IsNullOrWhiteSpace(key))
            {
                Term term = Client.GetTerm(key);
                if (term == null)
                {
                    return GetCancerTypeDisplayName(cancertypeids, null);
                }
                else
                {
                    return term.DisplayText;
                }
            }
            else
            {
                //If we did not have a key, OR we did not find an entry
                //in either case we need to try and find it by ID.
                Dictionary<string, object> filterCriteria = new Dictionary<string, object>();

                //If we have a key, then look it up by key
                filterCriteria.Add("codes", cancertypeids);

                TermCollection rtnResults = Client.Terms(
                    size: 100,
                    from: 0,
                    searchParams: filterCriteria
                );

                if (rtnResults.TotalResults > 0)
                {
                    //This is hacky!!
                    //TODO: Clean this up and check for errors
                    //TODO: Some terms may have 2 IDs, this query will return anything with either term,
                    //not just the terms with both.  We should iterate through the results and find the first
                    //with both.
                    return rtnResults.Terms[0].DisplayText;
                }

            }

            return string.Empty; //Nothing found
        }

        /// <summary>
        /// Gets a collection of trials based on trial ids, batchVal at a time.
        /// </summary>
        /// <param name="ids">An array of trial IDs to fetch</param>
        /// <param name="batchVal">The number of trials to retrieve at a time</param>
        /// <returns>An enumerable list of ClinicalTrial objects</returns>
        public IEnumerable<ClinicalTrial> GetMultipleTrials(List<String> ids, int batchVal = 5)
        {
            foreach (IEnumerable<string> batch in ids.Batch(batchVal))
            {
                Dictionary<string, object> filterCriteria = new Dictionary<string, object>();
                filterCriteria.Add("nci_id", batch.ToArray());
                ClinicalTrialsCollection ctColl = new ClinicalTrialsCollection();

                ctColl = Client.List(
                    size: 100,
                    //from: 0,
                    searchParams: filterCriteria
                );

                foreach (ClinicalTrial c in ctColl.Trials)                
                {
                    //Remove all the inactive sites from the trial.
                    RemoveNonRecruitingSites(c);
                    yield return c;

                }
            }            
        }


        

        #endregion

        #region Private Members

        /// <summary>
        /// Creates a list of actively recruiting sites only 
        /// </summary>
        /// <param name="trial">Clinical trial</param>
        private void RemoveNonRecruitingSites(ClinicalTrial trial)
        {
            if (trial.Sites != null)
            {
                trial.Sites = new List<ClinicalTrial.StudySite>(trial.Sites.Where(site => IsActivelyRecruiting(site)));
            }
        }

        /// <summary>
        /// Set to true if site status matches an item in ActiveRecruitmentStatuses
        /// </summary>
        /// <param name="site">Study site</param>
        private bool IsActivelyRecruiting(ClinicalTrial.StudySite site)
        {
            return CTSConstants.ActiveRecruitmentStatuses.Any(status => status.ToLower() == site.RecruitmentStatus.ToLower());
        }

        #endregion

        


    }
}
