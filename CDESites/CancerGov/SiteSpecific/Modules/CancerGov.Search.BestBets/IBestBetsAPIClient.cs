using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CancerGov.Search.BestBets
{
    public interface IBestBetsAPIClient
    {
        /// <summary>
        /// Calls the BestBets endpoint (/BestBets) of the best bets API
        /// </summary>
        /// <param name="collection">Collection name (required)</param>
        /// <param name="language">Language to use (required)</param>
        /// <param name="searchParams">Search term (required)</param>
        /// <returns>A Best Bet result</returns>
        BestBetAPIResult[] Search(
            string collection,
            string language,
            string searchTerm
            );
    }
}