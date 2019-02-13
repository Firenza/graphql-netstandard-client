using GraphQl.NetStandard.Client;
using Newtonsoft.Json;

namespace GraphQL.NetStandard.Client.UnitTests.Model
{
    public class Repository
    {
        public string Name { get; set; }

        [JsonConverter(typeof(GraphQlNodesParentConverter<PullRequest>))]
        public GraphQlNodesParent<PullRequest> PullRequests { get; set; }
    }
}
