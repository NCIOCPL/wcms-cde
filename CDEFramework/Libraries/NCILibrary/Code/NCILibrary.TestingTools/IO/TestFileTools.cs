using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NCI.Test.IO
{
    /// <summary>
    /// Set of test tools for handling loading of test data and resolving test file paths.
    /// </summary>
    public static class TestFileTools
    {
        /// <summary>
        /// Gets a test file from the TestData folder and returns an array of bytes
        /// </summary>
        /// <returns>An array of byte</returns>
        public static byte[] GetTestFileAsBytes(Type testClass, string testFile)
        {

            //Get the path to the file.
            string path = GetPathToTestFile(testClass, testFile);

            //Get the bytes
            return GetTestFileAsBytes(path);
        }

        /// <summary>
        /// Gets a test file from the TestData folder and returns an array of bytes
        /// </summary>
        /// <returns>An array of byte</returns>
        public static byte[] GetTestFileAsBytes(string path)
        {

            //Get the bytes
            byte[] contents = File.ReadAllBytes(path);

            return contents;
        }


        /// <summary>
        /// Gets a test file from the TestData folder as a stream
        /// </summary>
        /// <param name="testFile">The name of the testfile</param>
        /// <returns></returns>
        public static Stream GetTestFileAsStream(Type testClass, string testFile)
        {
            //Get the path to the file.
            string path = GetPathToTestFile(testClass, testFile);

            //Get the bytes
            return GetTestFileAsStream(path);
        }

        /// <summary>
        /// Gets a test file from the TestData folder as a stream
        /// </summary>
        /// <param name="testFile">The name of the testfile</param>
        /// <returns></returns>
        public static Stream GetTestFileAsStream(string path)
        {
            //Get the bytes
            Stream contents = File.OpenRead(path);

            return contents;
        }

        /// <summary>
        /// Gets the path to a test file within the testdata folder.
        /// </summary>
        /// <param name="testFile">The test filename</param>
        /// <returns>The full path to the file.</returns>
        public static string GetPathToTestFile(Type testClass, string testFile)
        {
            Uri codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            string dirPath = Path.GetDirectoryName(codeBasePath);

            // Build a path to the test file
            string path = Path.Combine(new string[] { dirPath, testFile });

            return path;
        }
    }
}
