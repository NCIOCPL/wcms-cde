using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCI.Web.CDE.UI
{
    /// <summary>
    /// Enum to define named constants for html meta data types.
    /// </summary>
    public enum HtmlMetaDataType
    {
        Description,
        KeyWords,
        ContentType,
        ContentLanguage,
        Title,
        DatePublished,
        EnglishLinkingPolicy,
        EspanolLinkingPolicy,
        Robots
    }

    /// <summary>
    /// Enum to define named constants for html link ref types.
    /// </summary>
    public enum HtmlLinkRelType
    {
        SchemaDcTerms,
        Alternate,
        Next,
        Prev
    }

    /// <summary>
    /// Enum to define named contants for types of page option
    /// </summary>
    public enum PageOptionType
    { 
        Link,
        Email,
        BookMarkShare
    }

}
