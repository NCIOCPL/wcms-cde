﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using NCI.Logging;

namespace NCI.Web.CDE.Modules
{
    public class ModuleObjectFactory<ModuleObjectType>
    {
        private static System.Collections.Generic.Dictionary<string,XmlSerializer> serializers = new Dictionary<string,XmlSerializer>();

        public static ModuleObjectType GetModuleObject(string snippetXmlData)
        {
            try
            {
                using (XmlTextReader reader = new XmlTextReader(snippetXmlData.Trim(), XmlNodeType.Element, null))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ModuleObjectType), "cde");
                    return (ModuleObjectType)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("cde:ModuleObjectFactory.cs.GetModuleObject", "Invalid xml data in the snippet for DynamicList, check xml received from Percussion", NCIErrorLevel.Error);
                throw ex;
            }
        }

        public static ModuleObjectType GetObjectFromFile(string filePath)
        {
            try
            {
                XmlSerializer serializer = serializers[typeof(ModuleObjectType).ToString()];
                if( serializer== null )
                {
                    serializer = new XmlSerializer(typeof(ModuleObjectType), "cde");
                    serializers.Add( typeof(ModuleObjectType).ToString(), serializer);
                }
                using (FileStream xmlFile = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    using (XmlReader xmlReader = XmlReader.Create(xmlFile))
                    {
                        return (ModuleObjectType)serializer.Deserialize(xmlReader);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = String.Format("Unable to load object from file \"{0}.\"  The file may not exist or the XML in the file may not be deserializable into a valid object.", filePath);
                Logger.LogError("CDE:SectionDetailFactory.cs:GetObjectFromFile", message, NCIErrorLevel.Error, ex);
                throw ex;
            }
        }
    }
}
