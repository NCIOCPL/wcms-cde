﻿using System;
using System.Web.UI;
using Common.Logging;

namespace NCI.Web.UI.WebControls
{
    public class SimpleUlPager : SimplePager
    {
        static ILog log = LogManager.GetLogger(typeof(SimpleUlPager));

        protected override void RenderNextLink(HtmlTextWriter output)
        {
            // Get the url for the link
            string url = GetNextLinkUrl();

            //If there is no image, and there is not text, then there is nothing to write.
            if (url != null)
            {
                // render the wrapping li and next class if available
                if (this.PagerStyleSettings.NextPageCssClass != string.Empty)
                    output.AddAttribute(HtmlTextWriterAttribute.Class, this.PagerStyleSettings.NextPageCssClass);
                output.RenderBeginTag(HtmlTextWriterTag.Li);

                if (this.PagerStyleSettings.NextPageSeparatorImageUrl != string.Empty)
                {
                    //If there is a class, draw a span tag, otherwise, just spit it out.
                    if (this.PagerStyleSettings.NextPageSeparatorCssClass != string.Empty)
                    {
                        output.AddAttribute(HtmlTextWriterAttribute.Class, this.PagerStyleSettings.NextPageSeparatorCssClass);
                        output.RenderBeginTag(HtmlTextWriterTag.Span);
                    }

                    //Draw separator image
                    if (this.PagerStyleSettings.NextPageSeparatorText != string.Empty)
                        output.AddAttribute(HtmlTextWriterAttribute.Alt, this.PagerStyleSettings.NextPageSeparatorText);
                    else
                        output.AddAttribute(HtmlTextWriterAttribute.Alt, "");
                    output.AddAttribute(HtmlTextWriterAttribute.Src, this.PagerStyleSettings.NextPageSeparatorImageUrl);
                    output.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                    output.RenderBeginTag(HtmlTextWriterTag.Img);
                    output.RenderEndTag();

                    if (this.PagerStyleSettings.NextPageSeparatorCssClass != string.Empty)
                        output.RenderEndTag();
                }
                else
                {
                    //The next button separator is only text
                    if (this.PagerStyleSettings.NextPageSeparatorText != string.Empty)
                    {
                        //If there is a class, draw a span tag, otherwise, just spit it out.
                        if (this.PagerStyleSettings.NextPageSeparatorCssClass != string.Empty)
                        {
                            output.AddAttribute(HtmlTextWriterAttribute.Class, this.PagerStyleSettings.NextPageSeparatorCssClass);
                            output.RenderBeginTag(HtmlTextWriterTag.Span);
                        }

                        output.Write(this.PagerStyleSettings.NextPageSeparatorText);

                        if (this.PagerStyleSettings.NextPageSeparatorCssClass != string.Empty)
                            output.RenderEndTag();
                    }
                }

                //Begin writing the next link
                output.AddAttribute(HtmlTextWriterAttribute.Href, url);
                output.RenderBeginTag(HtmlTextWriterTag.A);

                //Draw either the text or the image
                if (this.PagerStyleSettings.NextPageImageUrl != string.Empty)
                {
                    if (this.PagerStyleSettings.NextPageText != string.Empty)
                        output.AddAttribute(HtmlTextWriterAttribute.Alt, this.PagerStyleSettings.NextPageText);
                    else
                        output.AddAttribute(HtmlTextWriterAttribute.Alt, "");
                    output.AddAttribute(HtmlTextWriterAttribute.Src, this.PagerStyleSettings.NextPageImageUrl);
                    output.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                    output.RenderBeginTag(HtmlTextWriterTag.Img);
                    output.RenderEndTag();
                }
                else
                {
                    output.Write(this.PagerStyleSettings.NextPageText);
                }
                output.RenderEndTag();

                // close initial li
                output.RenderEndTag();

            }
        }

