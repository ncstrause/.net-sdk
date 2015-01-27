using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;

namespace CTCT.Util
{
    /// <summary>
    /// Class implementation of REST client.
    /// </summary>
    public class RestClient : IRestClient
    {
        private readonly HttpClient client;

        public RestClient()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-ctct-request-source", "sdk.NET." + GetWrapperAssemblyVersion());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ServicePointManager.DefaultConnectionLimit = 48;
        }

        /// <summary>
        /// Make an Http GET request.
        /// </summary>
        /// <param name="url">Request URL.</param>
        /// <param name="accessToken">Constant Contact OAuth2 access token</param>
        /// <param name="apiKey">The API key for the application</param>
        /// <returns>The response body, http info, and error (if one exists).</returns>
        public CUrlResponse Get(string url, string accessToken, string apiKey)
        {
            return SendHttpRequest(url, accessToken, apiKey, string.Empty, HttpMethod.Get, null);
            //return HttpRequest(url, WebRequestMethods.Http.Get, accessToken, apiKey, null, null);
        }

        /// <summary>
        /// Make an Http POST request.
        /// </summary>
        /// <param name="url">Request URL.</param>
        /// <param name="accessToken">Constant Contact OAuth2 access token</param>
        /// <param name="apiKey">The API key for the application</param>
        /// <param name="data">Data to send with request.</param>
        /// <returns>The response body, http info, and error (if one exists).</returns>
        public CUrlResponse Post(string url, string accessToken, string apiKey, string data)
        {
            return SendHttpRequest(url, accessToken, apiKey, data, HttpMethod.Post, null);

            #region old

//            byte[] bytes = null;
//
//			if(!string.IsNullOrEmpty(data))
//			{
//				// Convert the request contents to a byte array
//				bytes = Encoding.UTF8.GetBytes(data);
//			}
//
//            return HttpRequest(url, WebRequestMethods.Http.Post, accessToken, apiKey, bytes, null);

            #endregion
        }

        /// <summary>
        /// Make an Http PATCH request.
        /// </summary>
        /// <param name="url">Request URL.</param>
        /// <param name="accessToken">Constant Contact OAuth2 access token</param>
        /// <param name="apiKey">The API key for the application</param>
        /// <param name="data">Data to send with request.</param>
        /// <returns>The response body, http info, and error (if one exists).</returns>
        public CUrlResponse Patch(string url, string accessToken, string apiKey, string data)
        {
            return SendHttpRequest(url, accessToken, apiKey, data, new HttpMethod("PATCH"), null);

            #region old

            //            byte[] bytes = null;
//
//            if (!string.IsNullOrEmpty(data))
//            {
//                // Convert the request contents to a byte array
//                bytes = Encoding.UTF8.GetBytes(data);
//            }
//
            //            return HttpRequest(url, "PATCH", accessToken, apiKey, bytes, null);

            #endregion
        }

		/// <summary>
		/// Make an HTTP Post Multipart request.
		/// </summary>
		/// <param name="url">Request URL.</param>
        /// <param name="accessToken">Constant Contact OAuth2 access token</param>
        /// <param name="apiKey">The API key for the application</param>
        /// <param name="data">Data to send with request.</param>
        /// <returns>The response body, http info, and error (if one exists).</returns>
		public CUrlResponse PostMultipart(string url, string accessToken, string apiKey, byte[] data)
		{
		    var stringData = Encoding.UTF8.GetString(data);
		    return SendHttpRequest(url, accessToken, apiKey, stringData, HttpMethod.Post ,true);

		    #region old

		    //return HttpRequest(url, WebRequestMethods.Http.Post, accessToken, apiKey, data, true);

		    #endregion
		}

        /// <summary>
        /// Make an Http PUT request.
        /// </summary>
        /// <param name="url">Request URL.</param>
        /// <param name="accessToken">Constant Contact OAuth2 access token</param>
        /// <param name="apiKey">The API key for the application</param>
        /// <param name="data">Data to send with request.</param>
        /// <returns>The response body, http info, and error (if one exists).</returns>
        public CUrlResponse Put(string url, string accessToken, string apiKey, string data)
        {
            return SendHttpRequest(url, accessToken, apiKey, data, HttpMethod.Put, null);

            #region old

            //			byte[] bytes = null;
//
//			if(!string.IsNullOrEmpty(data))
//			{
//				// Convert the request contents to a byte array 
//				bytes = Encoding.UTF8.GetBytes(data);
//			}

            //return HttpRequest(url, WebRequestMethods.Http.Put, accessToken, apiKey, bytes, null);

            #endregion
        }

        /// <summary>
        /// Make an Http DELETE request.
        /// </summary>
        /// <param name="url">Request URL.</param>
        /// <param name="accessToken">Constant Contact OAuth2 access token</param>
        /// <param name="apiKey">The API key for the application</param>
        /// <returns>The response body, http info, and error (if one exists).</returns>
        public CUrlResponse Delete(string url, string accessToken, string apiKey)
        {
            return SendHttpRequest(url, accessToken, apiKey, string.Empty, HttpMethod.Delete, null);
            //return HttpRequest(url, "DELETE", accessToken, apiKey, null, null);
        }

