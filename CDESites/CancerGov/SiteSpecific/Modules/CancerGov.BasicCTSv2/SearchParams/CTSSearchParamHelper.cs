using NCI.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Collection of extension methods to help parsing of CTS Search Parameters.
    /// </summary>
    public static class CTSSearchParamExtensions
    {


        /// <summary>
        /// Gets a query parameter as a string or uses a default
        /// </summary>
        /// <param name="param"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string CTSParamAsStr(this NameValueCollection collection, string param, string def = "")
        {
            string paramval = collection[param];

            return ParamAsStr(paramval, def);
        }

        /// <summary>
        /// Gets a query parameter as a string or uses a default
        /// </summary>
        /// <param name="param"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static string CTSParamAsStr(this NciUrl url, string param, string def = "")
        {
            string paramval = url.QueryParameters[param];

            return ParamAsStr(paramval, def);
        }

        /// <summary>
        /// Gets a query parameter as a string
        /// </summary>
        public static string ParamAsStr(string paramVal, string def = "")
        {
            if (string.IsNullOrWhiteSpace(paramVal))
                return def;
            else
                return paramVal.Trim();
        }


        /// <summary>
        /// Gets a query parameter as an int or uses a default
        /// </summary>
        /// <param name="param"></param>
        /// <param name="def"></param>
        /// <param name="isInvalid">An output variable indicating if the value was set, but was NOT a valid int</param>
        /// <returns></returns>
        public static int CTSParamAsInt(this NameValueCollection collection, string param, int def, out bool isInvalid)
        {
            string paramval = collection[param];
            return ParamAsInt(paramval, def, out isInvalid);
        }

        /// <summary>
        /// Gets a query parameter as an int or uses a default
        /// </summary>
        /// <param name="param"></param>
        /// <param name="def"></param>
        /// <param name="isInvalid">An output variable indicating if the value was set, but was NOT a valid int</param>
        /// <returns></returns>
        public static int CTSParamAsInt(this NameValueCollection collection, string param, int def)
        {
            string paramval = collection[param];
            bool isInvalid;
            return ParamAsInt(paramval, def, out isInvalid);
        }


        /// <summary>
        /// Gets a query parameter as an int or uses a default
        /// </summary>
        /// <param name="param"></param>
        /// <param name="def"></param>
        /// <param name="isInvalid">An output variable indicating if the value was set, but was NOT a valid int</param>
        /// <returns></returns>
        public static int CTSParamAsInt(this NciUrl url, string param, int def, out bool isInvalid)
        {
            string paramval = url.QueryParameters[param];
            return ParamAsInt(paramval, def, out isInvalid);
        }

        /// <summary>
        /// Gets a query parameter as an int or uses a default
        /// </summary>
        /// <param name="param"></param>
        /// <param name="def"></param>
        /// <param name="isInvalid">An output variable indicating if the value was set, but was NOT a valid int</param>
        /// <returns></returns>
        public static int CTSParamAsInt(this NciUrl url, string param, int def)
        {
            string paramval = url.QueryParameters[param];
            bool isInvalid;
            return ParamAsInt(paramval, def, out isInvalid);
        }


        /// <summary>
        /// Converts a query param to an int; returns 0 if unable to parse
        /// </summary>
        public static int ParamAsInt(string paramVal, int def)
        {
            bool isInvalid;
            return ParamAsInt(paramVal, def, out isInvalid);

        }

        /// <summary>
        /// Converts a query param to an int; returns 0 if unable to parse
        /// </summary>
        public static int ParamAsInt(string paramVal, int def, out bool isInvalid)
        {

            isInvalid = false;

            if (string.IsNullOrWhiteSpace(paramVal))
            {
                return def;
            }
            else
            {
                int tmpInt = 0;
                if (int.TryParse(paramVal.Trim(), out tmpInt))
                {
                    if (tmpInt == 0)
                        isInvalid = true;

                    return tmpInt;
                }
                else
                {
                    isInvalid = true;
                    return def;
                }
            }
        }

    }
}
