using System.Threading.Tasks;

namespace GraphQl.NetStandard.Client
{
    /// <summary>
    /// Simple client to provide rudimentary GraphQL querying
    /// </summary>
    public interface IGraphQLClient
    {
        /// <summary>
        /// Returns the HTML body of the GraphQL query response as a string
        /// </summary>
        /// <param name="query"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        Task<string> QueryAsync(string query, object variables);

        /// <summary>
        /// Returns the response deserialized into the provided type.  Only useful for simple responses.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        Task<T> QueryAsync<T>(string query, object variables);
    }
}
