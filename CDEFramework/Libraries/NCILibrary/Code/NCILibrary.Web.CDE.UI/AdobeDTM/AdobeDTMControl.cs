using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using NCI.Web.CDE.WebAnalytics;

namespace NCI.Web.CDE.UI.WebControls
{
    /// <summary>
    /// This web controls renders the Omniture java script required for web analytics. 
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:AdobeDTMControl runat=server></{0}:AdobeDTMControl>")]
    public class AdobeDTMControl : WebControl
    {
        static ILog log = LogManager.GetLogger(typeof(AdobeDTMControl));

        protected override void OnInit(EventArgs e)
        {
        }

        /// <summary>
        /// Override this method to avoid rendering the default span tag.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            RenderContents(writer);
        }
        /// <summary>
        /// Uses the WebAnalyticsPageLoad helper class to render the required omniture java script 
        /// for Web Analytics.
        /// </summary>
        /// <param name="output">HtmlTextWriter object</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            base.RenderContents(output);

            System.Web.UI.AttributeCollection coll = this.Attributes;
            String myID = this.ID;

            switch (myID)
            {
                case "top":
                    this.DrawTop(output);
                    break;
                case "middle":
                    this.DrawMiddle(output);
                    break;
                case "bottom":
                    this.DrawBottom(output);
                    break;
                default:
                    break;
            }

        }

        /// <summary>Draw the analytics metadata to be used in the document head.</summary>
        /// <param name="writer">Text writer object used to output HTML tags</param>
        public void DrawTop(HtmlTextWriter writer)
        {
            String id = "script-on-top";

            // Draw meta tag ID, suites, channel attributes
            writer.AddAttribute(HtmlTextWriterAttribute.Id, id);

            // Draw the <script> tag 
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();
        }

        /// <summary>Draw the analytics metadata to be used in the document head.</summary>
        /// <param name="writer">Text writer object used to output HTML tags</param>
        public void DrawMiddle(HtmlTextWriter writer)
        {
            String id = "script-in-mid";

            // Draw meta tag ID, suites, channel attributes
            writer.AddAttribute(HtmlTextWriterAttribute.Id, id);

            // Draw the <script> tag 
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();
        }

        /// <summary>Draw the analytics metadata to be used in the document head.</summary>
        /// <param name="writer">Text writer object used to output HTML tags</param>
        public void DrawBottom(HtmlTextWriter writer)
        {
            String id = "script-on-bottom";

            // Draw meta tag ID, suites, channel attributes
            writer.AddAttribute(HtmlTextWriterAttribute.Id, id);

            // Draw the <script> tag 
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.RenderEndTag();
        }
    }
}
