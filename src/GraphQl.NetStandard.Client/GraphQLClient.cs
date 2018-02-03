using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GraphQl.NetStandard.Client
{
    /// <summary>
    /// Simple client to provide rudimentary GraphQL querying
    /// </summary>s
    public class GraphQLClient : IGraphQLClient
    {
        private static HttpClient httpClient = new HttpClient();

        private GraphQLClient() { }

        public GraphQLClient(string url)
        {
            httpClient.BaseAddress = new Uri(url);
        }

        public GraphQLClient(string url, List<KeyValuePair<string, string>> requestHeaderKvps) : this(url)
        {
            if (requestHeaderKvps != null)
            {
                foreach (var requestHeaderKvp in requestHeaderKvps)
                {
                    httpClient.DefaultRequestHeaders.Add(requestHeaderKvp.Key, requestHeaderKvp.Value);
                }
            }
        }

        /// <summary>
        /// Returns the HTML body of the GraphQL query response as a string
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

            var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(string.Empty, requestContent).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return responseContent;
        }

        /// <summary>
        /// Returns the response deserialized into the provided type.  Only useful for simple responses.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public async Task<T> QueryAsync<T>(string query, object variables)
        {
            var stringContent = await QueryAsync(query, variables).ConfigureAwait(false);

            var jObject = JObject.Parse(stringContent);
            var dataString = jObject["data"][typeof(T).Name.ToLowerCaseFirstCharacter()].ToString();

            var returnType = JsonConvert.DeserializeObject<T>(dataString);

            return returnType;
        }
    }
}