        protected override void RenderPrevLink(HtmlTextWriter output)
        {
            //Get the url for the link
            string url = GetPrevLinkUrl();

            //If there is no image, and there is not text, then there is nothing to write.
            if (url != null)
            {
                // render li and class if set
                if (this.PagerStyleSettings.PrevPageCssClass != string.Empty)
                    output.AddAttribute(HtmlTextWriterAttribute.Class, this.PagerStyleSettings.PrevPageCssClass);
                output.RenderBeginTag(HtmlTextWriterTag.Li);

                //Begin writing the previous link
                output.AddAttribute(HtmlTextWriterAttribute.Href, url);
                output.RenderBeginTag(HtmlTextWriterTag.A);

                //Draw either the text or the image
                if (this.PagerStyleSettings.PrevPageImageUrl != string.Empty)
                {
                    if (this.PagerStyleSettings.PrevPageText != string.Empty)
                        output.AddAttribute(HtmlTextWriterAttribute.Alt, this.PagerStyleSettings.PrevPageText);
                    else
                        output.AddAttribute(HtmlTextWriterAttribute.Alt, "");
                    output.AddAttribute(HtmlTextWriterAttribute.Src, this.PagerStyleSettings.PrevPageImageUrl);
                    output.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                    output.RenderBeginTag(HtmlTextWriterTag.Img);
                    output.RenderEndTag();
                }
                else
                {
                    output.Write(this.PagerStyleSettings.PrevPageText);
                }
                output.RenderEndTag();

                if (this.PagerStyleSettings.PrevPageSeparatorImageUrl != string.Empty)
                {
                    //If there is a class, draw a span tag, otherwise, just spit it out.
                    if (this.PagerStyleSettings.PrevPageSeparatorCssClass != string.Empty)
                    {
                        output.AddAttribute(HtmlTextWriterAttribute.Class, this.PagerStyleSettings.PrevPageSeparatorCssClass);
                        output.RenderBeginTag(HtmlTextWriterTag.Span);
                    }

                    //Draw separator image
                    if (this.PagerStyleSettings.PrevPageSeparatorText != string.Empty)
                        output.AddAttribute(HtmlTextWriterAttribute.Alt, this.PagerStyleSettings.PrevPageSeparatorText);
                    else
                        output.AddAttribute(HtmlTextWriterAttribute.Alt, "");
                    output.AddAttribute(HtmlTextWriterAttribute.Src, this.PagerStyleSettings.PrevPageSeparatorImageUrl);
                    output.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                    output.RenderBeginTag(HtmlTextWriterTag.Img);
                    output.RenderEndTag();

                    if (this.PagerStyleSettings.PrevPageSeparatorCssClass != string.Empty)
                        output.RenderEndTag();
                }
                else
                {
                    //The prev button separator is only text
                    if (this.PagerStyleSettings.PrevPageSeparatorText != string.Empty)
                    {
                        //If there is a class, draw a span tag, otherwise, just spit it out.
                        if (this.PagerStyleSettings.PrevPageSeparatorCssClass != string.Empty)
                        {
                            output.AddAttribute(HtmlTextWriterAttribute.Class, this.PagerStyleSettings.PrevPageSeparatorCssClass);
                            output.RenderBeginTag(HtmlTextWriterTag.Span);
                        }

                        output.Write(this.PagerStyleSettings.PrevPageSeparatorText);

                        if (this.PagerStyleSettings.PrevPageSeparatorCssClass != string.Empty)
                            output.RenderEndTag();
                    }
                }

                // close li
                output.RenderEndTag();
            }

        }

