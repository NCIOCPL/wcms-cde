using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using RichardSzalay.MockHttp;

using NCI.Test.IO;

namespace NCI.Test.Net
{



    public static class HttpClientMockHelper
    {
        /// <summary>
        /// Gets a mocked up HttpClient that will return a File's data as a response 
        /// </summary>
        /// <param name="reqUrl">The expected URL path</param>
        /// <param name="pathToResponse">The absolute path to the file to use for the response</param>
        /// <param name="statusCode">The status code to return with (DEFAULT: 200)</param>
        /// <param name="contentType">The mime type of the response (DEFAULT: "application/json")</param>
        /// <returns>HttpClient with a mocked message handler</returns>
        public static HttpClient GetClientMockForURLWithFileResponse(string reqUrl, string pathToResponse, HttpStatusCode statusCode = HttpStatusCode.OK, string contentType = "application/json")
        {
            //The HttpMessageHandler is what HttpClient uses to send & recieve data from a server.
            //The MockHttpMessageHandler allows us to intercept (or inspect) those requests and return mocked data.
            MockHttpMessageHandler mockHandler = new MockHttpMessageHandler();


            //Load content.
            ByteArrayContent content = new ByteArrayContent(TestFileTools.GetTestFileAsBytes(pathToResponse));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            mockHandler
                .When(reqUrl)
                .Respond(statusCode, content);

            return new HttpClient(mockHandler);
        }
    }
}
