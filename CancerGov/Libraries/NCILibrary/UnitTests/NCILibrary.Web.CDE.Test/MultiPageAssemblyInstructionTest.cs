﻿using NCI.Web.CDE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCI.Web;
using System.Xml.Schema;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Web;
using System;
using NCI.Test.Web;
using System.Reflection;
namespace NCI.Web.CDE.Test
{
    
    
    /// <summary>
    ///This is a test class for MultiPageAssemblyInstructionTest and is intended
    ///to contain all MultiPageAssemblyInstructionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MultiPageAssemblyInstructionTest : CDETest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        //public TestContext TestContext
        //{
        //    get
        //    {
        //        return testContextInstance;
        //    }
        //    set
        //    {
        //        testContextInstance = value;
        //    }
        //}

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private IPageAssemblyInstruction InitializeTestPageAssemblyInfo()
        {
            string xmlFilePath = TestContext.TestDeploymentDir + "\\PublishedContent\\PageInstructions\\Multicancertopics.xml";

            IPageAssemblyInstruction pageAssemblyInfo = null;
            using (XmlReader xmlReader = XmlReader.Create(xmlFilePath))
            {
                xmlReader.MoveToContent();
                string pageAssemblyInfoTypeName = xmlReader.LocalName;

                //XmlSerializer serializer = _serializers[pageAssemblyInfoTypeName];
                XmlSerializer serializer = new XmlSerializer(typeof(MultiPageAssemblyInstruction));

                // Deserialize the XML into an object.
                pageAssemblyInfo = (IMultiPageAssemblyInstruction)serializer.Deserialize(xmlReader);
                return pageAssemblyInfo;
            }
        }



        [TestMethod()]
        [DeploymentItem(@"XmlFiles")]
        public void MultiPageAssemblyInstruction_XMLSerializer_Test()
        {
            IPageAssemblyInstruction pageAssemblyInfo = null;
            pageAssemblyInfo = InitializeTestPageAssemblyInfo();

            Assert.IsNotNull(pageAssemblyInfo);
            Assert.IsNotNull(pageAssemblyInfo.PageTemplateName);
            Assert.IsNotNull(pageAssemblyInfo.SectionPath);
        }



        [TestMethod()]
        [DeploymentItem(@"XmlFiles")]
        public void MultiPageAssemblyInstruction_ContainsURL_Test()
        {

            using (HttpSimulator httpSimulator = GetStandardSimulatedRequest())
            {

                IPageAssemblyInstruction actual = PageAssemblyInstructionFactory.GetPageAssemblyInfo("/multicancertopics");

                Object[] args = new Object[] { "multicancertopics/page100" };
                Boolean boolContainsUrl = (Boolean)actual.GetType().InvokeMember("ContainsURL", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic |
                                             BindingFlags.Instance | BindingFlags.InvokeMethod, null, actual, args);

                Assert.IsFalse(boolContainsUrl);
            }
        }


        [TestMethod()]
        [DeploymentItem(@"XmlFiles")]
        public void GetField_Test()
        {

            IPageAssemblyInstruction pageAssemblyInfo = null;
            pageAssemblyInfo = InitializeTestPageAssemblyInfo();

            pageAssemblyInfo.AddFieldFilter("Foo12345", data =>
            {
                data.Value = "Foo12345";
            });

            Assert.AreEqual("Foo12345", pageAssemblyInfo.GetField("Foo12345"));


        }


        [TestMethod()]
        [DeploymentItem(@"XmlFiles")]
        public void GetField_MultipleFieldFilters_Test()
        {

            IPageAssemblyInstruction pageAssemblyInfo = null;
            pageAssemblyInfo = InitializeTestPageAssemblyInfo();

            pageAssemblyInfo.AddFieldFilter("Foo12345", data =>
            {
                data.Value = "Dictionary of cancer terms";
            });

            //Add another one, but make sure we chain from the previous
            pageAssemblyInfo.AddFieldFilter("Foo12345", data =>
            {
                data.Value += "--Modified";
            });

            Assert.AreEqual("Dictionary of cancer terms--Modified", pageAssemblyInfo.GetField("Foo12345"));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "The fieldName parameter may not be null or empty.")]
        [DeploymentItem(@"XmlFiles")]
        public void AddField_NullFieldName_Test()
        {
            IPageAssemblyInstruction pageAssemblyInfo = null;
            pageAssemblyInfo = InitializeTestPageAssemblyInfo();

            pageAssemblyInfo.AddFieldFilter(null, data =>
            {
                data.Value = "Dictionary of cancer terms";
            });

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException), "The fieldName parameter may not be null or empty.")]
        [DeploymentItem(@"XmlFiles")]
        public void AddField_EmptyFieldName_Test()
        {
            IPageAssemblyInstruction pageAssemblyInfo = null;
            pageAssemblyInfo = InitializeTestPageAssemblyInfo();

            pageAssemblyInfo.AddFieldFilter(string.Empty, data =>
            {
                data.Value = "Dictionary of cancer terms";
            });

        }


        /* Test the MultiPageassemblyinstruction's individual Field Filters. */
        [TestMethod()]
        [DeploymentItem(@"XmlFiles")]
        public void GetField_HTML_Title_Field_Test()
        {

            using (HttpSimulator httpSimulator = GetStandardSimulatedRequest())
            {
                string HTML_Title = "Cancer Topics Home Page--About This Booklet";
                IPageAssemblyInstruction actual = PageAssemblyInstructionFactory.GetPageAssemblyInfo("/multicancertopics");
                Assert.AreEqual(HTML_Title, actual.GetField("HTML_Title"));


            }

        }

        [TestMethod()]
        [DeploymentItem(@"XmlFiles")]
        public void GetField_HTML_MetaDescription_Test()
        {
            using (HttpSimulator httpSimulator = GetStandardSimulatedRequest())
            {
                string MetaDescription = "sdfds --About This Booklet";

                IPageAssemblyInstruction actual = PageAssemblyInstructionFactory.GetPageAssemblyInfo("/multicancertopics");
                Assert.AreEqual(MetaDescription, actual.GetField("HTML_MetaDescription"));
            }
        }


        [TestMethod()]
        [DeploymentItem(@"XmlFiles")]
        public void GetField_HTML_MetaKeywords_Test()
        {
            using (HttpSimulator httpSimulator = GetStandardSimulatedRequest())
            {

                string MetaKeywords = "cancer,information,About This Booklet";
                IPageAssemblyInstruction actual = PageAssemblyInstructionFactory.GetPageAssemblyInfo("/multicancertopics");

                Assert.AreEqual(MetaKeywords, actual.GetField("HTML_MetaKeywords"));
            }
        }

        [TestMethod()]
        [DeploymentItem(@"XmlFiles")]
        public void GetBlockedSlots_Test()
        {
            IPageAssemblyInstruction pageAssemblyInfo = null;
            pageAssemblyInfo = InitializeTestPageAssemblyInfo();
            string[] expectedBlockSlots = { "cgvContentHeader" };
            string[] actualblockedSlots;

            actualblockedSlots = pageAssemblyInfo.BlockedSlotNames;
            Assert.AreEqual(expectedBlockSlots[0], actualblockedSlots[0]);

        }
    }
}
