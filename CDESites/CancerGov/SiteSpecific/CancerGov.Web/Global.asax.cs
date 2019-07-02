using System;
using System.Net;
using Common.Logging;
using NCI.Web.CDE.Application;
using Quartz;
using Quartz.Impl;

namespace CancerGov.Web
{
    public class Global : GlobalApplication
    {
        static ILog log = LogManager.GetLogger(typeof(Global));

        protected override void SiteSpecificAppStart(object sender, EventArgs e)
        {

            //Setting to allow TLS 1.1 & 1.2 for HttpClient in addition the default 4.5.X TLS 1.0.
            //NOTE: This is supposed to be set for the AppDomain, however setting this in application start
            //This affects all connections.
            if ((ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) != SecurityProtocolType.Tls12)
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            }

            if ((ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls11) != SecurityProtocolType.Tls11)
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls11;
            }          
        }
    }
}