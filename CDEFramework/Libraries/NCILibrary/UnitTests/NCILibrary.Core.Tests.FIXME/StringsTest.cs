using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCI.Util;

namespace NCILibrary.Core.Tests
{
    [TestClass()]
    public class StringsTest
    {
        #region Clean Tests

        /// <summary>
        ///A test for Clean
        ///</summary>
        [TestMethod()]
        [Description("Clean(obj)")]
        public void Clean_obj_Test()
        {
            Assert.AreEqual(null, Strings.Clean(null), "Strings.Clean(null) did not pass");
            Assert.AreEqual(null, Strings.Clean("  "), @"Strings.Clean(""  "") did not pass");
            Assert.AreEqual("a", Strings.Clean(" a "), @"Strings.Clean("" a "") did not pass");
        }

        /// <summary>
        ///A test for Clean
        ///</summary>
        [TestMethod()]
        [Description("Clean(obj, bool)")]
        public void Clean_obj_bool_Test()
        {
            Assert.AreEqual(null, Strings.Clean(null, false), "Strings.Clean(null, false) did not pass");
            Assert.AreEqual(null, Strings.Clean(null, true), "Strings.Clean(null, true) did not pass");
            Assert.AreEqual(null, Strings.Clean("  ", false), @"Strings.Clean(""  "", false) did not pass");
            Assert.AreEqual("", Strings.Clean("  ", true), @"Strings.Clean(""  "", true) did not pass");
            Assert.AreEqual("a", Strings.Clean(" a ", false), @"Strings.Clean("" a "", false) did not pass");
            Assert.AreEqual("a", Strings.Clean(" a ", true), @"Strings.Clean("" a "", true) did not pass");
        }

        /// <summary>
        ///A test for Clean
        ///</summary>
        [TestMethod()]
        [Description("Clean(obj, string)")]
        public void Clean_obj_str_Test()
        {
            Assert.AreEqual("default", Strings.Clean(null, "default"), @"Strings.Clean(null, ""default"") did not pass");
            Assert.AreEqual("default", Strings.Clean("  ", "default"), @"Strings.Clean(""  "", ""default"") did not pass");
            Assert.AreEqual("a", Strings.Clean(" a ", "default"), @"Strings.Clean("" a "", ""default"") did not pass");
        }

        /// <summary>
        ///A test for Clean
        ///</summary>
        [TestMethod()]
        [Description("Clean(obj, string, bool)")]
        public void Clean_obj_str_bool_Test()
        {
            Assert.AreEqual("default", Strings.Clean(null, "default", false), @"Strings.Clean(null, ""default"", false) did not pass");
            Assert.AreEqual("default", Strings.Clean(null, "default", true), @"Strings.Clean(null, ""default"", true) did not pass");
            Assert.AreEqual("default", Strings.Clean("  ", "default", false), @"Strings.Clean(""  "", ""default"", false) did not pass");
            Assert.AreEqual("", Strings.Clean("  ", "default", true), @"Strings.Clean(""  "", ""default"", true) did not pass");
            Assert.AreEqual("a", Strings.Clean(" a ", "default", false), @"Strings.Clean("" a "", ""default"", false) did not pass");
            Assert.AreEqual("a", Strings.Clean(" a ", "default", true), @"Strings.Clean("" a "", ""default"", true) did not pass");
        }
        
        #endregion

        #region Conversion Tests

