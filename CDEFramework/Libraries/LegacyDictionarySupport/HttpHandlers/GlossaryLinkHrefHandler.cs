using System;
using System.Web;
using System.Net.Http;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

using LegacyDictionarySupport.Configuration;

namespace LegacyDictionarySupport.HttpHandlers
{

    public class GlossaryLinkHrefHandler : IHttpHandler
    {
        // implement IsReusable method
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;

            // Format ID parameter for API call
            string id = request.QueryString["id"];
            id = Regex.Replace(id, "^CDR0+", "", RegexOptions.Compiled);

            // Format dictionary and audience parameters for API call
            string dictionary = request.QueryString["dictionary"];
            if (!string.IsNullOrWhiteSpace(dictionary) && dictionary.ToLower() == "genetic")
            {
                dictionary = "Genetics";
            }

            string audience = request.QueryString["version"];
            if (!string.IsNullOrWhiteSpace(audience) && audience.ToLower() == "patient")
            {
                audience = "Patient";
            }
            if (!string.IsNullOrWhiteSpace(audience) && audience.ToLower() == "healthprofessional")
            {
                audience = "HealthProfessional";
            }

            if (string.IsNullOrWhiteSpace(dictionary) && string.IsNullOrWhiteSpace(audience))
            {
                dictionary = "Cancer.gov";
                audience = "Patient";
            }

            if (!string.IsNullOrWhiteSpace(audience) && string.IsNullOrWhiteSpace(dictionary))
            {
                switch (audience.ToLower())
                {
                    case "patient":
                        dictionary = "Cancer.gov";
                        audience = "Patient";
                        break;

                    case "healthprofessional":
                        dictionary = "NotSet";
                        audience = "HealthProfessional";
                        break;

                    default:
                        dictionary = "NotSet";
                        audience = "Patient";
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(dictionary) && string.IsNullOrWhiteSpace(audience))
            {
                audience = dictionary.ToLower() == "cancer.gov" ? "Patient" : "HealthProfessional";
            }

            // Format language parameter for API call
            string language = request.QueryString["language"];
            if(language == "English")
            {
                language = "en";
            }
            else if (language == "Spanish")
            {
                language = "es";
            }

            bool useFallback = true;

            GlossaryAPIClient client = new GlossaryAPIClient();

            GlossaryTerm term = client.GetById(dictionary, audience, language, id, useFallback);

            if (term == null)
            {
                throw new HttpException(404, String.Format("Term with CDRID {0} does not exist", id));
            }
            else
            {
                string termForRedirect = String.IsNullOrEmpty(term.PrettyUrlName) ? term.TermId : term.PrettyUrlName;
                string dictionaryPath = null;

                switch (term.Dictionary.ToLower())
                {
                    case "cancer.gov":
                        if (term.Language.ToLower() == "en")
                        {
                            dictionaryPath = LegacyDictionarySupportSection.GetEnglishTermDictionaryUrl();
                        }
                        if (term.Language.ToLower() == "es")
                        {
                            dictionaryPath = LegacyDictionarySupportSection.GetSpanishTermDictionaryUrl();
                        }
                        break;

                    case "genetics":
                        if (term.Language.ToLower() == "en")
                        {
                            dictionaryPath = LegacyDictionarySupportSection.GetEnglishGeneticsDictionaryUrl();
                        }
                        break;

                    case "notset":
                        break;

                    default:
                        break;
                }

                if (!string.IsNullOrWhiteSpace(dictionaryPath))
                {
                    context.Response.Redirect(String.Format("{0}/def/{1}", dictionaryPath, termForRedirect));
                }
                else
                {
                    context.Response.Write(@"
<!DOCTYPE html>
    <html lang=""" + term.Language + @""">
    <head>
        <title>Definition of " + term.TermName + @"</title>
        <meta name=""robots"" content=""noindex, nofollow"" />
        <style>
        @import url(""https://fonts.googleapis.com/css2?family=Noto+Sans:wght@400;700&display=swap"");
        .definition {
            font-family: ""Noto Sans"";
            margin: 30px 15px;
        }
        .definition__header {
            margin-bottom: 15px;
        }
        .definition h1 {
            font-size: 16px;
            font-weight: 700;
            display: inline-block;
        }
        .definition dd {
            margin: 0;
        }
        .definition dd +* {
            margin - top: 15px;
        }
        </style>
    </head>");

                    context.Response.Write(@"
    <body>
        <div class=""definition"">
            <dl>
            <div class=""definition__header"">
                <a href=""/"" id=""logoAnchor"">
                <img src=""https://www.cancer.gov/publishedcontent/images/images/design-elements/logos/nci-logo-full.svg""  id=""logoImage"" alt=""National Cancer Institute"" width=""300"" />
                </a>
            </div>
            <dt>
                <h1>" + term.TermName + @"</h1>");

                    if(term.Pronunciation != null && term.Pronunciation.Key != null)
                    {
                        context.Response.Write(@"
                <span class=""pronunciation"">" + term.Pronunciation.Key + @"</span>");
                    }

                    context.Response.Write(@"
            </dt>");

                    if (term.Definition != null && term.Definition.Text != null)
                    {
                        context.Response.Write(@"
            <dd>" + term.Definition.Text + @"</dd>");
                    }

                    context.Response.Write(@"
        </dl>
        </div>
    </body>");

                    context.Response.Write(@"
</html>");
                }
            }
        }
    }
}
