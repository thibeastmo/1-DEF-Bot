using JsonObjectConverter;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NLBE_Bot
{
    public static class HttpClientRequester
    {
        public static Json SendRequest(string uri, HttpMethod method, List<Tuple<string, string>> headers = null, string content = "")
        {
            if (method == null)
            {
                method = HttpMethod.Get;
            }
            using (HttpClient http = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(method, uri);
                request.Headers.Add("Accept", "application/json");
                if (headers != null && headers.Count > 0)
                {
                    bool firstTime = true;
                    foreach (var header in headers)
                    {
                        string headerKey = header.Item1;
                        //if (firstTime)
                        //{
                        //    firstTime = false;
                        //}
                        //else
                        //{
                        //    headerKey = "&" + headerKey;
                        //}
                        request.Headers.Add(headerKey, header.Item2);
                    }
                }
                HttpResponseMessage response = null;
                if (method == HttpMethod.Get)
                {
                    response = http.SendAsync(request).Result;
                }
                else
                {
                    response = http.PostAsync(uri, new StringContent(content, Encoding.UTF8, "application/json")).Result;
                }
                if (response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    // status-code: 2xx
                    string jsonString = response.Content.ReadAsStringAsync().Result;
                    return new Json(jsonString.Trim().Trim('\n'), string.Empty);

                }
            }
            return null;
        }
    }
}
