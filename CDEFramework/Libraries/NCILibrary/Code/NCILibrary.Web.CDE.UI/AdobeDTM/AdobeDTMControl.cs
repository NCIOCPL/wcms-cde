using System;
using System.ComponentModel;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;

namespace NCI.Web.CDE.UI.WebControls
{
    /// <summary>
    /// This web control renders DTM-specific script tags on the site.
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:AdobeDTMControl runat=server></{0}:AdobeDTMControl>")]
    public class AdobeDTMControl : WebControl
    {
        static ILog log = LogManager.GetLogger(typeof(AdobeDTMControl));
        private static String DTMSrc = ConfigurationManager.AppSettings["DTMUrl"].ToString();
        private const String SCRIPT_TYPE = "text/javascript";
        private const String DTM_BOTTOM = "_satellite.pageBottom();";

        protected override void OnInit(EventArgs e)
        { // Not doing anything at the moment
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
        /// Draw the required tag based on the control ID set in the form.
        /// </summary>
        /// <param name="writer">HtmlTextWriter object</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            // Select which Draw() method to use
            switch (this.ID)
            {
                case "DTMTop":
                    this.DrawTopTag(writer);
                    break;
                case "DTMDirectCall":
                    this.DrawDirectCallTag(writer);
                    break;
                case "DTMBottom":
                    this.DrawBottomTag(writer);
                    break;
                default:
                    break;
            }

            // Draw the closing tag 
            writer.RenderEndTag();
        }

        /// <summary>Draw the analytics script tag with source for the head.</summary>
        /// <param name="writer">Text writer object used to output HTML tags</param>
        public void DrawTopTag(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Src, DTMSrc);
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
        }

        /// <summary>Draw the analytics Javascript for the page bottom.</summary>
        /// <param name="writer">Text writer object used to output HTML tags</param>
        public void DrawBottomTag(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, SCRIPT_TYPE);
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.Write(DTM_BOTTOM);
        }

        /// <summary>Draw a DTM direct call tag.</summary>
        /// <param name="writer">Text writer object used to output HTML tags</param>
        public void DrawDirectCallTag(HtmlTextWriter writer)
        {
            // Placeholder method if we need it later
        }
    }
}