        protected override void RenderPageNumbers(HtmlTextWriter output, int startIndex, int endIndex)
        {
            //Loop through page numbers and draw page links
            for (int pageNumber = startIndex; pageNumber <= endIndex; pageNumber++)
            {
                // start li encapsulating all other content, with current item class if appropriate
                if (this.CurrentPage == pageNumber)
                {
                    string cssClass = GetItemCssClass(this.PagerStyleSettings.SelectedIndexCssClass, startIndex, endIndex, pageNumber);
                    //CurrentPageNumber
                    if (cssClass != string.Empty)
                        output.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
                }
                output.RenderBeginTag(HtmlTextWriterTag.Li);

                if (pageNumber > startIndex)
                {
                    //Draw the separator between items
                    if (this.PagerStyleSettings.PageSeparatorImageUrl != string.Empty)
                    {
                        //If there is a class, draw a span tag, otherwise, just spit it out.
                        if (this.PagerStyleSettings.PageSeparatorCssClass != string.Empty)
                        {
                            output.AddAttribute(HtmlTextWriterAttribute.Class, this.PagerStyleSettings.PageSeparatorCssClass);
                            output.RenderBeginTag(HtmlTextWriterTag.Span);
                        }

                        //Draw separator image
                        if (this.PagerStyleSettings.PageSeparatorText != string.Empty)
                            output.AddAttribute(HtmlTextWriterAttribute.Alt, this.PagerStyleSettings.PageSeparatorText);
                        else
                            output.AddAttribute(HtmlTextWriterAttribute.Alt, "");
                        output.AddAttribute(HtmlTextWriterAttribute.Src, this.PagerStyleSettings.PageSeparatorImageUrl);
                        output.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                        output.RenderBeginTag(HtmlTextWriterTag.Img);
                        output.RenderEndTag();

                        if (this.PagerStyleSettings.PageSeparatorCssClass != string.Empty)
                            output.RenderEndTag();
                    }
                    else
                    {
                        //The next button separator is only text
                        if (this.PagerStyleSettings.PageSeparatorText != string.Empty)
                        {
                            //If there is a class, draw a span tag, otherwise, just spit it out.
                            if (this.PagerStyleSettings.PageSeparatorCssClass != string.Empty)
                            {
                                output.AddAttribute(HtmlTextWriterAttribute.Class, this.PagerStyleSettings.PageSeparatorCssClass);
                                output.RenderBeginTag(HtmlTextWriterTag.Span);
                            }

                            output.Write(this.PagerStyleSettings.PageSeparatorText);

                            if (this.PagerStyleSettings.PageSeparatorCssClass != string.Empty)
                                output.RenderEndTag();
                        }
                    }
                }

                if (this.CurrentPage != pageNumber)
                {
                    int offSet = ((pageNumber - 1) * this.RecordsPerPage);
                    string url = GetItemUrl(offSet, pageNumber);

                    string cssClass = GetItemCssClass(this.PagerStyleSettings.IndexLinkCssClass, startIndex, endIndex, pageNumber);

                    if (cssClass != string.Empty)
                        output.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
                    output.AddAttribute(HtmlTextWriterAttribute.Href, url);
                    output.RenderBeginTag(HtmlTextWriterTag.A);
                    output.Write(pageNumber.ToString());
                    output.RenderEndTag();
                }
                else
                {
                    output.RenderBeginTag(HtmlTextWriterTag.Span);
                    output.Write(pageNumber.ToString());
                    output.RenderEndTag();
                }

                // close li
                output.RenderEndTag();
            }
        }

        /// <summary>
        /// Renders the contents of this control.
        /// </summary>
        /// <param name="output">A HtmlTextWriter to render the contents to.</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            try
            {
                // Get number of pages
                int pages = 0;
                if (this.RecordsPerPage > 0)
                {
                    pages = (this.RecordCount / this.RecordsPerPage) + ((this.RecordCount % this.RecordsPerPage > 0) ? 1 : 0);
                }

                if (pages > 1)
                {
                    // wrap output in ul
                    output.RenderBeginTag(HtmlTextWriterTag.Ul);
                    base.RenderContents(output);
                    output.RenderEndTag();
                }
            }
            catch (Exception e)
            {
                log.Error("RenderContents(): encountered error while rendering", e);
            }
        }
    }
}
