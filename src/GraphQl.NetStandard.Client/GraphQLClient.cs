using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GraphQl.NetStandard.Client
{
    public class GraphQLClient : IGraphQLClient
    {
        private readonly HttpClient httpClient;
        private readonly string url;
        private readonly NameValueCollection requestHeaders;

        private GraphQLClient() { }

        public GraphQLClient(HttpClient httpClient, string url)
        {
            this.httpClient = httpClient;
            this.url = url;
        }

        public GraphQLClient(HttpClient httpClient, string url, NameValueCollection requestHeaders) : this(httpClient, url)
        {
            this.requestHeaders = requestHeaders;
        }

        /// <summary>
        /// Returns the JSON body of the GraphQL query response as a string
        /// </summary>
        /// <param name="query"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public async Task<string> QueryAsync(string query, object variables)
        {
            var fullQuery = new GraphQLQuery
            {
                Query = query,
                Variables = variables
            };

            var jsonContent = JsonConvert.SerializeObject(fullQuery);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            if (requestHeaders != null)
            {
                for (int i = 0; i < requestHeaders.Count; i++)
                {
                    requestMessage.Headers.Add(requestHeaders.GetKey(i), requestHeaders.GetValues(i));
                }
            }
    
            requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            string responseContent = null;

            if (response.Content != null)
            {
               responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            CheckGraphQLResponseForErrors(response, responseContent);

            return responseContent;
        }

        /// <summary>
        /// Returns the JSON body of the GraphQL query response as a string
        /// </summary>
        /// <param name="query"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public async Task<string> QueryAsync(string query)
        {
            return await QueryAsync(query, null);
        }

        /// <summary>
        /// Returns the response deserialized into the provided type
        /// </summary>
        /// <param name="query"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public async Task<T> QueryAsync<T>(string query, object variables)
        {
            var stringContent = await QueryAsync(query, variables).ConfigureAwait(false);

            var jObject = JObject.Parse(stringContent);

            var dataString = jObject["data"].First.First.ToString();

            var returnType = JsonConvert.DeserializeObject<T>(dataString);

            return returnType;
        }


        /// <summary>
        /// Returns the response deserialized into the provided type
        /// </summary>
        /// <param name="query"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public async Task<T> QueryAsync<T>(string query)
        {
            return await QueryAsync<T>(query, null);
        }

        private void CheckGraphQLResponseForErrors(HttpResponseMessage httpResponseMessage, string responseContent)
        {
            if (httpResponseMessage.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(responseContent))
            {
                // Check for any errors in the response JSON
                var jObject = JObject.Parse(responseContent);
                var errorsJObject = jObject["errors"];

                if (errorsJObject != null && errorsJObject.HasValues)
                {
                    var errorMessages = new List<string>();

                    foreach (var errorJObject in errorsJObject)
                    {
                        errorMessages.Add(errorJObject["message"].Value<string>());
                    }

                    throw new GraphQLQueryException(errorMessages);
                }
            }
            else if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new GraphQLRequestException(httpResponseMessage.StatusCode, responseContent);
            }
        }
    }
}