        #region ToBoolean

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj)")]
        public void ToBoolean_obj_Test()
        {
            Assert.AreEqual(false, Strings.ToBoolean(null), @"ToBoolean(null) did not pass");
            Assert.AreEqual(false, Strings.ToBoolean(""), @"ToBoolean("""") did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("NO"), @"ToBoolean(""NO"") did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("N"), @"ToBoolean(""N"") did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("0"), @"ToBoolean(""0"") did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("false"), @"ToBoolean(""false"") did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("chicken"), @"ToBoolean(""chicken"") did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("27"), @"ToBoolean(""27"") did not pass");

            Assert.AreEqual(true, Strings.ToBoolean("YES"), @"ToBoolean(""YES"") did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("Y"), @"ToBoolean(""Y"") did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("1"), @"ToBoolean(""1"") did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("true"), @"ToBoolean(""true"") did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  YES  "), @"ToBoolean(""  YES  "") did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  Y  "), @"ToBoolean(""  Y  "") did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  1  "), @"ToBoolean(""  1  "") did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  true  "), @"ToBoolean(""  true  "") did not pass");
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        public void ToBoolean_obj_bool_bool_Test()
        {
            Assert.AreEqual(false, Strings.ToBoolean(null, false, false), @"ToBoolean(null, false, false) did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("", false, false), @"ToBoolean("""", false, false) did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("NO", false, false), @"ToBoolean(""NO"", false, false) did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("N", false, false), @"ToBoolean(""N"", false, false) did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("0", false, false), @"ToBoolean(""0"", false, false) did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("false", false, false), @"ToBoolean(""false"", false, false) did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("chicken", false, false), @"ToBoolean(""chicken"", false, false) did not pass");
            Assert.AreEqual(false, Strings.ToBoolean("27", false, false), @"ToBoolean(""27"", false, false) did not pass");

            Assert.AreEqual(true, Strings.ToBoolean(null, true, false), @"ToBoolean(null, true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("", true, false), @"ToBoolean("""", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("NO", true, false), @"ToBoolean(""NO"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("N", true, false), @"ToBoolean(""N"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("0", true, false), @"ToBoolean(""0"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("false", true, false), @"ToBoolean(""false"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("chicken", true, false), @"ToBoolean(""chicken"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("27", true, false), @"ToBoolean(""27"", true, false) did not pass");

            Assert.AreEqual(true, Strings.ToBoolean("YES", false, false), @"ToBoolean(""YES"", false, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("Y", false, false), @"ToBoolean(""Y"", false, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("1", false, false), @"ToBoolean(""1"", false, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("true", false, false), @"ToBoolean(""true"", false, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  YES  ", false, false), @"ToBoolean(""  YES  "", false, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  Y  ", false, false), @"ToBoolean(""  Y  "", false, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  1  ", false, false), @"ToBoolean(""  1  "", false, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  true  ", false, false), @"ToBoolean(""  true  "", false, false) did not pass");

            Assert.AreEqual(true, Strings.ToBoolean("YES", true, false), @"ToBoolean(""YES"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("Y", true, false), @"ToBoolean(""Y"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("1", true, false), @"ToBoolean(""1"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("true", true, false), @"ToBoolean(""true"", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  YES  ", true, false), @"ToBoolean(""  YES  "", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  Y  ", true, false), @"ToBoolean(""  Y  "", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  1  ", true, false), @"ToBoolean(""  1  "", true, false) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  true  ", true, false), @"ToBoolean(""  true  "", true, false) did not pass");

            #region Throw Errors == True that do not throw errors

            Assert.AreEqual(true, Strings.ToBoolean("YES", false, true), @"ToBoolean(""YES"", false, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("Y", false, true), @"ToBoolean(""Y"", false, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("1", false, true), @"ToBoolean(""1"", false, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("true", false, true), @"ToBoolean(""true"", false, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  YES  ", false, true), @"ToBoolean(""  YES  "", false, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  Y  ", false, true), @"ToBoolean(""  Y  "", false, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  1  ", false, true), @"ToBoolean(""  1  "", false, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  true  ", false, true), @"ToBoolean(""  true  "", false, true) did not pass");

            Assert.AreEqual(true, Strings.ToBoolean("YES", true, true), @"ToBoolean(""YES"", true, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("Y", true, true), @"ToBoolean(""Y"", true, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("1", true, true), @"ToBoolean(""1"", true, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("true", true, true), @"ToBoolean(""true"", true, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  YES  ", true, true), @"ToBoolean(""  YES  "", true, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  Y  ", true, true), @"ToBoolean(""  Y  "", true, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  1  ", true, true), @"ToBoolean(""  1  "", true, true) did not pass");
            Assert.AreEqual(true, Strings.ToBoolean("  true  ", true, true), @"ToBoolean(""  true  "", true, true) did not pass");

            Assert.AreEqual(false, Strings.ToBoolean("false", false, true), @"ToBoolean(""false"", false, true) did not pass");

            #endregion
        }

        #region Throw Exceptions

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean(null, false, true) did not pass")]
        public void ToBoolean_obj_bool_bool_NULL_Throw_Test()
        {
            Strings.ToBoolean(null, false, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean(null, true, true) did not pass")]
        public void ToBoolean_obj_bool_bool_NULL_Throw_Test2()
        {
            Strings.ToBoolean(null, true, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean("""", false, true) did not pass")]
        public void ToBoolean_obj_bool_bool_EmptyString_Throw_Test()
        {
            Strings.ToBoolean("", false, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean("""", true, true) did not pass")]
        public void ToBoolean_obj_bool_bool_EmptyString_Throw_Test2()
        {
            Strings.ToBoolean("", true, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean(""0"", false, true) did not pass")]
        public void ToBoolean_obj_bool_bool_0_Throw_Test()
        {
            Strings.ToBoolean("0", false, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean(""0"", true, true) did not pass")]
        public void ToBoolean_obj_bool_bool_0_Throw_Test2()
        {
            Strings.ToBoolean("0", true, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean(""NO"", false, true) did not pass")]
        public void ToBoolean_obj_bool_bool_NO_Throw_Test()
        {
            Strings.ToBoolean("NO", false, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean(""NO"", true, true) did not pass")]
        public void ToBoolean_obj_bool_bool_NO_Throw_Test2()
        {
            Strings.ToBoolean("NO", true, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean(""chicken"", false, true) did not pass")]
        public void ToBoolean_obj_bool_bool_Chicken_Throw_Test()
        {
            Strings.ToBoolean("chicken", false, true);
        }

        /// <summary>
        ///A test for ToBoolean
        ///</summary>
        [TestMethod()]
        [Description("ToBoolean(obj, bool, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"ToBoolean(""chicken"", true, true) did not pass")]
        public void ToBoolean_obj_bool_bool_Chicken_Throw_Test2()
        {
            Strings.ToBoolean("chicken", true, true);
        }

        #endregion

        #endregion

        #region ToInt

        /// <summary>
        ///A test for ToInt
        ///</summary>
        [TestMethod()]
        [Description("ToInt(obj)")]
        public void ToInt_obj_Test()
        {
            Assert.AreEqual(-1, Strings.ToInt(null), @"Strings.ToInt(null) did not pass");
            Assert.AreEqual(-1, Strings.ToInt(""), @"Strings.ToInt("""") did not pass");
            Assert.AreEqual(-1, Strings.ToInt("4.735"), @"Strings.ToInt(""4.735"") did not pass");
            Assert.AreEqual(-1, Strings.ToInt("chicken"), @"Strings.ToInt(""chicken"") did not pass");
            Assert.AreEqual(-1, Strings.ToInt("2147483648"), @"Strings.ToInt(""2147483648"") did not pass"); //maxint +1
            
            Assert.AreEqual(5, Strings.ToInt("5"), @"Strings.ToInt(""5"") did not pass");
            Assert.AreEqual(5, Strings.ToInt("  5  "), @"Strings.ToInt(""  5  "") did not pass");
            Assert.AreEqual(0, Strings.ToInt("0"), @"Strings.ToInt(""0"") did not pass");
            Assert.AreEqual(0, Strings.ToInt("  0  "), @"Strings.ToInt(""  0  "") did not pass");
            Assert.AreEqual(-5, Strings.ToInt("-5"), @"Strings.ToInt(""-5"") did not pass");
            Assert.AreEqual(-5, Strings.ToInt("  -5  "), @"Strings.ToInt(""  -5  "") did not pass");

        }

        /// <summary>
        ///A test for ToInt
        ///</summary>
        [TestMethod()]
        [Description("ToInt(obj, int)")]
        public void ToInt_obj_int_Test()
        {
            Assert.AreEqual(5, Strings.ToInt(null, 5), @"Strings.ToInt(null, 5) did not pass");
            Assert.AreEqual(5, Strings.ToInt("", 5), @"Strings.ToInt("""", 5) did not pass");
            Assert.AreEqual(5, Strings.ToInt("4.735", 5), @"Strings.ToInt(""4.735"", 5) did not pass");
            Assert.AreEqual(5, Strings.ToInt("chicken", 5), @"Strings.ToInt(""chicken"", 5) did not pass");
            Assert.AreEqual(5, Strings.ToInt("2147483648", 5), @"Strings.ToInt(""2147483648"", 5) did not pass"); //maxint +1

            Assert.AreEqual(6, Strings.ToInt("6", 5), @"Strings.ToInt(""6"", 5) did not pass");
            Assert.AreEqual(6, Strings.ToInt("  6  ", 5), @"Strings.ToInt(""  6  "", 5) did not pass");
            Assert.AreEqual(0, Strings.ToInt("0", 5), @"Strings.ToInt(""0"", 5) did not pass");
            Assert.AreEqual(0, Strings.ToInt("  0  ", 5), @"Strings.ToInt(""  0  "", 5) did not pass");
            Assert.AreEqual(-5, Strings.ToInt("-5", 5), @"Strings.ToInt(""-5"", 5) did not pass");
            Assert.AreEqual(-5, Strings.ToInt("  -5  ", 5), @"Strings.ToInt(""  -5  "", 5) did not pass");

        }

        /// <summary>
        ///A test for ToInt
        ///</summary>
        [TestMethod()]
        [Description("ToInt(obj, bool)")]
        public void ToInt_obj_bool_Test()
        {            
            Assert.AreEqual(-1, Strings.ToInt(null, false), @"Strings.ToInt(null, false) did not pass");
            Assert.AreEqual(-1, Strings.ToInt("", false), @"Strings.ToInt("""", false) did not pass");
            Assert.AreEqual(-1, Strings.ToInt("4.735", false), @"Strings.ToInt(""4.735"", false) did not pass");
            Assert.AreEqual(-1, Strings.ToInt("chicken", false), @"Strings.ToInt(""chicken"", false) did not pass");
            Assert.AreEqual(-1, Strings.ToInt("2147483648", false), @"Strings.ToInt(""2147483648"", false) did not pass"); //maxint +1

            Assert.AreEqual(5, Strings.ToInt("5", false), @"Strings.ToInt(""5"", false) did not pass");
            Assert.AreEqual(5, Strings.ToInt("  5  ", false), @"Strings.ToInt(""  5  "", false) did not pass");
            Assert.AreEqual(0, Strings.ToInt("0", false), @"Strings.ToInt(""0"", false) did not pass");
            Assert.AreEqual(0, Strings.ToInt("  0  ", false), @"Strings.ToInt(""  0  "", false) did not pass");
            Assert.AreEqual(-5, Strings.ToInt("-5", false), @"Strings.ToInt(""-5"", false) did not pass");
            Assert.AreEqual(-5, Strings.ToInt("  -5  ", false), @"Strings.ToInt(""  -5  "", false) did not pass");

            Assert.AreEqual(6, Strings.ToInt("6", true), @"Strings.ToInt(""6"", true) did not pass");
            Assert.AreEqual(6, Strings.ToInt("  6  ", true), @"Strings.ToInt(""  6  "", true) did not pass");
            Assert.AreEqual(0, Strings.ToInt("0", true), @"Strings.ToInt(""0"", true) did not pass");
            Assert.AreEqual(0, Strings.ToInt("  0  ", true), @"Strings.ToInt(""  0  "", true) did not pass");
            Assert.AreEqual(-5, Strings.ToInt("-5", true), @"Strings.ToInt(""-5"", true) did not pass");
            Assert.AreEqual(-5, Strings.ToInt("  -5  ", true), @"Strings.ToInt(""  -5  "", true) did not pass");
        }

        /// <summary>
        ///A test for ToInt
        ///</summary>
        [TestMethod()]
        [Description("ToInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToInt(null, true) did not pass")]
        public void ToInt_obj_bool_NULL_Throw_Test()
        {
            Strings.ToInt(null, true);
        }

        /// <summary>
        ///A test for ToInt
        ///</summary>
        [TestMethod()]
        [Description("ToInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToInt("", true) did not pass")]
        public void ToInt_obj_bool_EmptyString_Throw_Test()
        {
            Strings.ToInt("", true);
        }

        /// <summary>
        ///A test for ToInt
        ///</summary>
        [TestMethod()]
        [Description("ToInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToInt(""chicken"", true) did not pass")]
        public void ToInt_obj_bool_Chicken_Throw_Test()
        {
            Strings.ToInt("chicken", true);
        }

        /// <summary>
        ///A test for ToInt
        ///</summary>
        [TestMethod()]
        [Description("ToInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToInt(""4.753"", true) did not pass")]
        public void ToInt_obj_bool_Float_Throw_Test()
        {
            Strings.ToInt("4.753", true);
        }

        /// <summary>
        ///A test for ToInt
        ///</summary>
        [TestMethod()]
        [Description("ToInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToInt(""2147483648"", true) did not pass")]
        public void ToInt_obj_bool_MaxIntPlus_Throw_Test()
        {
            Strings.ToInt("2147483648", true);
        }

        #endregion

        #region ToGuid

        /// <summary>
        ///A test for ToGuid
        ///</summary>
        [TestMethod()]
        [Description("ToGuid(obj)")]
        public void ToGuid_obj_Test()
        {
            Assert.AreEqual(Guid.Empty, Strings.ToGuid(null), @"Strings.ToGuid(null) did not pass");
            Assert.AreEqual(Guid.Empty, Strings.ToGuid(""), @"Strings.ToGuid("""") did not pass");
            Assert.AreEqual(Guid.Empty, Strings.ToGuid("chicken"), @"Strings.ToGuid(""chicken"") did not pass");

            Assert.AreEqual(new Guid("89643865-EB30-4BDA-9014-400E8B5B7406"), Strings.ToGuid("89643865-EB30-4BDA-9014-400E8B5B7406"), @"Strings.ToGuid(""89643865-EB30-4BDA-9014-400E8B5B7406"") did not pass");
            Assert.AreEqual(new Guid("89643865-EB30-4BDA-9014-400E8B5B7406"), Strings.ToGuid("  89643865-EB30-4BDA-9014-400E8B5B7406  "), @"Strings.ToGuid(""  89643865-EB30-4BDA-9014-400E8B5B7406  "") did not pass");
            //probably should check {'fff'} format...
        }

        /// <summary>
        ///A test for ToGuid
        ///</summary>
        [TestMethod()]
        [Description("ToGuid(obj, guid)")]
        public void ToGuid_obj_guid_Test()
        {
            Assert.AreEqual(new Guid("89643865-EB30-4BDA-9014-400E8B5B7406"), Strings.ToGuid(null, new Guid("89643865-EB30-4BDA-9014-400E8B5B7406")), @"Strings.ToGuid(null, new Guid(""89643865-EB30-4BDA-9014-400E8B5B7406"")) did not pass");
            Assert.AreEqual(new Guid("89643865-EB30-4BDA-9014-400E8B5B7406"), Strings.ToGuid("", new Guid("89643865-EB30-4BDA-9014-400E8B5B7406")), @"Strings.ToGuid("""", new Guid(""89643865-EB30-4BDA-9014-400E8B5B7406"")) did not pass");
            Assert.AreEqual(new Guid("89643865-EB30-4BDA-9014-400E8B5B7406"), Strings.ToGuid("chicken", new Guid("89643865-EB30-4BDA-9014-400E8B5B7406")), @"Strings.ToGuid(""chicken"", new Guid(""89643865-EB30-4BDA-9014-400E8B5B7406"")) did not pass");

            Assert.AreEqual(new Guid("89643865-EEEE-4BDA-9014-400E8B5B7406"), Strings.ToGuid("89643865-EEEE-4BDA-9014-400E8B5B7406", new Guid("89643865-EB30-4BDA-9014-400E8B5B7406")), @"Strings.ToGuid(""89643865-EEEE-4BDA-9014-400E8B5B7406"", new Guid(""89643865-EB30-4BDA-9014-400E8B5B7406"")) did not pass");
            Assert.AreEqual(new Guid("89643865-EEEE-4BDA-9014-400E8B5B7406"), Strings.ToGuid("  89643865-EEEE-4BDA-9014-400E8B5B7406  ", new Guid("89643865-EB30-4BDA-9014-400E8B5B7406")), @"Strings.ToGuid(""  89643865-EEEE-4BDA-9014-400E8B5B7406  "", new Guid(""89643865-EB30-4BDA-9014-400E8B5B7406"")) did not pass");
            //probably should check {'fff'} format...
        }

        /// <summary>
        ///A test for ToGuid
        ///</summary>
        [TestMethod()]
        [Description("ToGuid(obj, bool)")]
        public void ToGuid_obj_bool_Test()
        {
            Assert.AreEqual(Guid.Empty, Strings.ToGuid(null, false), @"Strings.ToGuid(null, false) did not pass");
            Assert.AreEqual(Guid.Empty, Strings.ToGuid("", false), @"Strings.ToGuid("""", false) did not pass");
            Assert.AreEqual(Guid.Empty, Strings.ToGuid("chicken", false), @"Strings.ToGuid(""chicken"", false) did not pass");

            Assert.AreEqual(new Guid("89643865-EB30-4BDA-9014-400E8B5B7406"), Strings.ToGuid("89643865-EB30-4BDA-9014-400E8B5B7406", false), @"Strings.ToGuid(""89643865-EB30-4BDA-9014-400E8B5B7406"", false) did not pass");
            Assert.AreEqual(new Guid("89643865-EB30-4BDA-9014-400E8B5B7406"), Strings.ToGuid("  89643865-EB30-4BDA-9014-400E8B5B7406  ", false), @"Strings.ToGuid(""  89643865-EB30-4BDA-9014-400E8B5B7406  "", false) did not pass");

            Assert.AreEqual(new Guid("89643865-EEEE-4BDA-9014-400E8B5B7406"), Strings.ToGuid("89643865-EEEE-4BDA-9014-400E8B5B7406", true), @"Strings.ToGuid(""89643865-EEEE-4BDA-9014-400E8B5B7406"", true) did not pass");
            Assert.AreEqual(new Guid("89643865-EEEE-4BDA-9014-400E8B5B7406"), Strings.ToGuid("  89643865-EEEE-4BDA-9014-400E8B5B7406  ", true), @"Strings.ToGuid(""  89643865-EEEE-4BDA-9014-400E8B5B7406  "", true) did not pass");
            //probably should check {'fff'} format...
        }

        /// <summary>
        ///A test for ToGuid
        ///</summary>
        [TestMethod()]
        [Description("ToGuid(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToGuid(null, true) did not pass")]
        public void ToGuid_obj_bool_NULL_Throw_Test()
        {
            Strings.ToGuid(null, true);
        }

        /// <summary>
        ///A test for ToGuid
        ///</summary>
        [TestMethod()]
        [Description("ToGuid(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToGuid("""", true) did not pass")]
        public void ToGuid_obj_bool_EmptyString_Throw_Test()
        {
            Strings.ToGuid("", true);
        }

        /// <summary>
        ///A test for ToGuid
        ///</summary>
        [TestMethod()]
        [Description("ToGuid(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToGuid(""chicken"", true) did not pass")]
        public void ToGuid_obj_bool_Chicken_Throw_Test()
        {
            Strings.ToGuid("chicken", true);
        }

        #endregion

        #region ToLong

        /// <summary>
        ///A test for ToLong
        ///</summary>
        [TestMethod()]
        [Description("ToLong(obj)")]
        public void ToLong_obj_Test()
        {
            Assert.AreEqual(-1, Strings.ToLong(null), @"Strings.ToLong(null) did not pass");
            Assert.AreEqual(-1, Strings.ToLong(""), @"Strings.ToLong("""") did not pass");
            Assert.AreEqual(-1, Strings.ToLong("4.735"), @"Strings.ToLong(""4.735"") did not pass");
            Assert.AreEqual(-1, Strings.ToLong("chicken"), @"Strings.ToLong(""chicken"") did not pass");
            
            Assert.AreEqual(5, Strings.ToLong("5"), @"Strings.ToLong(""5"") did not pass");
            Assert.AreEqual(5, Strings.ToLong("  5  "), @"Strings.ToLong(""  5  "") did not pass");
            Assert.AreEqual(0, Strings.ToLong("0"), @"Strings.ToLong(""0"") did not pass");
            Assert.AreEqual(0, Strings.ToLong("  0  "), @"Strings.ToLong(""  0  "") did not pass");
            Assert.AreEqual(-5, Strings.ToLong("-5"), @"Strings.ToLong(""-5"") did not pass");
            Assert.AreEqual(-5, Strings.ToLong("  -5  "), @"Strings.ToLong(""  -5  "") did not pass");
            Assert.AreEqual(2147483648, Strings.ToLong("2147483648"), @"Strings.ToLong(""2147483648"") did not pass"); //maxint +1
        }

        /// <summary>
        ///A test for ToLong
        ///</summary>
        [TestMethod()]
        [Description("ToLong(obj, long)")]
        public void ToLong_obj_long_Test()
        {
            Assert.AreEqual(5, Strings.ToLong(null, 5), @"Strings.ToLong(null, 5) did not pass");
            Assert.AreEqual(5, Strings.ToLong("", 5), @"Strings.ToLong("""", 5) did not pass");
            Assert.AreEqual(5, Strings.ToLong("4.735", 5), @"Strings.ToLong(""4.735"", 5) did not pass");
            Assert.AreEqual(5, Strings.ToLong("chicken", 5), @"Strings.ToLong(""chicken"", 5) did not pass");

            Assert.AreEqual(6, Strings.ToLong("6", 5), @"Strings.ToLong(""6"", 5) did not pass");
            Assert.AreEqual(6, Strings.ToLong("  6  ", 5), @"Strings.ToLong(""  6  "", 5) did not pass");
            Assert.AreEqual(0, Strings.ToLong("0", 5), @"Strings.ToLong(""0"", 5) did not pass");
            Assert.AreEqual(0, Strings.ToLong("  0  ", 5), @"Strings.ToLong(""  0  "", 5) did not pass");
            Assert.AreEqual(-5, Strings.ToLong("-5", 5), @"Strings.ToLong(""-5"", 5) did not pass");
            Assert.AreEqual(-5, Strings.ToLong("  -5  ", 5), @"Strings.ToLong(""  -5  "", 5) did not pass");
            Assert.AreEqual(2147483648, Strings.ToLong("2147483648", 5), @"Strings.ToLong(""2147483648"", 5) did not pass"); //maxint +1
        }

        /// <summary>
        ///A test for ToLong
        ///</summary>
        [TestMethod()]
        [Description("ToLong(obj, bool)")]
        public void ToLong_obj_bool_Test()
        {
            Assert.AreEqual(-1, Strings.ToLong(null, false), @"Strings.ToLong(null, false) did not pass");
            Assert.AreEqual(-1, Strings.ToLong("", false), @"Strings.ToLong("""", false) did not pass");
            Assert.AreEqual(-1, Strings.ToLong("4.735", false), @"Strings.ToLong(""4.735"", false) did not pass");
            Assert.AreEqual(-1, Strings.ToLong("chicken", false), @"Strings.ToLong(""chicken"", false) did not pass");

            Assert.AreEqual(5, Strings.ToLong("5", false), @"Strings.ToLong(""5"", false) did not pass");
            Assert.AreEqual(5, Strings.ToLong("  5  ", false), @"Strings.ToLong(""  5  "", false) did not pass");
            Assert.AreEqual(0, Strings.ToLong("0", false), @"Strings.ToLong(""0"", false) did not pass");
            Assert.AreEqual(0, Strings.ToLong("  0  ", false), @"Strings.ToLong(""  0  "", false) did not pass");
            Assert.AreEqual(-5, Strings.ToLong("-5", false), @"Strings.ToLong(""-5"", false) did not pass");
            Assert.AreEqual(-5, Strings.ToLong("  -5  ", false), @"Strings.ToLong(""  -5  "", false) did not pass");
            Assert.AreEqual(2147483648, Strings.ToLong("2147483648", false), @"Strings.ToLong(""2147483648"", false) did not pass"); //maxint +1

            Assert.AreEqual(6, Strings.ToLong("6", true), @"Strings.ToLong(""6"", true) did not pass");
            Assert.AreEqual(6, Strings.ToLong("  6  ", true), @"Strings.ToLong(""  6  "", true) did not pass");
            Assert.AreEqual(0, Strings.ToLong("0", true), @"Strings.ToLong(""0"", true) did not pass");
            Assert.AreEqual(0, Strings.ToLong("  0  ", true), @"Strings.ToLong(""  0  "", true) did not pass");
            Assert.AreEqual(-5, Strings.ToLong("-5", true), @"Strings.ToLong(""-5"", true) did not pass");
            Assert.AreEqual(-5, Strings.ToLong("  -5  ", true), @"Strings.ToLong(""  -5  "", true) did not pass");
            Assert.AreEqual(2147483648, Strings.ToLong("2147483648", true), @"Strings.ToLong(""2147483648"", true) did not pass"); //maxint +1
        }

        /// <summary>
        ///A test for ToLong
        ///</summary>
        [TestMethod()]
        [Description("ToLong(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLong(null, true) did not pass")]
        public void ToLong_obj_bool_NULL_Throw_Test()
        {
            Strings.ToLong(null, true);
        }

        /// <summary>
        ///A test for ToLong
        ///</summary>
        [TestMethod()]
        [Description("ToLong(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLong("", true) did not pass")]
        public void ToLong_obj_bool_EmptyString_Throw_Test()
        {
            Strings.ToLong("", true);
        }

        /// <summary>
        ///A test for ToLong
        ///</summary>
        [TestMethod()]
        [Description("ToLong(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLong(""chicken"", true) did not pass")]
        public void ToLong_obj_bool_Chicken_Throw_Test()
        {
            Strings.ToLong("chicken", true);
        }

        /// <summary>
        ///A test for ToLong
        ///</summary>
        [TestMethod()]
        [Description("ToLong(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLong(""4.753"", true) did not pass")]
        public void ToLong_obj_bool_Float_Throw_Test()
        {
            Strings.ToLong("4.753", true);
        }

        #endregion

        #region ToUInt

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj)")]
        public void ToUInt_obj_Test()
        {
            Assert.AreEqual<uint>(0, Strings.ToUInt(null), @"Strings.ToUInt(null) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt(""), @"Strings.ToUInt("""") did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("4.735"), @"Strings.ToUInt(""4.735"") did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("chicken"), @"Strings.ToUInt(""chicken"") did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("4294967296"), @"Strings.ToUInt(""4294967296"") did not pass"); //maxuint +1
            Assert.AreEqual<uint>(0, Strings.ToUInt("-5"), @"Strings.ToUInt(""-5"") did not pass");
            
            Assert.AreEqual<uint>(5, Strings.ToUInt("5"), @"Strings.ToUInt(""5"") did not pass");
            Assert.AreEqual<uint>(5, Strings.ToUInt("  5  "), @"Strings.ToUInt(""  5  "") did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("0"), @"Strings.ToUInt(""0"") did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("  0  "), @"Strings.ToUInt(""  0  "") did not pass");
        }

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj, uint)")]
        public void ToUInt_obj_uint_Test()
        {
            Assert.AreEqual<uint>(5, Strings.ToUInt(null, 5), @"Strings.ToUInt(null, 5) did not pass");
            Assert.AreEqual<uint>(5, Strings.ToUInt("", 5), @"Strings.ToUInt("""", 5) did not pass");
            Assert.AreEqual<uint>(5, Strings.ToUInt("4.735", 5), @"Strings.ToUInt(""4.735"", 5) did not pass");
            Assert.AreEqual<uint>(5, Strings.ToUInt("chicken", 5), @"Strings.ToUInt(""chicken"", 5) did not pass");
            Assert.AreEqual<uint>(5, Strings.ToUInt("-5", 5), @"Strings.ToUInt(""-5"", 5) did not pass");
            Assert.AreEqual<uint>(5, Strings.ToUInt("4294967296", 5), @"Strings.ToUInt(""4294967296"", 5) did not pass"); //maxuint +1

            Assert.AreEqual<uint>(6, Strings.ToUInt("6", 5), @"Strings.ToUInt(""6"", 5) did not pass");
            Assert.AreEqual<uint>(6, Strings.ToUInt("  6  ", 5), @"Strings.ToUInt(""  6  "", 5) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("0", 5), @"Strings.ToUInt(""0"", 5) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("  0  ", 5), @"Strings.ToUInt(""  0  "", 5) did not pass");
        }

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj, bool)")]
        public void ToUInt_obj_bool_Test()
        {
            Assert.AreEqual<uint>(0, Strings.ToUInt(null, false), @"Strings.ToUInt(null, false) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("", false), @"Strings.ToUInt("""", false) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("4.735", false), @"Strings.ToUInt(""4.735"", false) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("chicken", false), @"Strings.ToUInt(""chicken"", false) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("4294967296", false), @"Strings.ToUInt(""4294967296"", false) did not pass"); //maxuint +1
            Assert.AreEqual<uint>(0, Strings.ToUInt("-5", false), @"Strings.ToUInt(""-5"", false) did not pass");

            Assert.AreEqual<uint>(5, Strings.ToUInt("5", false), @"Strings.ToUInt(""5"", false) did not pass");
            Assert.AreEqual<uint>(5, Strings.ToUInt("  5  ", false), @"Strings.ToUInt(""  5  "", false) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("0", false), @"Strings.ToUInt(""0"", false) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("  0  ", false), @"Strings.ToUInt(""  0  "", false) did not pass");

            Assert.AreEqual<uint>(6, Strings.ToUInt("6", true), @"Strings.ToUInt(""6"", true) did not pass");
            Assert.AreEqual<uint>(6, Strings.ToUInt("  6  ", true), @"Strings.ToUInt(""  6  "", true) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("0", true), @"Strings.ToUInt(""0"", true) did not pass");
            Assert.AreEqual<uint>(0, Strings.ToUInt("  0  ", true), @"Strings.ToUInt(""  0  "", true) did not pass");
        }

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUInt(null, true) did not pass")]
        public void ToUInt_obj_bool_NULL_Throw_Test()
        {
            Strings.ToUInt(null, true);
        }

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUInt("", true) did not pass")]
        public void ToUInt_obj_bool_EmptyString_Throw_Test()
        {
            Strings.ToUInt("", true);
        }

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUInt(""chicken"", true) did not pass")]
        public void ToUInt_obj_bool_Chicken_Throw_Test()
        {
            Strings.ToUInt("chicken", true);
        }

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUInt(""4.753"", true) did not pass")]
        public void ToUInt_obj_bool_Float_Throw_Test()
        {
            Strings.ToUInt("4.753", true);
        }

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUInt(""4294967296"", true) did not pass")]
        public void ToUInt_obj_bool_MaxIntPlus_Throw_Test()
        {
            Strings.ToUInt("4294967296", true);
        }

        /// <summary>
        ///A test for ToUInt
        ///</summary>
        [TestMethod()]
        [Description("ToUInt(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUInt(""-1"", true) did not pass")]
        public void ToUInt_obj_bool_NegNumber_Throw_Test()
        {
            Strings.ToUInt("-1", true);
        }

        #endregion

        #region ToDateTime

        /// <summary>
        ///A test for ToDateTime
        ///</summary>
        [TestMethod()]
        [Description("ToDateTime(obj)")]
        public void ToDateTime_obj_Test()
        {
            Assert.AreEqual(DateTime.MinValue, Strings.ToDateTime(null), @"Strings.ToDateTime(null) did not pass");
            Assert.AreEqual(DateTime.MinValue, Strings.ToDateTime(""), @"Strings.ToDateTime("""") did not pass");
            Assert.AreEqual(DateTime.MinValue, Strings.ToDateTime("chicken"), @"Strings.ToDateTime(""chicken"") did not pass");

            Assert.AreEqual(Convert.ToDateTime("01/01/2007"), Strings.ToDateTime("01/01/2007"), @"Strings.ToDateTime(""01/01/2007"") did not pass");
            Assert.AreEqual(Convert.ToDateTime("01/01/2007"), Strings.ToDateTime(" January 1, 2007  "), @"Strings.ToDateTime("" January 1, 2007  "") did not pass");
        }

        /// <summary>
        ///A test for ToDateTime
        ///</summary>
        [TestMethod()]
        [Description("ToDateTime(obj, datetime)")]
        public void ToDateTime_obj_datetime_Test()
        {
            Assert.AreEqual(Convert.ToDateTime("01/01/2700"), Strings.ToDateTime(null, Convert.ToDateTime("01/01/2700")), @"Strings.ToDateTime(null, Convert.ToDateTime(""01/01/2700"")) did not pass");
            Assert.AreEqual(Convert.ToDateTime("01/01/2700"), Strings.ToDateTime("", Convert.ToDateTime("01/01/2700")), @"Strings.ToDateTime("""", Convert.ToDateTime(""01/01/2700"")) did not pass");
            Assert.AreEqual(Convert.ToDateTime("01/01/2700"), Strings.ToDateTime("chicken", Convert.ToDateTime("01/01/2700")), @"Strings.ToDateTime(""chicken"", Convert.ToDateTime(""01/01/2700"")) did not pass");

            Assert.AreEqual(Convert.ToDateTime("01/01/2007"), Strings.ToDateTime("01/01/2007", Convert.ToDateTime("01/01/2700")), @"Strings.ToDateTime(""01/01/2007"", Convert.ToDateTime(""01/01/2700"")) did not pass");
            Assert.AreEqual(Convert.ToDateTime("01/01/2007"), Strings.ToDateTime(" January 1, 2007  ", Convert.ToDateTime("01/01/2700")), @"Strings.ToDateTime("" January 1, 2007  "", Convert.ToDateTime(""01/01/2700"")) did not pass");
        }

        /// <summary>
        ///A test for ToDateTime
        ///</summary>
        [TestMethod()]
        [Description("ToDateTime(obj, bool)")]
        public void ToDateTime_obj_bool_Test()
        {
            Assert.AreEqual(DateTime.MinValue, Strings.ToDateTime(null, false), @"Strings.ToDateTime(null, false) did not pass");
            Assert.AreEqual(DateTime.MinValue, Strings.ToDateTime("", false), @"Strings.ToDateTime("""", false) did not pass");
            Assert.AreEqual(DateTime.MinValue, Strings.ToDateTime("chicken", false), @"Strings.ToDateTime(""chicken"", false) did not pass");

            Assert.AreEqual(Convert.ToDateTime("01/01/2007"), Strings.ToDateTime("01/01/2007", false), @"Strings.ToDateTime(""01/01/2007"", false) did not pass");
            Assert.AreEqual(Convert.ToDateTime("01/01/2007"), Strings.ToDateTime(" January 1, 2007  ", false), @"Strings.ToDateTime("" January 1, 2007  "", false) did not pass");

            Assert.AreEqual(Convert.ToDateTime("01/01/2007"), Strings.ToDateTime("01/01/2007", true), @"Strings.ToDateTime(""01/01/2007"", true) did not pass");
            Assert.AreEqual(Convert.ToDateTime("01/01/2007"), Strings.ToDateTime(" January 1, 2007  ", true), @"Strings.ToDateTime("" January 1, 2007  "", true) did not pass");

        }

        /// <summary>
        ///A test for ToDateTime
        ///</summary>
        [TestMethod()]
        [Description("ToDateTime(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToDateTime(null, true) did not pass")]
        public void ToDateTime_obj_bool_NULL_Throw_Test()
        {
            Strings.ToDateTime(null, true);
        }

        /// <summary>
        ///A test for ToDateTime
        ///</summary>
        [TestMethod()]
        [Description("ToDateTime(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToDateTime("""", true) did not pass")]
        public void ToDateTime_obj_bool_EmptyString_Throw_Test()
        {
            Strings.ToDateTime("", true);
        }

        /// <summary>
        ///A test for ToDateTime
        ///</summary>
        [TestMethod()]
        [Description("ToDateTime(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToDateTime(""chicken"", true) did not pass")]
        public void ToDateTime_obj_bool_Chicken_Throw_Test()
        {
            Strings.ToDateTime("chicken", true);
        }

        #endregion

        #endregion

        #region ToArrayTests

        #region ToStringArray

        /// <summary>
        ///A test for ToStringArray
        ///</summary>
        [TestMethod()]
        [Description("ToStringArray(obj)")]
        public void ToStringArray_obj_Test()
        {
            string[] starr1 = { "one", "two", "three", "four" };
            string[] starr2 = { "one" };

            CollectionAssert.AreEqual(null, Strings.ToStringArray(null), @"Strings.ToStringArray(null) did not pass.");
            CollectionAssert.AreEqual(new string[] { }, Strings.ToStringArray(",,"), @"Strings.ToStringArray("",,"") did not pass.");
            CollectionAssert.AreEqual(new string[] { }, Strings.ToStringArray(""), @"Strings.ToStringArray("""") did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray(String.Join(",", starr1)), @"Strings.ToStringArray(" + String.Join(",", starr1) + ") did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray("one,  two,     three,  four"), @"Strings.ToStringArray(""one,  two,     three,  four"") did not pass.");
            CollectionAssert.AreEqual(starr2, Strings.ToStringArray("  one  "), @"Strings.ToStringArray(""  one  "") did not pass.");
        }

        /// <summary>
        ///A test for ToStringArray
        ///</summary>
        [TestMethod()]
        [Description("ToStringArray(obj, str)")]
        public void ToStringArray_obj_str_Test()
        {
            string[] starr1 = { "one", "two", "three", "four" };
            string[] starr2 = { "one" };

            CollectionAssert.AreEqual(null, Strings.ToStringArray(null, "|"), @"Strings.ToStringArray(null, ""|"") did not pass.");
            CollectionAssert.AreEqual(new string[] { ",," }, Strings.ToStringArray(",,", "|"), @"Strings.ToStringArray("",,"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new string[] { }, Strings.ToStringArray("", "|"), @"Strings.ToStringArray("""", ""|"") did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray(String.Join("|", starr1), "|"), @"Strings.ToStringArray(" + String.Join("|", starr1) + @", ""|"") did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray("one|  two|     three |  four", "|"), @"Strings.ToStringArray(""one|  two|     three |  four"", ""|"") did not pass.");
            CollectionAssert.AreEqual(starr2, Strings.ToStringArray("  one  ", "|"), @"Strings.ToStringArray(""  one  "", ""|"") did not pass.");
        }

        /// <summary>
        ///A test for ToStringArray
        ///</summary>
        [TestMethod()]
        [Description("ToStringArray(obj, bool)")] //here
        public void ToStringArray_obj_bool_Test()
        {
            string[] starr1 = { "one", "two", "three", "four" };
            string[] starr2 = { "one" };
            
            CollectionAssert.AreEqual(new string[] { }, Strings.ToStringArray(",,", true), @"Strings.ToStringArray("",,"", true) did not pass.");
            CollectionAssert.AreEqual(new string[] { }, Strings.ToStringArray("", "|"), @"Strings.ToStringArray("""", true) did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray(String.Join(",", starr1), true), @"Strings.ToStringArray(" + String.Join("|", starr1) + @", true) did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray("one,  two,     three,  four", true), @"Strings.ToStringArray(""one,  two,     three,  four"", true) did not pass.");
            CollectionAssert.AreEqual(starr2, Strings.ToStringArray("  one  ", true), @"Strings.ToStringArray(""  one  "", true) did not pass.");
        }

        /// <summary>
        ///A test for ToStringArray
        ///</summary>
        [TestMethod()]
        [Description("ToStringArray(obj, str, bool)")] //here
        public void ToStringArray_obj_str_bool_Test()
        {
            string[] starr1 = { "one", "two", "three", "four" };
            string[] starr2 = { "one" };

            CollectionAssert.AreEqual(null, Strings.ToStringArray(null, "|", false), @"Strings.ToStringArray(null, ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new string[] { ",," }, Strings.ToStringArray(",,", "|", false), @"Strings.ToStringArray("",,"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new string[] { }, Strings.ToStringArray("", "|", false), @"Strings.ToStringArray("""", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray(String.Join("|", starr1), "|", false), @"Strings.ToStringArray(" + String.Join("|", starr1) + @", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray("one|  two|     three |  four", "|", false), @"Strings.ToStringArray(""one|  two|     three |  four"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(starr2, Strings.ToStringArray("  one  ", "|", false), @"Strings.ToStringArray(""  one  "", ""|"", false) did not pass.");

            CollectionAssert.AreEqual(new string[] { ",," }, Strings.ToStringArray(",,", "|", true), @"Strings.ToStringArray("",,"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new string[] { }, Strings.ToStringArray("", "|", true), @"Strings.ToStringArray("""", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray(String.Join("|", starr1), "|", true), @"Strings.ToStringArray(" + String.Join("|", starr1) + @", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(starr1, Strings.ToStringArray("one|  two|     three |  four", "|", true), @"Strings.ToStringArray(""one|  two|     three |  four"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(starr2, Strings.ToStringArray("  one  ", "|", true), @"Strings.ToStringArray(""  one  "", ""|"", true) did not pass.");

        }

        /// <summary>
        ///A test for ToStringArray
        ///</summary>
        [TestMethod()]
        [Description("ToStringArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToStringArray(null, true) did not pass.")]
        public void ToStringArray_obj_bool_NULL_Throw_Test()
        {
            Strings.ToStringArray(null, true);
        }

        /// <summary>
        ///A test for ToStringArray
        ///</summary>
        [TestMethod()]
        [Description("ToStringArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToStringArray(null, ""|"", true) did not pass.")]
        public void ToStringArray_obj_str_bool_NULL_Throw_Test()
        {
            Strings.ToStringArray(null, "|", true);
        }

        #endregion

        #region ToIntArray

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj)")]
        public void ToIntArray_obj_Test()
        {            
            CollectionAssert.AreEqual(null, Strings.ToIntArray(null), @"Strings.ToIntArray(null) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray(""), @"Strings.ToIntArray("""") did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray(",,"), @"Strings.ToIntArray("",,"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray("1"), @"Strings.ToIntArray(""1"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(" 1 "), @"Strings.ToIntArray("" 1 "") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("1,2,3"), @"Strings.ToIntArray(""1,2,3"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("  1,  2,    3 "), @"Strings.ToIntArray(""  1  , 2  , 3"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray("1,chicken,3"), @"Strings.ToIntArray(""1,chicken,3"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 3 }, Strings.ToIntArray("1,,3"), @"Strings.ToIntArray(""1,,3"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray("1,2147483648,3"), @"Strings.ToIntArray(""1,2147483648,3"") did not pass.");
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, bool)")]
        public void ToIntArray_obj_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToIntArray(null, false), @"Strings.ToIntArray(null, false) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray("", false), @"Strings.ToIntArray("""", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray(",,", false), @"Strings.ToIntArray("",,"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray("1", false), @"Strings.ToIntArray(""1"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(" 1 ", false), @"Strings.ToIntArray("" 1 "", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("1,2,3", false), @"Strings.ToIntArray(""1,2,3"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("  1,  2,    3 ", false), @"Strings.ToIntArray(""  1  , 2  , 3"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray("1,chicken,3", false), @"Strings.ToIntArray(""1,chicken,3"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 3 }, Strings.ToIntArray("1,,3", false), @"Strings.ToIntArray(""1,,3"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray("1,2147483648,3", false), @"Strings.ToIntArray(""1,2147483648,3"", false) did not pass.");

            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray("", true), @"Strings.ToIntArray("""", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray(",,", true), @"Strings.ToIntArray("",,"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray("1", true), @"Strings.ToIntArray(""1"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(" 1 ", true), @"Strings.ToIntArray("" 1 "", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("1,2,3", true), @"Strings.ToIntArray(""1,2,3"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("  1,  2,    3 ", true), @"Strings.ToIntArray(""  1  , 2  , 3"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 3 }, Strings.ToIntArray("1,,3", true), @"Strings.ToIntArray(""1,,3"", true) did not pass.");
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(null, true) did not pass.")]
        public void ToIntArray_obj_bool_NULL_Throw_Test()
        {
            Strings.ToIntArray(null, true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(""1,chicken,3"", true) did not pass.")]
        public void ToIntArray_obj_bool_ContainsChicken_Throw_Test()
        {
            Strings.ToIntArray("1,chicken,3", true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(""1,2147483648,3"", true) did not pass.")]
        public void ToIntArray_obj_bool_ContainsMaxInt_Throw_Test()
        {
            Strings.ToIntArray("1,2147483648,3", true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, str)")]
        public void ToIntArray_obj_str_Test()
        {            
            CollectionAssert.AreEqual(null, Strings.ToIntArray(null, "|"), @"Strings.ToIntArray(null, ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray("", "|"), @"Strings.ToIntArray("""", ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray("||", "|"), @"Strings.ToIntArray(""||"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray("1", "|"), @"Strings.ToIntArray(""1"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(" 1 ", "|"), @"Strings.ToIntArray("" 1 "", ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("1|2|3", "|"), @"Strings.ToIntArray(""1|2|3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("  1|  2|    3 ", "|"), @"Strings.ToIntArray(""  1  | 2  | 3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray("1|chicken|3", "|"), @"Strings.ToIntArray(""1|chicken|3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 3 }, Strings.ToIntArray("1||3", "|"), @"Strings.ToIntArray(""1||3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray("1|2147483648|3", "|"), @"Strings.ToIntArray(""1|2147483648|3"", ""|"") did not pass.");
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, str, bool)")]
        public void ToIntArray_obj_str_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToIntArray(null, "|", false), @"Strings.ToIntArray(null, ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray("", "|", false), @"Strings.ToIntArray("""", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray("||", "|", false), @"Strings.ToIntArray(""||"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray("1", "|", false), @"Strings.ToIntArray(""1"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(" 1 ", "|", false), @"Strings.ToIntArray("" 1 "", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("1|2|3", "|", false), @"Strings.ToIntArray(""1|2|3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("  1|  2|    3 ", "|", false), @"Strings.ToIntArray(""  1  | 2  | 3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray("1|chicken|3", "|", false), @"Strings.ToIntArray(""1|chicken|3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 3 }, Strings.ToIntArray("1||3", "|", false), @"Strings.ToIntArray(""1||3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray("1|2147483648|3", "|", false), @"Strings.ToIntArray(""1|2147483648|3"", ""|"", false) did not pass.");

            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray("", "|", true), @"Strings.ToIntArray("""", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray("||", "|", true), @"Strings.ToIntArray(""||"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray("1", "|", true), @"Strings.ToIntArray(""1"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(" 1 ", "|", true), @"Strings.ToIntArray("" 1 "", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("1|2|3", "|", true), @"Strings.ToIntArray(""1|2|3"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray("  1|  2|    3 ", "|", true), @"Strings.ToIntArray(""  1  | 2  | 3"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 3 }, Strings.ToIntArray("1||3", "|", true), @"Strings.ToIntArray(""1||3"", ""|"", true) did not pass.");

        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(null, ""|"", true) did not pass.")]
        public void ToIntArray_obj_str_bool_NULL_Throw_Test()
        {
            Strings.ToIntArray(null, "|", true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(""1|chicken|3"", ""|"", true) did not pass."") did not pass.")]
        public void ToIntArray_obj_str_bool_ContainsChicken_Throw_Test()
        {
            Strings.ToIntArray("1|chicken|3", "|", true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(""1|2147483648|3"", ""|"", true) did not pass.")]
        public void ToIntArray_obj_str_bool_ContainsMaxInt_Throw_Test()
        {
            Strings.ToIntArray("1|2147483648|3", "|", true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(starr)")]
        public void ToIntArray_starr_Test()
        {            
            CollectionAssert.AreEqual(null, Strings.ToIntArray((string[])null), @"Strings.ToIntArray(null) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray(new string[] { }), @"Strings.ToIntArray(new string[]{}) did not pass.");
            CollectionAssert.AreEqual(new int[] { -1, -1, -1 }, Strings.ToIntArray(new string[] { "", "", "" }), @"Strings.ToIntArray(new string[] {"""", """", """"}) did not pass.");
            CollectionAssert.AreEqual(new int[] { -1, -1, -1 }, Strings.ToIntArray(new string[] { "", null, "" }), @"Strings.ToIntArray(new string[] {"""", null, """"}) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(new string[] {"1"}), @"Strings.ToIntArray(new string[] {""1""}) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray(new string[] {"1","2","3"}), @"Strings.ToIntArray(new string[] {""1"",""2"",""3""}) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray(new string[]{"1","chicken","3"}), @"Strings.ToIntArray(new string[]{""1"",""chicken"",""3""}) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray(new string[] {"1","2147483648","3"}), @"Strings.ToIntArray(new string[] {""1"",""2147483648"",""3""}) did not pass.");
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(starr, bool)")]
        public void ToIntArray_starr_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToIntArray((string[])null, false), @"Strings.ToIntArray(null, false) did not pass.");
            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray(new string[] { }, false), @"Strings.ToIntArray(new string[]{}, false) did not pass.");
            CollectionAssert.AreEqual(new int[] { -1, -1, -1 }, Strings.ToIntArray(new string[] { "", "", "" }, false), @"Strings.ToIntArray(new string[] {"""", """", """"}, false) did not pass.");
            CollectionAssert.AreEqual(new int[] { -1, -1, -1 }, Strings.ToIntArray(new string[] { "", null, "" }, false), @"Strings.ToIntArray(new string[] {"""", null, """"}, false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(new string[] { "1" }, false), @"Strings.ToIntArray(new string[] {""1""}, false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray(new string[] { "1", "2", "3" }, false), @"Strings.ToIntArray(new string[] {""1"",""2"",""3""}, false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray(new string[] { "1", "chicken", "3" }, false), @"Strings.ToIntArray(new string[]{""1"",""chicken"",""3""}, false) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, -1, 3 }, Strings.ToIntArray(new string[] { "1", "2147483648", "3" }, false), @"Strings.ToIntArray(new string[] {""1"",""2147483648"",""3""}, false) did not pass.");

            CollectionAssert.AreEqual(new int[] { }, Strings.ToIntArray(new string[] { }, true), @"Strings.ToIntArray(new string[]{}, true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1 }, Strings.ToIntArray(new string[] { "1" }, true), @"Strings.ToIntArray(new string[] {""1""}, true) did not pass.");
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, Strings.ToIntArray(new string[] { "1", "2", "3" }, true), @"Strings.ToIntArray(new string[] {""1"",""2"",""3""}, true) did not pass.");
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(null, true) did not pass.")]
        public void ToIntArray_starr_bool_NULL_Throw_Test()
        {
            Strings.ToIntArray((string[])null, true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(new string[] {"""", """", """"}, true) did not pass.")]
        public void ToIntArray_starr_bool_ArrayEmptyStr_Throw_Test()
        {
            Strings.ToIntArray(new string[] { "", "", "" }, true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(new string[] {"""", null, """"}, true) did not pass.")]
        public void ToIntArray_starr_bool_ArrayContainsNull_Throw_Test()
        {
            Strings.ToIntArray(new string[] { "", null, "" }, true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(new string[]{""1"",""chicken"",""3""}, true) did not pass.")]
        public void ToIntArray_starr_bool_ArrayContainsChicken_Throw_Test()
        {
            Strings.ToIntArray(new string[] { "1", "chicken", "3" }, true);
        }

        /// <summary>
        ///A test for ToIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToIntArray(new string[] {""1"",""2147483648"",""3""}, true) did not pass.")]
        public void ToIntArray_starr_bool_ArrayContainsMaxIntPlus_Throw_Test()
        {
            Strings.ToIntArray(new string[] { "1", "2147483648", "3" }, true);
        }

        #endregion

        #region ToLongArray

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(obj)")]
        public void ToLongArray_obj_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToLongArray(null), @"Strings.ToLongArray(null) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray(""), @"Strings.ToLongArray("""") did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray(",,"), @"Strings.ToLongArray("",,"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray("1"), @"Strings.ToLongArray(""1"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(" 1 "), @"Strings.ToLongArray("" 1 "") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("1,2,3"), @"Strings.ToLongArray(""1,2,3"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("  1,  2,    3 "), @"Strings.ToLongArray(""  1  , 2  , 3"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, -1, 3 }, Strings.ToLongArray("1,chicken,3"), @"Strings.ToLongArray(""1,chicken,3"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 3 }, Strings.ToLongArray("1,,3"), @"Strings.ToLongArray(""1,,3"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray("1,2147483648,3"), @"Strings.ToLongArray(""1,2147483648,3"") did not pass.");
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(obj, bool)")]
        public void ToLongArray_obj_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToLongArray(null, false), @"Strings.ToLongArray(null, false) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray("", false), @"Strings.ToLongArray("""", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray(",,", false), @"Strings.ToLongArray("",,"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray("1", false), @"Strings.ToLongArray(""1"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(" 1 ", false), @"Strings.ToLongArray("" 1 "", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("1,2,3", false), @"Strings.ToLongArray(""1,2,3"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("  1,  2,    3 ", false), @"Strings.ToLongArray(""  1  , 2  , 3"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, -1, 3 }, Strings.ToLongArray("1,chicken,3", false), @"Strings.ToLongArray(""1,chicken,3"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 3 }, Strings.ToLongArray("1,,3", false), @"Strings.ToLongArray(""1,,3"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray("1,2147483648,3", false), @"Strings.ToLongArray(""1,2147483648,3"", false) did not pass.");

            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray("", true), @"Strings.ToLongArray("""", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray(",,", true), @"Strings.ToLongArray("",,"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray("1", true), @"Strings.ToLongArray(""1"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(" 1 ", true), @"Strings.ToLongArray("" 1 "", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("1,2,3", true), @"Strings.ToLongArray(""1,2,3"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("  1,  2,    3 ", true), @"Strings.ToLongArray(""  1  , 2  , 3"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 3 }, Strings.ToLongArray("1,,3", true), @"Strings.ToLongArray(""1,,3"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray("1,2147483648,3", true), @"Strings.ToLongArray(""1,2147483648,3"", true) did not pass.");
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLongArray(null, true) did not pass.")]
        public void ToLongArray_obj_bool_NULL_Throw_Test()
        {
            Strings.ToLongArray(null, true);
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLongArray(""1,chicken,3"", true) did not pass.")]
        public void ToLongArray_obj_bool_ContainsChicken_Throw_Test()
        {
            Strings.ToLongArray("1,chicken,3", true);
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(obj, str)")]
        public void ToLongArray_obj_str_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToLongArray(null, "|"), @"Strings.ToLongArray(null, ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray("", "|"), @"Strings.ToLongArray("""", ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray("||", "|"), @"Strings.ToLongArray(""||"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray("1", "|"), @"Strings.ToLongArray(""1"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(" 1 ", "|"), @"Strings.ToLongArray("" 1 "", ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("1|2|3", "|"), @"Strings.ToLongArray(""1|2|3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("  1|  2|    3 ", "|"), @"Strings.ToLongArray(""  1  | 2  | 3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, -1, 3 }, Strings.ToLongArray("1|chicken|3", "|"), @"Strings.ToLongArray(""1|chicken|3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 3 }, Strings.ToLongArray("1||3", "|"), @"Strings.ToLongArray(""1||3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray("1|2147483648|3", "|"), @"Strings.ToLongArray(""1|2147483648|3"", ""|"") did not pass.");
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(obj, str, bool)")]
        public void ToLongArray_obj_str_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToLongArray(null, "|", false), @"Strings.ToLongArray(null, ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray("", "|", false), @"Strings.ToLongArray("""", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray("||", "|", false), @"Strings.ToLongArray(""||"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray("1", "|", false), @"Strings.ToLongArray(""1"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(" 1 ", "|", false), @"Strings.ToLongArray("" 1 "", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("1|2|3", "|", false), @"Strings.ToLongArray(""1|2|3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("  1|  2|    3 ", "|", false), @"Strings.ToLongArray(""  1  | 2  | 3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, -1, 3 }, Strings.ToLongArray("1|chicken|3", "|", false), @"Strings.ToLongArray(""1|chicken|3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 3 }, Strings.ToLongArray("1||3", "|", false), @"Strings.ToLongArray(""1||3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray("1|2147483648|3", "|", false), @"Strings.ToLongArray(""1|2147483648|3"", ""|"", false) did not pass.");

            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray("", "|", true), @"Strings.ToLongArray("""", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray("||", "|", true), @"Strings.ToLongArray(""||"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray("1", "|", true), @"Strings.ToLongArray(""1"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(" 1 ", "|", true), @"Strings.ToLongArray("" 1 "", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("1|2|3", "|", true), @"Strings.ToLongArray(""1|2|3"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray("  1|  2|    3 ", "|", true), @"Strings.ToLongArray(""  1  | 2  | 3"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 3 }, Strings.ToLongArray("1||3", "|", true), @"Strings.ToLongArray(""1||3"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray("1|2147483648|3", "|", true), @"Strings.ToLongArray(""1|2147483648|3"", ""|"", true) did not pass.");

        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLongArray(null, ""|"", true) did not pass.")]
        public void ToLongArray_obj_str_bool_NULL_Throw_Test()
        {
            Strings.ToLongArray(null, "|", true);
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLongArray(""1|chicken|3"", ""|"", true) did not pass."") did not pass.")]
        public void ToLongArray_obj_str_bool_ContainsChicken_Throw_Test()
        {
            Strings.ToLongArray("1|chicken|3", "|", true);
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(starr)")]
        public void ToLongArray_starr_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToLongArray((string[])null), @"Strings.ToLongArray(null) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray(new string[] { }), @"Strings.ToLongArray(new string[]{}) did not pass.");
            CollectionAssert.AreEqual(new long[] { -1, -1, -1 }, Strings.ToLongArray(new string[] { "", "", "" }), @"Strings.ToLongArray(new string[] {"""", """", """"}) did not pass.");
            CollectionAssert.AreEqual(new long[] { -1, -1, -1 }, Strings.ToLongArray(new string[] { "", null, "" }), @"Strings.ToLongArray(new string[] {"""", null, """"}) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(new string[] { "1" }), @"Strings.ToLongArray(new string[] {""1""}) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray(new string[] { "1", "2", "3" }), @"Strings.ToLongArray(new string[] {""1"",""2"",""3""}) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, -1, 3 }, Strings.ToLongArray(new string[] { "1", "chicken", "3" }), @"Strings.ToLongArray(new string[]{""1"",""chicken"",""3""}) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray(new string[] { "1", "2147483648", "3" }), @"Strings.ToLongArray(new string[] {""1"",""2147483648"",""3""}) did not pass.");
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(starr, bool)")]
        public void ToLongArray_starr_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToLongArray((string[])null, false), @"Strings.ToLongArray(null, false) did not pass.");
            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray(new string[] { }, false), @"Strings.ToLongArray(new string[]{}, false) did not pass.");
            CollectionAssert.AreEqual(new long[] { -1, -1, -1 }, Strings.ToLongArray(new string[] { "", "", "" }, false), @"Strings.ToLongArray(new string[] {"""", """", """"}, false) did not pass.");
            CollectionAssert.AreEqual(new long[] { -1, -1, -1 }, Strings.ToLongArray(new string[] { "", null, "" }, false), @"Strings.ToLongArray(new string[] {"""", null, """"}, false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(new string[] { "1" }, false), @"Strings.ToLongArray(new string[] {""1""}, false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray(new string[] { "1", "2", "3" }, false), @"Strings.ToLongArray(new string[] {""1"",""2"",""3""}, false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, -1, 3 }, Strings.ToLongArray(new string[] { "1", "chicken", "3" }, false), @"Strings.ToLongArray(new string[]{""1"",""chicken"",""3""}, false) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray(new string[] { "1", "2147483648", "3" }, false), @"Strings.ToLongArray(new string[] {""1"",""2147483648"",""3""}, false) did not pass.");

            CollectionAssert.AreEqual(new long[] { }, Strings.ToLongArray(new string[] { }, true), @"Strings.ToLongArray(new string[]{}, true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1 }, Strings.ToLongArray(new string[] { "1" }, true), @"Strings.ToLongArray(new string[] {""1""}, true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, Strings.ToLongArray(new string[] { "1", "2", "3" }, true), @"Strings.ToLongArray(new string[] {""1"",""2"",""3""}, true) did not pass.");
            CollectionAssert.AreEqual(new long[] { 1, 2147483648, 3 }, Strings.ToLongArray(new string[] { "1", "2147483648", "3" }, false), @"Strings.ToLongArray(new string[] {""1"",""2147483648"",""3""}, true) did not pass.");
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLongArray(null, true) did not pass.")]
        public void ToLongArray_starr_bool_NULL_Throw_Test()
        {
            Strings.ToLongArray((string[])null, true);
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLongArray(new string[] {"""", """", """"}, true) did not pass.")]
        public void ToLongArray_starr_bool_ArrayEmptyStr_Throw_Test()
        {
            Strings.ToLongArray(new string[] { "", "", "" }, true);
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLongArray(new string[] {"""", null, """"}, true) did not pass.")]
        public void ToLongArray_starr_bool_ArrayContainsNull_Throw_Test()
        {
            Strings.ToLongArray(new string[] { "", null, "" }, true);
        }

        /// <summary>
        ///A test for ToLongArray
        ///</summary>
        [TestMethod()]
        [Description("ToLongArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToLongArray(new string[]{""1"",""chicken"",""3""}, true) did not pass.")]
        public void ToLongArray_starr_bool_ArrayContainsChicken_Throw_Test()
        {
            Strings.ToLongArray(new string[] { "1", "chicken", "3" }, true);
        }

        #endregion

        #region ToUIntArray

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj)")]
        public void ToUIntArray_obj_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToUIntArray(null), @"Strings.ToUIntArray(null) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray(""), @"Strings.ToUIntArray("""") did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray(",,"), @"Strings.ToUIntArray("",,"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray("1"), @"Strings.ToUIntArray(""1"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(" 1 "), @"Strings.ToUIntArray("" 1 "") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("1,2,3"), @"Strings.ToUIntArray(""1,2,3"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("  1,  2,    3 "), @"Strings.ToUIntArray(""  1  , 2  , 3"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1,chicken,3"), @"Strings.ToUIntArray(""1,chicken,3"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 3 }, Strings.ToUIntArray("1,,3"), @"Strings.ToUIntArray(""1,,3"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1,4294967296,3"), @"Strings.ToUIntArray(""1,4294967296,3"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1,-1,3"), @"Strings.ToUIntArray(""1,-1,3"") did not pass.");

        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, bool)")]
        public void ToUIntArray_obj_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToUIntArray(null, false), @"Strings.ToUIntArray(null, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray("", false), @"Strings.ToUIntArray("""", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray(",,", false), @"Strings.ToUIntArray("",,"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray("1", false), @"Strings.ToUIntArray(""1"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(" 1 ", false), @"Strings.ToUIntArray("" 1 "", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("1,2,3", false), @"Strings.ToUIntArray(""1,2,3"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("  1,  2,    3 ", false), @"Strings.ToUIntArray(""  1  , 2  , 3"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1,chicken,3", false), @"Strings.ToUIntArray(""1,chicken,3"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 3 }, Strings.ToUIntArray("1,,3", false), @"Strings.ToUIntArray(""1,,3"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1,4294967296,3", false), @"Strings.ToUIntArray(""1,4294967296,3"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1,-1,3", false), @"Strings.ToUIntArray(""1,-1,3"", false) did not pass.");

            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray("", true), @"Strings.ToUIntArray("""", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray(",,", true), @"Strings.ToUIntArray("",,"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray("1", true), @"Strings.ToUIntArray(""1"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(" 1 ", true), @"Strings.ToUIntArray("" 1 "", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("1,2,3", true), @"Strings.ToUIntArray(""1,2,3"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("  1,  2,    3 ", true), @"Strings.ToUIntArray(""  1  , 2  , 3"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 3 }, Strings.ToUIntArray("1,,3", true), @"Strings.ToUIntArray(""1,,3"", true) did not pass.");
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(null, true) did not pass.")]
        public void ToUIntArray_obj_bool_NULL_Throw_Test()
        {
            Strings.ToUIntArray(null, true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(""1,chicken,3"", true) did not pass.")]
        public void ToUIntArray_obj_bool_ContainsChicken_Throw_Test()
        {
            Strings.ToUIntArray("1,chicken,3", true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(""1,2147483648,3"", true) did not pass.")]
        public void ToUIntArray_obj_bool_ContainsMaxInt_Throw_Test()
        {
            Strings.ToUIntArray("1,4294967296,3", true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(""1,-1,3"", true) did not pass.")]
        public void ToUIntArray_obj_bool_ContainsNegative_Throw_Test()
        {
            Strings.ToUIntArray("1,-1,3", true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, str)")]
        public void ToUIntArray_obj_str_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToUIntArray(null, "|"), @"Strings.ToUIntArray(null, ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray("", "|"), @"Strings.ToUIntArray("""", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray("||", "|"), @"Strings.ToUIntArray(""||"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray("1", "|"), @"Strings.ToUIntArray(""1"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(" 1 ", "|"), @"Strings.ToUIntArray("" 1 "", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("1|2|3", "|"), @"Strings.ToUIntArray(""1|2|3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("  1|  2|    3 ", "|"), @"Strings.ToUIntArray(""  1  | 2  | 3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1|chicken|3", "|"), @"Strings.ToUIntArray(""1|chicken|3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 3 }, Strings.ToUIntArray("1||3", "|"), @"Strings.ToUIntArray(""1||3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1|4294967296|3", "|"), @"Strings.ToUIntArray(""1|4294967296|3"", ""|"") did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1|-1|3", "|"), @"Strings.ToUIntArray(""1|-1|3"", ""|"") did not pass.");
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, str, bool)")]
        public void ToUIntArray_obj_str_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToUIntArray(null, "|", false), @"Strings.ToUIntArray(null, ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray("", "|", false), @"Strings.ToUIntArray("""", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray("||", "|", false), @"Strings.ToUIntArray(""||"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray("1", "|", false), @"Strings.ToUIntArray(""1"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(" 1 ", "|", false), @"Strings.ToUIntArray("" 1 "", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("1|2|3", "|", false), @"Strings.ToUIntArray(""1|2|3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("  1|  2|    3 ", "|", false), @"Strings.ToUIntArray(""  1  | 2  | 3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1|chicken|3", "|", false), @"Strings.ToUIntArray(""1|chicken|3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 3 }, Strings.ToUIntArray("1||3", "|", false), @"Strings.ToUIntArray(""1||3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1|4294967296|3", "|", false), @"Strings.ToUIntArray(""1|4294967296|3"", ""|"", false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray("1|-1|3", "|", false), @"Strings.ToUIntArray(""1|-1|3"", ""|"", false) did not pass.");

            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray("", "|", true), @"Strings.ToUIntArray("""", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray("||", "|", true), @"Strings.ToUIntArray(""||"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray("1", "|", true), @"Strings.ToUIntArray(""1"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(" 1 ", "|", true), @"Strings.ToUIntArray("" 1 "", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("1|2|3", "|", true), @"Strings.ToUIntArray(""1|2|3"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray("  1|  2|    3 ", "|", true), @"Strings.ToUIntArray(""  1  | 2  | 3"", ""|"", true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 3 }, Strings.ToUIntArray("1||3", "|", true), @"Strings.ToUIntArray(""1||3"", ""|"", true) did not pass.");

        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(null, ""|"", true) did not pass.")]
        public void ToUIntArray_obj_str_bool_NULL_Throw_Test()
        {
            Strings.ToUIntArray(null, "|", true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(""1|chicken|3"", ""|"", true) did not pass."") did not pass.")]
        public void ToUIntArray_obj_str_bool_ContainsChicken_Throw_Test()
        {
            Strings.ToUIntArray("1|chicken|3", "|", true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(""1|4294967296|3"", ""|"", true) did not pass.")]
        public void ToUIntArray_obj_str_bool_ContainsMaxInt_Throw_Test()
        {
            Strings.ToUIntArray("1|4294967296|3", "|", true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(obj, str, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(""1|-1|3"", ""|"", true) did not pass.")]
        public void ToUIntArray_obj_str_bool_ContainsNegative_Throw_Test()
        {
            Strings.ToUIntArray("1|-1|3", "|", true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(starr)")]
        public void ToUIntArray_starr_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToUIntArray((string[])null), @"Strings.ToUIntArray(null) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray(new string[] { }), @"Strings.ToUIntArray(new string[]{}) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 0, 0, 0 }, Strings.ToUIntArray(new string[] { "", "", "" }), @"Strings.ToUIntArray(new string[] {"""", """", """"}) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 0, 0, 0 }, Strings.ToUIntArray(new string[] { "", null, "" }), @"Strings.ToUIntArray(new string[] {"""", null, """"}) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(new string[] { "1" }), @"Strings.ToUIntArray(new string[] {""1""}) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray(new string[] { "1", "2", "3" }), @"Strings.ToUIntArray(new string[] {""1"",""2"",""3""}) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray(new string[] { "1", "chicken", "3" }), @"Strings.ToUIntArray(new string[]{""1"",""chicken"",""3""}) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray(new string[] { "1", "4294967296", "3" }), @"Strings.ToUIntArray(new string[] {""1"",""2147483648"",""3""}) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray(new string[] { "1", "-1", "3" }), @"Strings.ToUIntArray(new string[] {""1"",""-1"",""3""}) did not pass.");
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(starr, bool)")]
        public void ToUIntArray_starr_bool_Test()
        {
            CollectionAssert.AreEqual(null, Strings.ToUIntArray((string[])null, false), @"Strings.ToUIntArray(null, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray(new string[] { }, false), @"Strings.ToUIntArray(new string[]{}, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 0, 0, 0 }, Strings.ToUIntArray(new string[] { "", "", "" }, false), @"Strings.ToUIntArray(new string[] {"""", """", """"}, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 0, 0, 0 }, Strings.ToUIntArray(new string[] { "", null, "" }, false), @"Strings.ToUIntArray(new string[] {"""", null, """"}, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(new string[] { "1" }, false), @"Strings.ToUIntArray(new string[] {""1""}, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray(new string[] { "1", "2", "3" }, false), @"Strings.ToUIntArray(new string[] {""1"",""2"",""3""}, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray(new string[] { "1", "chicken", "3" }, false), @"Strings.ToUIntArray(new string[]{""1"",""chicken"",""3""}, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray(new string[] { "1", "4294967296", "3" }, false), @"Strings.ToUIntArray(new string[] {""1"",""2147483648"",""3""}, false) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 0, 3 }, Strings.ToUIntArray(new string[] { "1", "-1", "3" }, false), @"Strings.ToUIntArray(new string[] {""1"",""-1"",""3""}, false) did not pass.");

            CollectionAssert.AreEqual(new uint[] { }, Strings.ToUIntArray(new string[] { }, true), @"Strings.ToUIntArray(new string[]{}, true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1 }, Strings.ToUIntArray(new string[] { "1" }, true), @"Strings.ToUIntArray(new string[] {""1""}, true) did not pass.");
            CollectionAssert.AreEqual(new uint[] { 1, 2, 3 }, Strings.ToUIntArray(new string[] { "1", "2", "3" }, true), @"Strings.ToUIntArray(new string[] {""1"",""2"",""3""}, true) did not pass.");
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(null, true) did not pass.")]
        public void ToUIntArray_starr_bool_NULL_Throw_Test()
        {
            Strings.ToUIntArray((string[])null, true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(new string[] {"""", """", """"}, true) did not pass.")]
        public void ToUIntArray_starr_bool_ArrayEmptyStr_Throw_Test()
        {
            Strings.ToUIntArray(new string[] { "", "", "" }, true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(new string[] {"""", null, """"}, true) did not pass.")]
        public void ToUIntArray_starr_bool_ArrayContainsNull_Throw_Test()
        {
            Strings.ToUIntArray(new string[] { "", null, "" }, true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(new string[]{""1"",""chicken"",""3""}, true) did not pass.")]
        public void ToUIntArray_starr_bool_ArrayContainsChicken_Throw_Test()
        {
            Strings.ToUIntArray(new string[] { "1", "chicken", "3" }, true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(new string[] {""1"",""2147483648"",""3""}, true) did not pass.")]
        public void ToUIntArray_starr_bool_ArrayContainsMaxIntPlus_Throw_Test()
        {
            Strings.ToUIntArray(new string[] { "1", "4294967296", "3" }, true);
        }

        /// <summary>
        ///A test for ToUIntArray
        ///</summary>
        [TestMethod()]
        [Description("ToUIntArray(starr, bool)")]
        [ExpectedException(typeof(NCIStringConversionFailedException), @"Strings.ToUIntArray(new string[] {""1"",""-1"",""3""}, true) did not pass.")]
        public void ToUIntArray_starr_bool_ArrayContainsNegative_Throw_Test()
        {
            Strings.ToUIntArray(new string[] { "1", "-1", "3" }, true);
        }

        #endregion

        #endregion
    }
}
