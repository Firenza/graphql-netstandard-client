using GraphQl.NetStandard.Client;
using Newtonsoft.Json;

namespace GraphQL.NetStandard.Client.UnitTests.Model
{
    public class PullRequest
    {
        public string BaseRefName { get; set; }
        public string HeadRefName { get; set; }
        [JsonConverter(typeof(GraphQlNodesParentConverter<Review>))]
        public GraphQlNodesParent<Review> Reviews { get; set; }
    }
}
