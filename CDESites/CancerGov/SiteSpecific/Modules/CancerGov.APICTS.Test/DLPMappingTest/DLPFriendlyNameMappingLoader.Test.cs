﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CancerGov.ClinicalTrials.Basic.v2.Lookups;
using CancerGov.ClinicalTrials.Basic.v2.Test.ConfigTest;
using Xunit;
using Moq;
using System.Reflection;
using System.IO;

namespace CancerGov.ClinicalTrials.Basic.v2.Test.DLPMappingTest
{
    public class DLPFriendlyNameMappingLoader
    {
        static readonly string AssemblyFileName;
        static readonly string AssemblyPath;

        static DLPFriendlyNameMappingLoader()
        {
            AssemblyFileName = Assembly.GetExecutingAssembly().CodeBase;
            Uri fileNameURI = new Uri(AssemblyFileName);
            AssemblyPath = Path.GetDirectoryName(fileNameURI.LocalPath);
        }

        /// <summary>
        /// Loads the paths for the test mapping file(s) from MappingFiles folder.  
        /// </summary>
        /// <param name="mappingName">The name of the file to load.</param>
        /// <returns>The filepath(s) for the mapping files.</returns>
        public static string LoadMappingPath(string mappingName)
        {
            string path = Path.Combine(AssemblyPath, "MappingFiles", mappingName);
            return path;
        }

        /// <summary>
        /// Loads the lookup service given test mapping filepaths.  
        /// </summary>
        /// <param name="evsFilePath">The name of the EVS label mapping file to load.</param>
        /// <param name="overrideFilePath">The name of the EVS friendly name mapping file to load.</param>
        /// <param name="withOverrides">Whether the mapping file loads the EVS mappings or the override mappings.</param>
        /// <returns>A mapping lookup service.</returns>
        public DynamicTrialListingFriendlyNameMapper GetMappingService(string evsFilePath, string overrideFilePath, bool withOverrides)
        {
            return new DynamicTrialListingFriendlyNameMapper(LoadMappingPath(evsFilePath),
                                                         LoadMappingPath(overrideFilePath),
                                                         withOverrides);
        }

        /// <summary>
        /// Gets the Friendly Name to replace a c-code in the URL for the dynamic trial listing page. If there is no exact override for that c-code,
        /// attempt to find a match that contains the given c-code in the EVS mappings.
        /// Sets needsRedirect to true if there is a friendly name override found.
        /// </summary>
        /// <returns>A string with the friendly name for the URL (replaces c-code) if the override exists, otherwise the given c-codes</returns>
        protected KeyValuePair<string, bool> GetFriendlyNameForURL(DynamicTrialListingFriendlyNameMapper FriendlyNameMapping, DynamicTrialListingFriendlyNameMapper FriendlyNameWithOverridesMapping, string param)
        {
            bool needsRedirect = false;           

            if (FriendlyNameMapping.MappingContainsFriendlyName(param))
            {
                // If a friendly name is given, check to see if there is a friendly name override for the same code.
                // If there is a friendly name override for the same thing, return the friendly name override and set redirection bool
                string code = FriendlyNameMapping.GetCodeFromFriendlyName(param);

                if (FriendlyNameWithOverridesMapping.MappingContainsCode(code, true))
                {
                    needsRedirect = true;
                    return new KeyValuePair<string, bool>(FriendlyNameWithOverridesMapping.GetFriendlyNameFromCode(code, true), needsRedirect);
                }
            }

            if (FriendlyNameWithOverridesMapping.MappingContainsCode(param, true))
            {
                // If an exact match is found in the Friendly Name With Overrides mapping, return the friendly name and set redirection bool
                needsRedirect = true;
                return new KeyValuePair<string, bool>(FriendlyNameWithOverridesMapping.GetFriendlyNameFromCode(param, true), needsRedirect);
            }
            else
            {
                if (FriendlyNameMapping.MappingContainsCode(param, false))
                {
                    // If an exact match is found in the Friendly Name mapping (without overrides), return the friendly name and set redirection bool
                    // Also if matches are found that contain the given codes and all have the same friendly name, return that friendly name and set redirection bool
                    needsRedirect = true;
                    return new KeyValuePair<string, bool>(FriendlyNameMapping.GetFriendlyNameFromCode(param, false), needsRedirect);
                }
            }

            return new KeyValuePair<string, bool>(param, needsRedirect);
        }

        [Fact]
        public void LoadEVSFriendlyNameMapping()
        {
            DynamicTrialListingFriendlyNameMapper mapper = GetMappingService("DLPFriendlyNameMapping.txt", "DLPFriendlyNameOverrideMapping.txt", false);

            Assert.Equal("thyroid-gland-cancer", mapper.GetFriendlyNameFromCode("c118827,c4815", true), new MappingsComparer());
        }

        [Fact]
        public void LoadOverrideFriendlyNameMapping()
        {
            DynamicTrialListingFriendlyNameMapper mapper = GetMappingService("DLPFriendlyNameMapping.txt", "DLPFriendlyNameOverrideMapping.txt", true);

            Assert.Equal("thyroid-cancer", mapper.GetFriendlyNameFromCode("c118827,c4815", true), new MappingsComparer());
        }

        [Fact]
        public void EVSMappingContainsCodes()
        {
            DynamicTrialListingFriendlyNameMapper mapper = GetMappingService("DLPFriendlyNameMapping.txt", "DLPFriendlyNameOverrideMapping.txt", false);

            Assert.Equal(true, mapper.MappingContainsCode("c4872", false));
            Assert.Equal(true, mapper.MappingContainsCode("c118827,c4815", false));
            Assert.Equal(true, mapper.MappingContainsCode("c139569", false));
        }

        [Fact]
        public void OverrideMappingContainsCodes()
        {
            DynamicTrialListingFriendlyNameMapper mapper = GetMappingService("DLPFriendlyNameMapping.txt", "DLPFriendlyNameOverrideMapping.txt", true);

            Assert.Equal(false, mapper.MappingContainsCode("c4872", true));
            Assert.Equal(false, mapper.MappingContainsCode("c118827", true));
            Assert.Equal(true, mapper.MappingContainsCode("c118827,c4815", true));
        }
    }
}