        private CUrlResponse HttpRequest(string url, string method, string accessToken, string apiKey, byte[] data, bool? isMultipart)
        {
            #region old

            //Initialize the response
            HttpWebResponse response = null;
            var urlResponse = new CUrlResponse();

            var address = url;

            if (!string.IsNullOrEmpty(apiKey))
            {
                address = string.Format("{0}{1}api_key={2}", url, url.Contains("?") ? "&" : "?", apiKey);
            }

            HttpWebRequest request;

            try
            {
                request = WebRequest.Create(address) as HttpWebRequest;
                
                if (request == null)
                {
                    throw new Exception("Failed to create web request.");
                }

                request.Timeout = 500000;

            }
            catch (Exception e)
            {
                throw new Exception("Failed to create web request.", e);
            }
                                                                                             
            request.Method = method;
			request.Accept = "application/json";
            request.Headers["x-ctct-request-source"] = "sdk.NET." + GetWrapperAssemblyVersion();

			if(isMultipart.HasValue && isMultipart.Value)
			{
				request.ContentType = "multipart/form-data; boundary=" + MultipartBuilder.MULTIPART_BOUNDARY;
			}
			else
			{
				request.ContentType = "application/json";			
			}
          
            // Add token as HTTP header
            request.Headers.Add("Authorization", "Bearer " + accessToken);

            try
            {
                if (data != null)
                {
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("failed to get request stream.", e);
            }

            // Now try to send the request
            try
            {
                response = request.GetResponse() as HttpWebResponse;
                // Expect the unexpected
                if (request.HaveResponse && response == null)
                {
                    throw new WebException("Response was not returned or is null");
                }
				foreach(var header in response.Headers.AllKeys)
				{
					urlResponse.Headers.Add(header, response.GetResponseHeader(header));
				}

                urlResponse.StatusCode = response.StatusCode;
                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.Created &&
                    response.StatusCode != HttpStatusCode.Accepted &&
                    response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new WebException("Response with status: " + response.StatusCode + " " + response.StatusDescription);
                }
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
                urlResponse.IsError = true;
            }
            finally
            {
                if (response != null)
                {
                    // Get the response content
                    string responseText;
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseText = reader.ReadToEnd();
                    }
                    response.Close();
                    response.Dispose();
                    request.Abort();
                    if (urlResponse.IsError && responseText.Contains("error_message"))
                    {
                        urlResponse.Info = CUrlRequestError.FromJSON<IList<CUrlRequestError>>(responseText);
                    }
                    else
                    {
                        urlResponse.Body = responseText;
                    }
                }
            }

            return urlResponse;

            #endregion
        }

        private CUrlResponse SendHttpRequest(string url, string accessToken, string apiKey, string data, HttpMethod method, bool? isMultipart)
        {
            var urlResponse = new CUrlResponse();
            HttpResponseMessage response = null;

            var req = new HttpRequestMessage(method, ConstructAddress(url, apiKey));

            if (!String.IsNullOrWhiteSpace(data))
                req.Content = new StringContent(data, Encoding.UTF8, "application/json");

            if (isMultipart.HasValue && isMultipart.Value)
            {
                req.Content.Headers.Remove("Content-Type");
                req.Content.Headers.TryAddWithoutValidation("Content-Type",
                    "multipart/form-data; boundary=" + MultipartBuilder.MULTIPART_BOUNDARY);
            }

            req.Headers.Add("Authorization", "Bearer " + accessToken);

            try
            {
                response = client.SendAsync(req).Result;

                if (response == null)
                {
                    throw new WebException("Response was not returned or is null");
                }
                foreach (var header in response.Headers)
                {
                    urlResponse.Headers.Add(header.Key, header.Value.FirstOrDefault());
                }

                urlResponse.StatusCode = response.StatusCode;
                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.Created &&
                    response.StatusCode != HttpStatusCode.Accepted &&
                    response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new WebException("Response with status: " + response.StatusCode + " " + response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (AggregateException ae)
            {
                urlResponse.IsError = true;
                urlResponse.Info = ae.InnerExceptions.Select(e =>
                    new CUrlRequestError { Key = e.GetType().ToString(), Message = e.Message }).ToList();
                return urlResponse;
            }
            catch (Exception e)
            {
                urlResponse.IsError = true;
                urlResponse.Info = new[] {new CUrlRequestError {Key = e.GetType().ToString(), Message = e.Message}};
                return urlResponse;
            }

            if (response != null)
            {
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (urlResponse.IsError && responseText.Contains("error_message"))
                {
                    urlResponse.Info = CUrlRequestError.FromJSON<IList<CUrlRequestError>>(responseText); 
                }
                else
                {
                    urlResponse.Body = responseText;
                }
            }

            return urlResponse;
        }

        private Version GetWrapperAssemblyVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return version;
        }

        private string ConstructAddress(string url, string apiKey)
        {
            var address = url;

            if (!string.IsNullOrEmpty(apiKey))
            {
                address = string.Format("{0}{1}api_key={2}", url, url.Contains("?") ? "&" : "?", apiKey);
            }

            return address;
        }
    }
}
