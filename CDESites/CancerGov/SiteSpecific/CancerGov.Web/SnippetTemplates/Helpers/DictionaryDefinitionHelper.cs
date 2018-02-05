using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NCI.Web.CDE;
using NCI.Web.CDE.UI;
using NCI.Web.CDE.WebAnalytics;
using NCI.Web.Dictionary;
using NCI.Web.Dictionary.BusinessObjects;


namespace CancerGov.Web.SnippetTemplates.Helpers
{
 /// <summary>
 ///    Helper class used to perform ad hoc operations that are common across different Dictionary Definitions (i.e. Genetics Term Dictionary Definition View,
 ///    Term Dicitonary Definition View, etc)
 /// </summary>
    public class DictionaryDefinitionHelper
    {

        public DictionaryDefinitionHelper()
        {

        }






        /// <summary>
        ///    Sets the meta tag description. The function checks if the Dictionary Term has a valid (not null and length greater than 0) definition.
        ///    If it is the case the function attempts to extract the first two sentences of the Definition and set them as the meta tag description.
        ///    If not, we revert to using the term itself has the meta tag description.
        ///    
        ///    AUTHOR: CHRISTIAN RIKONG
        ///    LAST PUBLISHED DATE: 12/08/2017 11:47 AM
        /// </summary>
        /// <param name="dataItem">Stores the Dictionary Term that is used to create the description meta tag</param>
        public static void SetMetaTagDescription(DictionaryTerm dataItem, string DictionaryLanguage, IPageAssemblyInstruction PageInstruction)
        {

            string termName = dataItem.Term;


            if (dataItem.Definition != null && dataItem.Definition.Text != null && dataItem.Definition.Text.Length > 0)
            {
                string sentences = "";
                string[] definitionsSentences = System.Text.RegularExpressions.Regex.Split(dataItem.Definition.Text, @"(?<=[\.!\?])\s+");


                if (definitionsSentences != null && definitionsSentences.Length > 0)
                {
                    int sentencesCount = 0;

                    foreach (string existingSentence in definitionsSentences)
                    {
                        sentencesCount = sentencesCount + 1;

                        if (sentencesCount <= 2)
                        {
                            sentences = sentences + existingSentence.Replace(".", "") + ". ";
                        }
                        else
                        {
                            break;
                        }
                    }

                    PageInstruction.AddFieldFilter("meta_description", (name, data) =>
                    {
                        data.Value = sentences;
                    });
                }
                else
                {
                    SetMetaTagDescriptionToTerm(PageInstruction, termName, DictionaryLanguage);
                }

            }
            else
            {
                SetMetaTagDescriptionToTerm(PageInstruction, termName, DictionaryLanguage);
            }

        }






        /// <summary>
        ///    Sets the Meta Tag Description to a given term
        /// </summary>
        /// <param name="PageInstruction"></param>
        /// <param name="termName"></param>
        /// <param name="DictionaryLanguage"></param>
        private static void SetMetaTagDescriptionToTerm(IPageAssemblyInstruction PageInstruction, string termName, string DictionaryLanguage)
        {
            switch (DictionaryLanguage.ToLower().Trim())
            {
                case "es":
                    PageInstruction.AddFieldFilter("meta_description", (name, data) =>
                    {
                        data.Value = "Definición de " + termName;
                    });
                    break;
                default:
                    PageInstruction.AddFieldFilter("meta_description", (name, data) =>
                    {
                        data.Value = "Definition of " + termName;
                    });
                    break;
            }
        }

    }
}