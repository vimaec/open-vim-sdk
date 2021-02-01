using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vim.DotNetUtilities
{
    public static class HttpRequest
    {
        /// <summary>
        /// Makes an HTTP POST request to the given Uri and serializes the given payload into the content of the request as an "application/json" media type.
        /// </summary>
        public static async Task<HttpResponseMessage> PostJsonAsync(
            this Uri uri,
            object payload,
            IJsonSerializer jsonSerializer,
            CancellationToken token = default)
        {
            using (var httpClient = new HttpClient())
            {
                // Serialize the content
                var payloadWriter = new StringWriter();
                jsonSerializer.Serialize(payloadWriter, payload);
                var content = new StringContent(payloadWriter.ToString(), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(uri, content, token);
                response.EnsureSuccessStatusCode();
                return response;
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(
            this Uri uri,
            CancellationToken token = default)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, token);
                response.EnsureSuccessStatusCode();
                return response;
            }
        }

        public static bool HttpStatusCodeIsSuccess(int statusCode)
        {
            // Make sure the status code exists among the standard HttpStatusCode values.
            if (!Enum.IsDefined(typeof(HttpStatusCode), statusCode))
            {
                return false;
            }

            return statusCode >= 200 && statusCode < 300;
        }
    }
}
