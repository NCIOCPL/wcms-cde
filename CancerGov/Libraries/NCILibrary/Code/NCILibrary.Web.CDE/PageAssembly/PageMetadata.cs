﻿using System;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace NCI.Web.CDE
{
    public class PageMetadata
    {
        /// <summary>
        /// Gets the title of the requested page
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string LongTitle { get; set; }

        /// <summary>
        /// Gets the title of the requested page
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string ShortTitle { get; set; }

        /// <summary>
        /// Short description is used for populating the meta name="description"
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string ShortDescription { get; set; }

        /// <summary>
        /// Long description is used for populating the meta name="description" when both meta and short description filed are empty
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string LongDescription { get; set; }


        /// <summary>
        /// Gets the text that should be used for the meta name="description" tag.
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets the text that should be used for the name="keywords" tag.
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string MetaKeywords { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public DateTime PostedDate { get; set; }  

        public override bool Equals(object obj)
        {
            PageMetadata target = obj as PageMetadata;


            if (LongTitle != target.LongTitle)
                return false;


            if (ShortTitle != target.ShortTitle)
                return false;

            if (LongDescription != target.LongDescription)
                return false;

            if (MetaDescription != target.MetaDescription)
                return false;


            if (MetaKeywords != target.MetaKeywords)
                return false;

            if (PostedDate != target.PostedDate)
                return false;
            
            return true;
        }
    }
}
