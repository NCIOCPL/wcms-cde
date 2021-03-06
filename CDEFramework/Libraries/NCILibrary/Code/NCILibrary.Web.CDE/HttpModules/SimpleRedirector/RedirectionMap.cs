﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using Common.Logging;
using NCI.Web.CDE.SimpleRedirector.Configuration;

namespace NCI.Web.CDE.SimpleRedirector
{
    /// <summary>
    /// Maps a collection of old URLs to new URLs. All lookups ignore case as well as
    /// any leading or trailing spaces.
    /// </summary>
    internal class RedirectionMap
    {
        static ILog log = LogManager.GetLogger(typeof(RedirectionMap));

        /// <summary>
        /// Returns a RedirectionMap created from the specified datafile.  If avaialble, a cached
        /// copy of the map is returned; otherwise, the data file is parsed and a new map created.
        /// </summary>
        /// <param name="datafile">Location of a data file creating comma-separated pairs of
        /// old URLs and new URLs for use as redirection targets.</param>
        /// <param name="context">The current HTTP Context object.</param>
        /// <returns>A (possibly empty) RedirectionMap.</returns>
        public static RedirectionMap GetMap(String datafile, HttpContext context)
        {
            log.Trace("Enter GetMap().");

            RedirectionMap map;
            Cache cache = context.Cache;

            Object lockObj = new Object();

            try
            {
                log.DebugFormat("Load cache for '{0}'.", datafile);
                map = (RedirectionMap)cache[datafile];
                if (map == null)
                {
                    lock (lockObj)
                    {
                        // Check whether the cache was loaded while we waited for the lock.
                        map = (RedirectionMap)cache[datafile];
                        if (map == null)
                        {
                            // There was no cached redirection map.  Load it from the file system.
                            log.DebugFormat("Cache miss. Loading redirection map from '{0}'.", datafile);

                            CacheItemRemovedCallback onRemove = new CacheItemRemovedCallback(RemovedItemCallback);
                            CacheDependency fileDependency = new CacheDependency(datafile);

                            map = LoadMapFromFile(datafile);
                            cache.Add(datafile, map, fileDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, onRemove);
                        }
                        else
                        {
                            log.Debug("Cached redirection map found on second chance retrieval.");
                        }
                    }
                }
                else
                {
                    // A cached redirection map was found. Return it.
                    log.Debug("Loading cached redirection map.");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while getting the redirection map.", ex);

                // Instead of letting the request die,
                // swallow the exception and return an empty map.
                map = new RedirectionMap();
            }

            return map;
        }

        /// <summary>
        /// Private helper method to respond when a cached item is removed.
        /// </summary>
        /// <param name="key">The cache key for the item.</param>
        /// <param name="item">The actual object which is being dropped from the cache.</param>
        /// <param name="reason">Enum explaining why the item was removed.</param>
        private static void RemovedItemCallback(String key, Object item, CacheItemRemovedReason reason)
        {
            log.TraceFormat("'{0}' removed from cache because '{1}'.", key, reason);
        }

        /// <summary>
        /// Private helper method to encapsulate the logic for loading and parsing the datafile.
        /// </summary>
        /// <param name="datafile">Location of a data file creating comma-separated pairs of
        /// old URLs and new URLs for use as redirection targets.</param>
        /// <returns></returns>
        private static RedirectionMap LoadMapFromFile(String datafile)
        {
            log.Trace("Enter LoadMapFromFile().");

            SimpleRedirectorConfigurationSection config = SimpleRedirectorConfigurationSection.Get();

            char[] separators = new char[1];
            separators[0] = config.DataSource.Separator;

            RedirectionMap map = new RedirectionMap();

            try
            {
                if (!File.Exists(datafile))
                {
                    log.ErrorFormat("Datafile '{0}' not found.", datafile);
                    throw new FileNotFoundException(datafile);
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error while loading urls from {0}.", ex, datafile);
                // Swallow the exception.  The worst case is we return an empty dictionary
                // and nothing gets redirected.
            }
                
            String[] listOfUrlPairs = File.ReadAllLines(datafile);
            foreach (String urlPair in listOfUrlPairs)
            {
                String[] urls = urlPair.Trim().Split(separators);
                if (urls.Length >= 2)
                    try
                    {
                        map.Add(urls[0], urls[1]);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("Duplicate URL found in RedirectMap: {0}", ex, urls[0]);
                    }
                        
                if (urls.Length != 2)
                {
                    // We can recover from this problem. No exception needed.
                    log.WarnFormat("Expected only two urls, found {0} in '{1}'.", urls.Length, urlPair);
                }
            }

            return map;
        }


        // The RedirectionMap is mainly a thin wrapper around Dictionary<string, string>.
        // These are the Dictionary methods we expose.
        private Dictionary<string, string> map;

        public RedirectionMap()
        {
            map = new Dictionary<string, string>();
        }

        public void Add(String oldUrl, String newUrl)
        {
            oldUrl = oldUrl.Trim().ToLowerInvariant();
            newUrl = newUrl.Trim().ToLowerInvariant();
            map.Add(oldUrl, newUrl);
        }

        public bool Contains(String url)
        {
            url = url.Trim().ToLowerInvariant();
            return map.ContainsKey(url);
        }

        public bool ContainsMultiple(String url)
        {
            url = url.Trim().ToLowerInvariant();
            if(map.ContainsKey(url + "/"))
            {
                    return true;
            }
            
            return false;
        }

        public String this[String url]
        {
            get
            {
                url = url.Trim().ToLowerInvariant();
                return map[url];
            }
        }
    }
}
