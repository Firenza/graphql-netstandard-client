using GraphQl.NetStandard.Client;
using Newtonsoft.Json;

namespace GraphQL.NetStandard.Client.UnitTests.Model
{
    public class Repository
    {
        [JsonConverter(typeof(GraphQlNodesParentConverter<PullRequest>))]
        public GraphQlNodesParent<PullRequest> PullRequests { get; set; }
    }
}
