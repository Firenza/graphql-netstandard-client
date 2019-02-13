using GraphQl.NetStandard.Client;
using Newtonsoft.Json;

namespace GraphQL.NetStandard.Client.UnitTests.Model
{
    public class Organization
    {
        public Repository Repository { get; set; }

        [JsonConverter(typeof(GraphQlNodesParentConverter<Team>))]
        public GraphQlNodesParent<Team> Teams { get; set; }
    }
}
